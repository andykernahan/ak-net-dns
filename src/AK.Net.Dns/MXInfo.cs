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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AK.Net.Dns
{
    /// <summary>
    /// Contains mail exchange information for a domain.
    /// </summary>
    [Serializable]
    public class MXInfo
    {
        #region Private Fields.

        private static readonly ReadOnlyCollection<DnsName> EMPTY_EXCHANGES =
            new ReadOnlyCollection<DnsName>(new DnsName[0]);

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the MXInfo class for a domain that has no
        /// mail exchanges specified.
        /// </summary>
        /// <param name="domain">The domain.</param>
        public MXInfo(DnsName domain)
        {
            Guard.NotNull(domain, "domain");

            Domain = domain;
            Exchanges = EMPTY_EXCHANGES;
        }

        /// <summary>
        /// Initialises a new instance of the MXInfo class and specifies the owner
        /// domain and the exchanges which are sorted in order of preference.
        /// </summary>
        /// <param name="domain">The owner domain.</param>
        /// <param name="exchanges">The mail exchanges sorted in order of preference.</param>
        public MXInfo(DnsName domain, DnsName[] exchanges)
        {
            Guard.NotNull(domain, "domain");
            Guard.NotNull(exchanges, "exchanges");

            Domain = domain;
            Exchanges = exchanges.Length > 0 ? Array.AsReadOnly(exchanges) : EMPTY_EXCHANGES;
        }

        /// <summary>
        /// Gets the owner domain name.
        /// </summary>
        public DnsName Domain { get; }

        /// <summary>
        /// Gets the exchanges, sorted in order of preference, which are responsible
        /// for processing incoming mail for the owner domain.
        /// </summary>
        public IList<DnsName> Exchanges { get; }

        #endregion
    }
}
