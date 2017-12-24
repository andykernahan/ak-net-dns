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

using System.Collections.Generic;
using System.Net;
using System.Threading;
using AK.Net.Dns.Configuration;

namespace AK.Net.Dns.Resolvers
{
    /// <summary>
    /// Provide functionality for querying the Domain Name System by forwarding
    /// queries to a configurable collection of recursive DNS servers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, the resolver configures itself using the values specified in the
    /// <see cref="AK.Net.Dns.Configuration.DnsStubResolverSection"/> section.
    /// </para>    
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class DnsStubResolver : DnsResolver
    {
        #region Protected Interface.

        /// <summary>
        /// Accepts responsibility for completely resolving the specified
        /// <paramref name="query"/> by forwarding it to the collection
        /// of recursive forward servers.
        /// </summary>
        /// <param name="query">The <see cref="AK.Net.Dns.DnsQuery"/> to
        /// resolve.</param>
        /// <returns>The <see cref="AK.Net.Dns.DnsReply"/> containing the
        /// answer to the query.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>        
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        protected override DnsReply Resolve(DnsQuery query)
        {
            Guard.NotNull(query, "query");

            DnsReply reply;

            foreach (var server in Servers)
            {
                try
                {
                    reply = Transport.Send(query, server);
                    return reply;
                }
                catch (DnsException exc)
                {
                    Log.Warn("exception whilst forwarding, server=" + server, exc);
                    MoveServerToTail(server);
                }
            }

            throw Guard.DnsTransportNoEndPointsReplied();
        }

        #endregion

        #region Private Impl.

        private void MoveServerToTail(IPEndPoint server)
        {
            int index;

            lock (Servers)
            {
                if (Servers.Count > 1 &&
                    (index = Servers.IndexOf(server)) > -1 &&
                    index != Servers.Count - 1)
                {
                    Servers.RemoveAt(index);
                    Servers.Add(server);
                    Log.InfoFormat("moved server to tail, tail={0}, head={1}",
                        server, Servers[0]);
                }
            }
        }

        #endregion

        #region Private Fields.

        private static DnsStubResolver _instance;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsStubResolver"/> class.
        /// </summary>
        public DnsStubResolver()
        {
            Servers = new CopyOnMutateCollection<IPEndPoint>();
            DnsStubResolverSection.GetSection().Apply(this);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsStubResolver"/> class
        /// and specifies the endpoints of the recursive forward servers.
        /// </summary>
        /// <param name="servers">The recursive forward servers.</param>
        public DnsStubResolver(IEnumerable<IPEndPoint> servers)
            : this()
        {
            Guard.NotNull(servers, "servers");

            Servers = new CopyOnMutateCollection<IPEndPoint>(servers);
        }

        /// <summary>
        /// Returns a default <see cref="AK.Net.Dns.Resolvers.DnsStubResolver"/>
        /// instance.
        /// </summary>
        /// <returns>The default <see cref="AK.Net.Dns.Resolvers.DnsStubResolver"/>
        /// instance.</returns>
        public static DnsStubResolver Instance()
        {
            if (_instance == null)
            {
                Interlocked.CompareExchange(ref _instance, new DnsStubResolver(), null);
            }
            return _instance;
        }

        /// <summary>
        /// Gets the collection of recursive forward servers.
        /// </summary>
        /// <remarks>
        /// Any modifications to the collection MUST be made under an exclusive lock.
        /// <example>
        /// <code lang="c#">
        /// <![CDATA[
        /// DnsStubResolver resolver = DnsStubResolver.Instance();
        /// 
        /// lock(resolver.Servers) {
        ///     resolver.Servers.Add(new IPEndPoint(IPAddress.Loopback, DnsTransport.DnsPort));
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        public IList<IPEndPoint> Servers { get; }

        #endregion
    }
}
