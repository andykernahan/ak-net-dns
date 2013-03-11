// Copyright 2008 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using System.Net.Sockets;

using AK.Net.Dns.Configuration;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Provides a <see cref="AK.Net.Dns.IDnsTransport"/> implementation which
    /// utilises UDP as the underlying transport protocol.
    /// </summary>
    /// <remarks>
    /// By default, the transport configures itself using the values specified in the
    /// <see cref="AK.Net.Dns.Configuration.DnsUdpTransportSection"/> section.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class DnsUdpTransport : DnsTransport
    {
        #region Private Fields.

        private int _transmitRetries = DnsUdpTransport.DefaultTransmitRetries;

        private const int MILLIS_IN_MICRO = 1000;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the size, in bytes, of a UDP DNS query. This field is constant.
        /// </summary>
        public const int UdpDataSize = 512;

        /// <summary>
        /// Defines the default number of transmit retries should be attemped. This
        /// field is constant.
        /// </summary>
        public const int DefaultTransmitRetries = 4;

        /// <summary>
        /// Initialises a new instance of the DnsUdpTransport class.
        /// </summary>
        public DnsUdpTransport() {

            DnsUdpTransportSection.GetSection().Apply(this);
        }

        /// <summary>
        /// Sends the specified <see cref="AK.Net.Dns.DnsQuery"/> to the specified
        /// end point and return the <see cref="AK.Net.Dns.DnsReply"/>.
        /// </summary>        
        /// <param name="query">The query to send.</param>
        /// <param name="endpoint">The transport end point.</param>
        /// <returns>
        /// The <see cref="AK.Net.Dns.DnsReply"/> to the <see cref="AK.Net.Dns.DnsQuery"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="query"/> or <paramref name="endpoint"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        public override DnsReply Send(DnsQuery query, IPEndPoint endpoint) {

            Guard.NotNull(query, "query");
            Guard.NotNull(endpoint, "endpoint");

            int read;
            int retries = this.TransmitRetries;
            DnsReply reply;
            EndPoint recvEndPoint = endpoint;
            byte[] readBuffer = CreateReadBuffer(query);
            ArraySegment<byte> writeBuffer = WriteQuery(query);            

            using(Socket socket = CreateSocket(endpoint.AddressFamily)) {
                do {
                    try {
                        socket.SendTo(writeBuffer, endpoint);
                        if(!socket.Poll(MillisToMicros(socket.ReceiveTimeout), SelectMode.SelectRead))
                            continue;
                        if((read = socket.ReceiveFrom(readBuffer, ref recvEndPoint)) == 0)
                            continue;
                        // Sanity check.
                        if(!((IPEndPoint)recvEndPoint).Address.Equals(endpoint.Address)) {
                            this.Log.WarnFormat("received reply from non-queried server, server={0}", endpoint);
                            continue;
                        }
                        if(!TryReadReply(readBuffer, 0, read, out reply)) {
                            this.Log.WarnFormat("received malformed reply, server={0}", endpoint);
                            continue;
                        }
                        // Sanity check.
                        if(QueriesAreEqual(reply, query))
                            return reply;
                    } catch(SocketException exc) {
                        this.Log.Warn(exc);
                        throw Guard.DnsTransportFailed(exc);
                    }
                } while(--retries > 0);
            }

            throw Guard.DnsTransportNoEndPointsReplied();
        }

        /// <summary>
        /// Gets or sets the number of transmit retries should be attemped.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a positive number.
        /// </exception>
        public int TransmitRetries {

            get { return _transmitRetries; }
            set {
                Guard.InRange(value > 0, "value");
                _transmitRetries = value;
            }
        }

        #endregion

        #region Private Impl.

        private Socket CreateSocket(AddressFamily addressFamily) {

            Socket socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);

            ConfigureSocket(socket);

            return socket;
        }

        private static byte[] CreateReadBuffer(DnsQuery query) {

            return new byte[DnsUdpTransport.UdpDataSize];
        }

        private static int MillisToMicros(int value) {

            return value * MILLIS_IN_MICRO;
        }

        #endregion
    }
}


