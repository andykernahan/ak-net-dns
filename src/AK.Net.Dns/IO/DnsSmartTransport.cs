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
using AK.Net.Dns.Configuration;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Provides a <see cref="AK.Net.Dns.IDnsTransport"/> implementation which
    /// provides automatic failover to TCP upon UDP failure or message truncation.
    /// </summary>
    /// <remarks>
    /// By default, the transport configures itself using the values specified in the
    /// <see cref="AK.Net.Dns.Configuration.DnsSmartTransportSection"/> section.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class DnsSmartTransport : DnsTransport
    {
        #region Protected Interface.

        /// <summary>
        /// Selects the best transport for the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The best <see cref="AK.Net.Dns.IDnsTransport"/> for the specified
        /// <paramref name="query"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="query"/> is <see langword="null"/>.
        /// </exception>
        protected virtual IDnsTransport SelectTransport(DnsQuery query)
        {
            Guard.NotNull(query, "query");

            var transport = UdpTransport;

            if (query.Questions.Count > 0)
            {
                // TODO does this code belong here?
                switch (query.Questions[0].Type)
                {
                    case DnsQueryType.Axfr:
                        transport = TcpTransport;
                        break;
                    default:
                        break;
                }
            }

            return transport;
        }

        #endregion

        #region Private Fields.

        private IDnsTransport _udpTransport;
        private IDnsTransport _tcpTransport;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the DnsSmartTransport class.
        /// </summary>
        public DnsSmartTransport()
        {
            DnsSmartTransportSection.GetSection().Apply(this);
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
        /// Thrown when <paramref name="endpoint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        public override DnsReply Send(DnsQuery query, IPEndPoint endpoint)
        {
            Guard.NotNull(endpoint, "endpoint");

            DnsReply reply = null;
            var transport = SelectTransport(query);

            try
            {
                reply = transport.Send(query, endpoint);
            }
            catch (DnsTransportException exc)
            {
                // If the selected transport was TCP, we cannot fail over.
                // NOTE this comparison is not thread safe, but at worst it would mean
                // the query being sent again.
                if (transport == TcpTransport)
                {
                    throw;
                }
                Log.Warn("UDP transport failure, failing over to TCP:", exc);
            }

            if (reply != null)
            {
                if (!reply.Header.IsTruncated)
                {
                    return reply;
                }
                Log.InfoFormat("message truncated, failing over to TCP, question={0}", query.Question);
            }

            return TcpTransport.Send(query, endpoint);
        }

        /// <summary>
        /// Gets or sets the UDP <see cref="AK.Net.Dns.IDnsTransport"/> implementation.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public IDnsTransport UdpTransport
        {
            get => _udpTransport;
            set
            {
                Guard.NotNull(value, "value");
                _udpTransport = value;
            }
        }

        /// <summary>
        /// Gets or sets the TCP <see cref="AK.Net.Dns.IDnsTransport"/> implementation.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public IDnsTransport TcpTransport
        {
            get => _tcpTransport;
            set
            {
                Guard.NotNull(value, "value");
                _tcpTransport = value;
            }
        }

        #endregion
    }
}
