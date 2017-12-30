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

using AK.Net.Dns;

namespace AK.NDig.Configuration
{
    /// <summary>
    /// Provides configuration information for <see cref="AK.NDig.Program"/>.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class NDigSection : ConfigurationSection
    {
        #region Public Interface.

        /// <summary>
        /// Defines the default location for NDigSection sections. This field is
        /// constant.
        /// </summary>
        public const string DefaultLocation = "ak.ndig";

        /// <summary>
        /// Gets the NDigSection from it's default location.
        /// </summary>
        /// <returns>The NDigSection from it's default location.</returns>
        public static NDigSection GetSection() {

            return (NDigSection)ConfigurationManager.GetSection(
                NDigSection.DefaultLocation) ?? new NDigSection();
        }

        /// <summary>
        /// Gets or sets the <see cref="AK.NDig.NDigOptions"/>.
        /// </summary>
        [ConfigurationProperty("options", IsRequired = false, 
            DefaultValue = NDigOptions.Default)]
        public NDigOptions Options {

            get { return (NDigOptions)this["options"]; }
            set { this["options"] = value; }
        }

        /*/// <summary>
        /// Gets or sets the default <see cref="AK.Net.Dns.DnsQueryClass"/> value
        /// used when no class has been specified by the command arguments.
        /// </summary>
        [ConfigurationProperty("defaultQueryClass", IsRequired = false,
            DefaultValue = DnsQueryClass.IN)]
        public DnsQueryClass DefaultQueryClass {

            get { return (DnsQueryClass)this["defaultQueryClass"]; }
            set { this["defaultQueryClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the default <see cref="AK.Net.Dns.DnsQueryType"/> value
        /// used when no type has been specified by the command arguments.
        /// </summary>
        [ConfigurationProperty("defaultQueryType", IsRequired = false,
            DefaultValue = DnsQueryType.A)]
        public DnsQueryType DefaultQueryType {

            get { return (DnsQueryType)this["defaultQueryType"]; }
            set { this["defaultQueryType"] = value; }
        }*/

        #endregion
    }
}
