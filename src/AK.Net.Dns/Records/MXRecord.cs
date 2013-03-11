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
    /// A DNS MX [RFC1035] record which specifies a single mail-exchange agent
    /// for the owner and a preference of that agent over others for the owner.
    /// </summary>
    [Serializable]
    public class MXRecord : DnsRecord, IComparable<MXRecord>, IComparable
    {
        #region Private Fields.

        private short _preference;
        private DnsName _exchange;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of MXRecord records. This field is readonly.
        /// </summary>
        new public static readonly MXRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the MXRecord class and specifies the owner name,
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
        public MXRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.MX, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _preference = reader.ReadInt16();
            _exchange = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the MXRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record, the preference of the mail
        /// exchange and the domain of the exchange itself.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="exchange">The domain name of the exchange.</param>
        /// <param name="preference">The preference of the exchange.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="exchange"/> is
        /// <see langword="null"/>.
        /// </exception>
        public MXRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, short preference,
            DnsName exchange)
            : base(owner, DnsRecordType.MX, cls, ttl) {

            Guard.NotNull(exchange, "exchange");

            _preference = preference;
            _exchange = exchange;
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

            writer.WriteInt16(this.Preference);
            writer.WriteName(this.Exchange);
        }

        /// <summary>
        /// Returns a value indicating relative eqaulity with the 
        /// <paramref name="other"/> instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>A value indicating relative eqaulity with the 
        /// <paramref name="other"/> instance.</returns>
        public int CompareTo(MXRecord other) {

            if(other == null)
                return 1;

            int res = this.Owner.CompareTo(other.Owner);

            return res != 0 ? res : this.Preference.CompareTo(other.Preference);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1,-2} {2}", base.ToString(), this.Preference,
                this.Exchange);
        }

        /// <summary>
        /// Gets or sets the domain name of the exchange.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Exchange {

            get {  return _exchange; }
            set {
                Guard.NotNull(value, "value");
                _exchange = value;
            }
        }

        /// <summary>
        /// Gets or sets the prefence of the exchange.
        /// </summary>
        public short Preference {

            get { return _preference; }
            set { _preference = value; }
        }

        #endregion

        #region Explicit Interface.

        int IComparable.CompareTo(object obj) {

            return CompareTo(obj as MXRecord);            
        }

        #endregion
    }
}
