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

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="AK.Net.Dns.Configuration.DnsEndPointElement"/> instances. This
    /// class cannot be inherited.
    /// </summary>
    public sealed class DnsEndPointElementCollection : ConfigurationElementCollection
    {
        #region Public Interface.

        /// <summary>
        /// Intialises a new instance of the DnsEndPointElementCollection class.
        /// </summary>
        public DnsEndPointElementCollection()
        {
            AddElementName = "endpoint";
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Creates a new <see cref="AK.Net.Dns.Configuration.DnsEndPointElement"/>
        /// configuration element.
        /// </summary>
        /// <returns>A new <see cref="AK.Net.Dns.Configuration.DnsEndPointElement"/>
        /// configuration element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DnsEndPointElement();
        }

        /// <summary>
        /// Gets the key for the specified configuration element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the specified configuration element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DnsEndPointElement)element).Endpoint;
        }

        /// <summary>
        /// Gets a value indicating if duplicate configurations elements should result in a
        /// <see cref="System.Configuration.ConfigurationException"/> being thrown.
        /// </summary>
        protected override bool ThrowOnDuplicate => true;

        #endregion
    }
}
