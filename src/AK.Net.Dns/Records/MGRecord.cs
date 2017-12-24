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
    /// A DNS MG [RFC1035] record which specifies a mailbox that is a member
    /// of mail group with the same name as the owner.
    /// </summary>
    [Serializable]
    public class MGRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _mailbox;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of MGRecord records. This field is readonly.
        /// </summary>
        public new static readonly MGRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the MGRecord class and specifies the owner name,
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
        public MGRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.MG, cls, ttl)
        {
            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _mailbox = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the MGRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the <paramref name="mailbox"/>
        /// that is a member of the group.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="mailbox">The mail group member.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="mailbox"/> is
        /// <see langword="null"/>.
        /// </exception>
        public MGRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName mailbox)
            : base(owner, DnsRecordType.MG, cls, ttl)
        {
            Guard.NotNull(mailbox, "mailbox");

            _mailbox = mailbox;
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer)
        {
            Guard.NotNull(writer, "writer");

            writer.WriteName(Mailbox);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0} {1}", base.ToString(), Mailbox);
        }

        /// <summary>
        /// Gets or sets the mail group member.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Mailbox
        {
            get => _mailbox;
            set
            {
                Guard.NotNull(value, "value");
                _mailbox = value;
            }
        }

        #endregion
    }
}
