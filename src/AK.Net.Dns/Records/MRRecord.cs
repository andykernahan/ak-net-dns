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
    /// A DNS MR [RFC1035] record which specifies a mailbox rename for the
    /// owner.
    /// </summary>
    [Serializable]
    public class MRRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _newMailbox;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of MRRecord records. This field is readonly.
        /// </summary>
        new public static readonly MRRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the MRRecord class and specifies the owner name,
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
        public MRRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.MR, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _newMailbox = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the MRRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the new mailbox for the
        /// owner name.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="newMailbox">The new mailbox.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="mailbox"/> is
        /// <see langword="null"/>.
        /// </exception>
        public MRRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName newMailbox)
            : base(owner, DnsRecordType.MG, cls, ttl) {

            Guard.NotNull(newMailbox, "newMailbox");

            _newMailbox = newMailbox;
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

            writer.WriteName(this.NewMailbox);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1}", base.ToString(), this.NewMailbox);
        }

        /// <summary>
        /// Gets or sets the new mailbox for the owner.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName NewMailbox {

            get { return _newMailbox; }
            set {
                Guard.NotNull(value, "value");
                _newMailbox = value;
            }
        }

        #endregion
    }
}