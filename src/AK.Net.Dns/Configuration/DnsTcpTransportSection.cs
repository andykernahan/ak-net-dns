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

using System.Configuration;
using AK.Net.Dns.IO;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information 
    /// <see cref="AK.Net.Dns.IO.DnsTcpTransport"/> instances.
    /// </summary>
    /// <seealso cref="AK.Net.Dns.Configuration.DnsTransportSection"/>
    /// <example>
    /// <para>
    /// The application configuration file below configures all
    /// <see cref="AK.Net.Dns.IO.DnsTcpTransport"/> instances to have a
    /// send and receive timeout of one second and a maximum incoming
    /// message size of one megabyte.
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///   <configSections>
    ///     <sectionGroup name="ak.net.dns">
    ///       <sectionGroup name="transports">
    ///         <section name="tcp" type="AK.Net.Dns.Configuration.DnsTcpTransportSection, AK.Net.Dns"/>
    ///       </sectionGroup>      
    ///     </sectionGroup>
    ///   </configSections>
    ///   <ak.net.dns>
    ///     <transports>
    ///       <tcp receiveTimeout="1000" sendTimeout="1000" maxIncomingMessageSize="1048576"/>    
    ///     </transports>    
    ///   </ak.net.dns>  
    /// </configuration>
    /// ]]>
    /// </code>
    /// </example>
    public class DnsTcpTransportSection : DnsTransportSection
    {
        #region Public Interface.

        /// <summary>
        /// Defines the default location of the <see cref="DnsTcpTransportSection"/>.
        /// This field is constant.
        /// </summary>
        public const string DefaultLocation = BaseLocation + "/tcp";

        /// <summary>
        /// Applies this configuration to the specified <paramref name="transport"/>.
        /// </summary>
        /// <param name="transport">The transport to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="transport"/> is <see langword="null"/>.
        /// </exception>
        public override void Apply(DnsTransport transport)
        {
            base.Apply(transport);
            ((DnsTcpTransport)transport).MaxIncomingMessageSize = MaxIncomingMessageSize;
        }

        /// <summary>
        /// Gets the <see cref="DnsTcpTransportSection"/> from its default location.
        /// </summary>
        /// <returns>The <see cref="DnsTcpTransportSection"/> from its default location.</returns>
        public static DnsTcpTransportSection GetSection()
        {
            return (DnsTcpTransportSection)ConfigurationManager.GetSection(
                       DefaultLocation) ?? new DnsTcpTransportSection();
        }

        /// <summary>
        /// Gets, in bytes, the maximum allowable size of an incoming TCP message before a
        /// <see cref="AK.Net.Dns.DnsTransportException"/> is thrown. The default value is
        /// 5MiB.
        /// </summary>
        [IntegerValidator(MinValue = 1)]
        [ConfigurationProperty("maxIncomingMessageSize", IsRequired = false,
            DefaultValue = 5 * 1024 * 1024)]
        public int MaxIncomingMessageSize => (int)this["maxIncomingMessageSize"];

        #endregion
    }
}
