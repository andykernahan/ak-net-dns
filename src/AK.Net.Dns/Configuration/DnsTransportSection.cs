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
    /// Provides configuration information for 
    /// <see cref="AK.Net.Dns.IO.DnsTransport"/> instances. This class
    /// is <see langword="abstract"/>.
    /// </summary>
    public abstract class DnsTransportSection : ConfigurationSection
    {
        #region Public Interface.

        /// <summary>
        /// Defines the base location for <see cref="DnsTransportSection"/>
        /// sections. This field is constant.
        /// </summary>
        public const string BaseLocation = "ak.net.dns/transports";

        /// <summary>
        /// Applies this configuration to the specified <paramref name="transport"/>.
        /// </summary>
        /// <param name="transport">The transport to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="transport"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Apply(DnsTransport transport) {

            Guard.NotNull(transport, "transport");

            transport.ReceiveTimeout = this.ReceiveTimeout;
            transport.SendTimeout = this.SendTimeout;
        }

        /// <summary>
        /// Gets, in milliseconds, the transport send timeout
        /// </summary>
        [IntegerValidator(MinValue = 0)]
        [ConfigurationProperty("sendTimeout", IsRequired = false,
            DefaultValue = DnsTransport.DefaultTimeout)]
        public int SendTimeout {

            get { return (int)this["sendTimeout"]; }
        }

        /// <summary>
        /// Gets, in milliseconds, the transport receive timeout
        /// </summary>
        [IntegerValidator(MinValue = 0)]
        [ConfigurationProperty("receiveTimeout", IsRequired = false,
            DefaultValue = DnsTransport.DefaultTimeout)]
        public int ReceiveTimeout {

            get { return (int)this["receiveTimeout"]; }
        }

        #endregion
    }
}
