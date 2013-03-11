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
    /// A DNS MINFO [RFC1035] record which contains mailing information relating to
    /// a domain mailbox.
    /// </summary>
    [Serializable]
    public class MInfoRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _rMbox;
        private DnsName _eMbox;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of MInfoRecord records. This field is readonly.
        /// </summary>
        new public static readonly MInfoRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the MInfoRecord class and specifies the owner name,
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
        public MInfoRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.HInfo, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _rMbox = reader.ReadName();
            _eMbox = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the MInfoRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record, the responsible mailbox and the
        /// mailbox to which errors are sent.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="rMbox">The domain name of the mailbox which is responsible for the mailing
        /// list or mailbox.</param>
        /// <param name="eMbox">The domain name of the mailbox which errors are mailed to.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="rMbox"/> <paramref name="eMbox"/> is
        /// <see langword="null"/>.
        /// </exception>
        public MInfoRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName rMbox, DnsName eMbox)
            : base(owner, DnsRecordType.HInfo, cls, ttl) {

            Guard.NotNull(rMbox, "rMbox");
            Guard.NotNull(eMbox, "eMbox");

            _rMbox = rMbox;
            _eMbox = eMbox;
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            writer.WriteName(this.RMbox);
            writer.WriteName(this.EMbox);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1} {2}", base.ToString(), this.RMbox, this.EMbox);
        }

        /// <summary>
        /// Gets or sets the domain name of the mailbox which errors are mailed to.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName EMbox {

            get { return _eMbox; }
            set {
                Guard.NotNull(value, "value");
                _eMbox = value;
            }
        }

        /// <summary>
        /// Gets or sets the domain name of the mailbox which is responsible for the mailing
        /// list or mailbox.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName RMbox {

            get { return _rMbox; }
            set {
                Guard.NotNull(value, "value");
                _rMbox = value;
            }
        }

        #endregion
    }
}
