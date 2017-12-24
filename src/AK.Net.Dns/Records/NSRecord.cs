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

namespace AK.Net.Dns.Records
{
    /// <summary>
    /// A DNS NS [RFC1035] record which contains the authoritative domain name
    /// server for a specific class and domain.
    /// </summary>
    [Serializable]
    public class NSRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _domain;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of NSRecord records. This field is readonly.
        /// </summary>
        public new static readonly NSRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the NSRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the reply reader from
        /// which the RDLENGTH and RDATA section of the resource record will be read.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="reader">The reply reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="reader"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the RDATA section of the record could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public NSRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.NS, cls, ttl)
        {
            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _domain = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the NSRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the domain name of the
        /// authoritative name server.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="domain">The domain name of <paramref name="owner"/>'s authoritative
        /// name server.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="domain"/> is
        /// <see langword="null"/>.
        /// </exception>
        public NSRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName domain)
            : base(owner, DnsRecordType.NS, cls, ttl)
        {
            Guard.NotNull(domain, "domain");

            _domain = domain;
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer)
        {
            Guard.NotNull(writer, "writer");

            writer.WriteName(Domain);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0} {1}", base.ToString(), Domain);
        }

        /// <summary>
        /// Gets or sets the domain name of the authoritative name server.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Domain
        {
            get => _domain;
            set
            {
                Guard.NotNull(value, "value");
                _domain = value;
            }
        }

        #endregion
    }
}
