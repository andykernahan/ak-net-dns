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
    /// A DNS CNAME [RFC1035] record which contains the canonical name of an
    /// owner alias.
    /// </summary>
    [Serializable]
    public class CNameRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _canonical;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of CNameRecord records. This field is readonly.
        /// </summary>
        new public static readonly CNameRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the CNameRecord class and specifies the owner name,
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
        public CNameRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.CName, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _canonical = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the CNameRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the canonical name of the
        /// owner name.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="canonical">The canonical name of the owner name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="canonical"/> is
        /// <see langword="null"/>.
        /// </exception>
        public CNameRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName canonical)
            : base(owner, DnsRecordType.CName, cls, ttl) {

            Guard.NotNull(canonical, "canonical");

            _canonical = canonical;
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            writer.WriteName(this.Canonical);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1}", base.ToString(), this.Canonical);
        }

        /// <summary>
        /// Gets or sets the canonical name of the owner name.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Canonical {

            get { return _canonical; }
            set {
                Guard.NotNull(value, "value");
                _canonical = value;
            }
        }

        #endregion
    }
}
