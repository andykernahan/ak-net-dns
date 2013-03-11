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

using System;
using System.Configuration;
using System.Net;

using AK.Net.Dns.IO;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information
    /// <see cref="AK.Net.Dns.IO.DnsUdpTransport"/> instances.
    /// </summary>
    /// <seealso cref="AK.Net.Dns.Configuration.DnsTransportSection"/>
    /// <example>
    /// <para>
    /// The application configuration file below configures all
    /// <see cref="AK.Net.Dns.IO.DnsUdpTransport"/> instances to have a
    /// send and receive timeout of one second and to re-transmit a maximum of
    /// four times.
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///   <configSections>
    ///     <sectionGroup name="ak.net.dns">
    ///       <sectionGroup name="transports">
    ///         <section name="udp" type="AK.Net.Dns.Configuration.DnsUdpTransportSection, AK.Net.Dns"/>          
    ///       </sectionGroup>      
    ///     </sectionGroup>
    ///   </configSections>
    ///   <ak.net.dns>
    ///     <transports>
    ///       <udp receiveTimeout="1000" sendTimeout="1000" transmitRetries="4"/>      
    ///     </transports>    
    ///   </ak.net.dns>  
    /// </configuration>
    /// ]]>
    /// </code>
    /// </example>
    public class DnsUdpTransportSection : DnsTransportSection
    {
        #region Public Interface.

        /// <summary>
        /// Defines the default location of the DnsUdpTransportSection. This field is
        /// constant.
        /// </summary>
        public const string DefaultLocation = DnsTransportSection.BaseLocation + "/udp";

        /// <summary>
        /// Applies this configuration to the specified <paramref name="transport"/>.
        /// </summary>
        /// <param name="transport">The transport to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="transport"/> is <see langword="null"/>.
        /// </exception>
        public override void Apply(DnsTransport transport) {

            base.Apply(transport);
            ((DnsUdpTransport)transport).TransmitRetries = this.TransmitRetries;
        }

        /// <summary>
        /// Gets the DnsUdpTransportSection from its default location.
        /// </summary>
        /// <returns>The DnsUdpTransportSection from its default location.</returns>
        public static DnsUdpTransportSection GetSection() {

            return (DnsUdpTransportSection)ConfigurationManager.GetSection(
                DnsUdpTransportSection.DefaultLocation) ?? new DnsUdpTransportSection();
        }

        /// <summary>
        /// Gets the number of transmit retries that should be attemped.
        /// </summary>
        [IntegerValidator(MinValue = 1)]
        [ConfigurationProperty("transmitRetries", IsRequired = false,
            DefaultValue = DnsUdpTransport.DefaultTransmitRetries)]
        public int TransmitRetries {

            get { return (int)this["transmitRetries"]; }
        }

        #endregion
    }
}
