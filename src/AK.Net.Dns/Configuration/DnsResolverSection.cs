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
using AK.Net.Dns.Caching;
using AK.Net.Dns.Configuration.TypeConversion;
using AK.Net.Dns.IO;
using log4net;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Provides configuration information for <see cref="AK.Net.Dns.IDnsResolver"/>
    /// instances. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class DnsResolverSection : ConfigurationSection
    {
        #region Protected Interface.

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>
        protected ILog Log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

        #endregion

        #region Private Fields.

        private ILog _log;
        private IDnsCache _cacheInstance;
        private IDnsTransport _transportInstance;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the base location for DnsResolverSection sections. This field is
        /// constant.
        /// </summary>
        public const string BaseLocation = "ak.net.dns/resolvers";

        /// <summary>
        /// Type initialiser for DnsResolverSection.
        /// </summary>
        static DnsResolverSection()
        {
            DnsNameTypeDescriptionProvider.RegisterProvider();
            RuntimeTypeTypeDescriptionProvider.RegisterProvider();
        }

        /// <summary>
        /// Applies this configuration to the specified <paramref name="resolver"/>.
        /// </summary>
        /// <param name="resolver">The resolver to be configured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="resolver"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Apply(IDnsResolver resolver)
        {
            Guard.NotNull(resolver, "resolver");

            resolver.NameSuffix = NameSuffix;
            resolver.Cache = CacheInstance;
            resolver.Transport = TransportInstance;
        }

        /// <summary>
        /// Gets the the <see cref="AK.Net.Dns.DnsName"/> suffix used to convert relative
        /// names to absolute names before they are queried.
        /// </summary>
        [ConfigurationProperty("nameSuffix", IsRequired = false)]
        public DnsName NameSuffix => (DnsName)this["nameSuffix"];

        /// <summary>
        /// Gets the <see cref="AK.Net.Dns.IDnsCache"/> type used by the resolver.
        /// </summary>
        [BaseTypeValidator(typeof(IDnsCache))]
        [ConfigurationProperty("cache", IsRequired = false, DefaultValue = typeof(DnsNullCache))]
        public Type CacheType => (Type)this["cache"];

        /// <summary>
        /// Returns the <see cref="AK.Net.Dns.IDnsCache"/> instance used by the resolver.
        /// </summary>
        public IDnsCache CacheInstance
        {
            get
            {
                if (_cacheInstance == null)
                {
                    _cacheInstance = (IDnsCache)Activator.CreateInstance(CacheType);
                }
                return _cacheInstance;
            }
        }

        /// <summary>
        /// Gets the <see cref="AK.Net.Dns.IDnsTransport"/> type used by the resolver.
        /// </summary>
        [BaseTypeValidator(typeof(IDnsTransport))]
        [ConfigurationProperty("transport", IsRequired = false, DefaultValue = typeof(DnsSmartTransport))]
        public Type TransportType => (Type)this["transport"];

        /// <summary>
        /// Returns the <see cref="AK.Net.Dns.IDnsTransport"/> instance used by the resolver.
        /// </summary>
        public IDnsTransport TransportInstance
        {
            get
            {
                if (_transportInstance == null)
                {
                    _transportInstance = (IDnsTransport)Activator.CreateInstance(TransportType);
                }
                return _transportInstance;
            }
        }

        #endregion
    }
}
