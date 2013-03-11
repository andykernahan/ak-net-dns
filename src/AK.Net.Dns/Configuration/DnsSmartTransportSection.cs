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
using System.Collections.Generic;
using System.Net;

using AK.Net.Dns.IO;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information for
    /// <see cref="AK.Net.Dns.IO.DnsSmartTransport"/> instances.
    /// </summary>    
    public class DnsSmartTransportSection : DnsTransportSection
    {
        #region Private Fields.

        private IDnsTransport _udpTransportInstance;
        private IDnsTransport _tcpTransportInstance;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the default location of the DnsSmartTransportSection. This field is
        /// constant.
        /// </summary>
        public const string DefaultLocation = DnsTransportSection.BaseLocation + "/smart";

        /// <summary>
        /// Applies this configuration to the specified <paramref name="transport"/>.
        /// </summary>
        /// <param name="transport">The transport to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="transport"/> is <see langword="null"/>.
        /// </exception>
        public override void Apply(DnsTransport transport) {

            DnsSmartTransport smart = (DnsSmartTransport)transport;

            base.Apply(transport);
            smart.UdpTransport = this.UdpTransportInstance;
            smart.TcpTransport = this.TcpTransportInstance;            
        }

        /// <summary>
        /// Gets the DnsSmartTransportSection from its default location.
        /// </summary>
        /// <returns>The DnsSmartTransportSection from its default location.</returns>
        public static DnsSmartTransportSection GetSection() {

            return (DnsSmartTransportSection)ConfigurationManager.GetSection(
                DnsSmartTransportSection.DefaultLocation) ?? new DnsSmartTransportSection();
        }

        /// <summary>
        /// Gets the UDP <see cref="AK.Net.Dns.IDnsTransport"/> type used by the transport.
        /// </summary>
        [BaseTypeValidator(typeof(IDnsTransport))]
        [ConfigurationProperty("udpTransport", IsRequired = false, DefaultValue = typeof(DnsUdpTransport))]
        public Type UdpTransportType {

            get { return (Type)this["udpTransport"]; }
        }

        /// <summary>
        /// Returns the UDP <see cref="AK.Net.Dns.IDnsTransport"/> instance used by the transport.
        /// </summary>
        public IDnsTransport UdpTransportInstance {

            get {
                if(_udpTransportInstance == null)
                    _udpTransportInstance = (IDnsTransport)Activator.CreateInstance(this.UdpTransportType);
                return _udpTransportInstance;
            }
        }

        /// <summary>
        /// Gets the TCP <see cref="AK.Net.Dns.IDnsTransport"/> type used by the transport.
        /// </summary>
        [BaseTypeValidator(typeof(IDnsTransport))]
        [ConfigurationProperty("tcpTransport", IsRequired = false, DefaultValue = typeof(DnsTcpTransport))]
        public Type TcpTransportType {

            get { return (Type)this["tcpTransport"]; }
        }

        /// <summary>
        /// Returns the TCP <see cref="AK.Net.Dns.IDnsTransport"/> instance used by the transport.
        /// </summary>
        public IDnsTransport TcpTransportInstance {

            get {
                if(_tcpTransportInstance == null)
                    _tcpTransportInstance = (IDnsTransport)Activator.CreateInstance(this.TcpTransportType);
                return _tcpTransportInstance;
            }
        }

        #endregion
    }
}
