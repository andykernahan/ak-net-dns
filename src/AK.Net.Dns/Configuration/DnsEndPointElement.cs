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
using System.Net;
using AK.Net.Dns.Configuration.TypeConversion;
using AK.Net.Dns.IO;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information for a single end point. This class
    /// cannot be inherited.
    /// </summary>
    public sealed class DnsEndPointElement : ConfigurationElement
    {
        #region Private Fields.

        private IPEndPoint _endpoint;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Type initialiser for DnsEndPointElement.
        /// </summary>
        static DnsEndPointElement()
        {
            IPAddressTypeDescriptionProvider.RegisterProvider();
        }

        /// <summary>
        /// Gets the end point <see cref="System.Net.IPAddress"/>.
        /// </summary>
        [ConfigurationProperty("address", IsRequired = true)]
        public IPAddress Address => (IPAddress)this["address"];

        /// <summary>
        /// Gets the end point port number.
        /// </summary>
        [IntegerValidator(MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        [ConfigurationProperty("port", IsRequired = false, DefaultValue = DnsTransport.DnsPort)]
        public int Port => (int)this["port"];

        /// <summary>
        /// Gets the constructed <see cref="System.Net.IPEndPoint"/>.
        /// </summary>
        public IPEndPoint Endpoint
        {
            get
            {
                if (_endpoint == null)
                {
                    _endpoint = new IPEndPoint(Address, Port);
                }
                return _endpoint;
            }
        }

        #endregion
    }
}
