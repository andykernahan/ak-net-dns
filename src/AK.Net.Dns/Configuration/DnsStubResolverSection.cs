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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using AK.Net.Dns.IO;
using AK.Net.Dns.Resolvers;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information for
    /// <see cref="AK.Net.Dns.Resolvers.DnsStubResolver"/> instances.
    /// </summary>
    /// <seealso cref="AK.Net.Dns.Configuration.DnsResolverSection"/>
    /// <remarks>
    /// When a <see cref="AK.Net.Dns.Resolvers.DnsStubResolver"/> instance is 
    /// configured using <see cref="AK.Net.Dns.Configuration.DnsStubResolverSection.Apply"/>
    /// and <see cref="AK.Net.Dns.Configuration.DnsStubResolverSection.DiscoverServers"/>
    /// is <see langword="true"/>, the configuration will attempt to discover the local
    /// network servers using the facilities provided by the
    /// <see cref="System.Net.NetworkInformation"/> namespace. Once discovery is complete
    /// and the endpoints have been added to the resolver, the endpoints specified
    /// in the application configuration file are added.
    /// <para>
    /// If, after configuration, no endpoints have been added to the resolver; an error
    /// is logged, but no exception is thrown.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// The application configuration file below configures all
    /// <see cref="AK.Net.Dns.Resolvers.DnsStubResolver"/> instances to forward queries to
    /// the recursive servers provided by OpenDNS.
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///   <configSections>
    ///     <sectionGroup name="ak.net.dns">
    ///       <sectionGroup name="resolvers">
    ///         <section name="stub" type="AK.Net.Dns.Configuration.DnsStubResolverSection, AK.Net.Dns"/>  
    ///       </sectionGroup>
    ///     </sectionGroup>
    ///   </configSections>
    ///   <ak.net.dns>
    ///     <resolvers>
    ///       <stub discoverServers="false">
    ///         <endpoint address="208.67.222.222"/>
    ///         <!-- the port may also be specified -->
    ///         <endpoint address="208.67.220.220" port="53"/>
    ///       </stub>
    ///     </resolvers>
    ///   </ak.net.dns>  
    /// </configuration>
    /// ]]>
    /// </code>
    /// <para>
    /// The application configuration file below shows a complete example.
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///   <configSections>
    ///     <sectionGroup name="ak.net.dns">
    ///       <sectionGroup name="resolvers">
    ///         <section name="stub" type="AK.Net.Dns.Configuration.DnsStubResolverSection, AK.Net.Dns"/>  
    ///       </sectionGroup>
    ///     </sectionGroup>
    ///   </configSections>
    ///   <ak.net.dns>
    ///     <resolvers>
    ///       <stub discoverServers="true" nameSuffix="example.com." 
    ///             transport="AK.Net.Dns.IO.DnsSmartTransport, AK.Net.Dns" 
    ///             cache="AK.Net.Dns.Caching.DnsNullCache, AK.Net.Dns">
    ///         <endpoint address="208.67.222.222"/>
    ///         <endpoint address="208.67.220.220"/>
    ///       </stub>
    ///     </resolvers>
    ///   </ak.net.dns>  
    /// </configuration>
    /// ]]>
    /// </code>
    /// </example>
    public class DnsStubResolverSection : DnsResolverSection
    {
        #region Private Impl.

        private IEnumerable<IPEndPoint> DiscoverNetworkServers()
        {
            try
            {
                return (from network in NetworkInterface.GetAllNetworkInterfaces()
                    where network.OperationalStatus == OperationalStatus.Up &&
                          network.NetworkInterfaceType != NetworkInterfaceType.Loopback
                    from address in network.GetIPProperties().DnsAddresses
                    select new IPEndPoint(address, DnsTransport.DnsPort)).Distinct();
            }
            catch (NetworkInformationException exc)
            {
                Log.Error("discovery of network servers failed due to exception:", exc);
                return DnsUtility.EMPTY_EP_ARRAY;
            }
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the default location of the DnsStubResolverSection. This field is
        /// constant.
        /// </summary>
        public const string DefaultLocation = BaseLocation + "/stub";

        /// <summary>
        /// Applies this configuration to the specified <paramref name="resolver"/>.
        /// </summary>
        /// <param name="resolver">The resolver to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="resolver"/> is <see langword="null"/>.
        /// </exception>
        public override void Apply(IDnsResolver resolver)
        {
            var stub = (DnsStubResolver)resolver;

            base.Apply(resolver);
            lock (stub.Servers)
            {
                stub.Servers.Clear();
                if (DiscoverServers)
                {
                    Log.Info("discovering network servers");
                    foreach (var server in DiscoverNetworkServers())
                    {
                        stub.Servers.Add(server);
                        Log.InfoFormat("discovered server, ep={0}", server);
                    }
                }
                foreach (DnsEndPointElement element in Servers)
                {
                    stub.Servers.Add(element.Endpoint);
                }
                if (stub.Servers.Count == 0)
                {
                    Log.Error("no servers specified in configuration and dicovery of network servers " +
                              "failed to yield any servers or is disabled");
                }
            }
        }

        /// <summary>
        /// Gets the DnsStubResolverSection from its default location.
        /// </summary>
        /// <returns>The DnsStubResolverSection from its default location.</returns>
        public static DnsStubResolverSection GetSection()
        {
            return (DnsStubResolverSection)ConfigurationManager.GetSection(
                       DefaultLocation) ?? new DnsStubResolverSection();
        }

        /// <summary>
        /// Gets or sets the collection of end point configuration elements which
        /// specify the recursive forward servers to which the resolver forwards all
        /// queries.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public DnsEndPointElementCollection Servers
        {
            get => (DnsEndPointElementCollection)this[""];
            set => this[""] = value;
        }

        /// <summary>
        /// Gets a value indicating if the configuration should discover the local network
        /// servers or only use those specified in the configuration file.
        /// </summary>
        /// <remarks>If this property is <see langword="true"/> then the discovered network
        /// servers will take priority over those specified in the configuration file.</remarks>
        [ConfigurationProperty("discoverServers", IsRequired = false, DefaultValue = true)]
        public bool DiscoverServers => (bool)this["discoverServers"];

        #endregion
    }
}
