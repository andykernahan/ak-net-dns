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

using System.Net;
using System.Net.Sockets;
using AK.Net.Dns.Configuration;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Provides a <see cref="AK.Net.Dns.IDnsTransport"/> implementation which
    /// utilises TCP as the underlying transport protocol.
    /// </summary>
    /// <remarks>
    /// By default, the transport configures itself using the values specified in the
    /// <see cref="AK.Net.Dns.Configuration.DnsTcpTransportSection"/> section.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class DnsTcpTransport : DnsTransport
    {
        #region Private Fields.

        private int _maxIncomingMessageSize;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the DnsTcpTransport class.
        /// </summary>
        public DnsTcpTransport()
        {
            DnsTcpTransportSection.GetSection().Apply(this);
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
        public override DnsReply Send(DnsQuery query, IPEndPoint endpoint)
        {
            Guard.NotNull(query, "query");
            Guard.NotNull(endpoint, "endpoint");

            int length;
            DnsReply reply;
            byte[] readBuffer;
            var writeBuffer = WriteQuery(query);
            var lengthBuffer = WriteUInt16(writeBuffer.Count);

            using (var socket = CreateSocket(endpoint.AddressFamily))
            {
                try
                {
                    socket.Connect(endpoint);
                    socket.Send(lengthBuffer);
                    socket.Send(writeBuffer);
                    if (socket.TryReceiveBuffer(lengthBuffer))
                    {
                        length = ReadUInt16(lengthBuffer);
                        CheckIncomingMessageSize(length, endpoint);
                        readBuffer = new byte[length];
                        if (socket.TryReceiveBuffer(readBuffer))
                        {
                            if (!TryReadReply(readBuffer, out reply))
                            {
                                Log.WarnFormat("received malformed reply, server={0}", endpoint);
                            }
                            else if (QueriesAreEqual(reply, query))
                            {
                                Log.DebugFormat("query answered, server={0}", endpoint);
                                return reply;
                            }
                        }
                    }
                }
                catch (SocketException exc)
                {
                    Log.Warn(exc);
                    throw Guard.DnsTransportFailed(exc);
                }
            }

            throw Guard.DnsTransportNoEndPointsReplied();
        }

        /// <summary>
        /// Gets or sets, in bytes, the maximum allowable size of an incoming TCP message
        /// before a <see cref="AK.Net.Dns.DnsTransportException"/> is thrown.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a positive number.
        /// </exception>
        public int MaxIncomingMessageSize
        {
            get => _maxIncomingMessageSize;
            set
            {
                Guard.InRange(value > 0, "value");
                _maxIncomingMessageSize = value;
            }
        }

        #endregion

        #region Private Impl.

        private Socket CreateSocket(AddressFamily addressFamily)
        {
            var socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

            ConfigureSocket(socket);

            return socket;
        }

        private void CheckIncomingMessageSize(int size, IPEndPoint endpoint)
        {
            if (size < 1)
            {
                Log.WarnFormat("received empty message, server={0}", endpoint);
                throw Guard.DnsTransportReceivedEmptyMessage();
            }
            if (size > MaxIncomingMessageSize)
            {
                Log.WarnFormat("incoming message size exceeded maximum, size={0}, maximum={1}, server={2}",
                    size, MaxIncomingMessageSize, endpoint);
                throw Guard.DnsTransportIncomingMessageToLarge(size, MaxIncomingMessageSize);
            }
        }

        private static int ReadUInt16(byte[] buffer)
        {
            return (buffer[0] << 8) | buffer[1];
        }

        private static byte[] WriteUInt16(int value)
        {
            return new[] {(byte)(value >> 8), (byte)value};
        }

        #endregion
    }
}
