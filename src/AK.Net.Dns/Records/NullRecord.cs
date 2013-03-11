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
    /// A DNS NULL [RFC1035] record whose data section contains an array of
    /// un-interpreted bytes.
    /// </summary>
    [Serializable]
    public class NullRecord : DnsRecord
    {
        #region Private Fields.

        private byte[] _data;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of NullRecord records. This field is readonly.
        /// </summary>
        new public static readonly NullRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the NullRecord class and specifies the owner name,
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
        public NullRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Null, cls, ttl) {

            Guard.NotNull(reader, "reader");

            _data = reader.ReadBytes(reader.ReadUInt16());
        }

        /// <summary>
        /// Initialises a new instance of the NSRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the record data.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="data">The resource record data.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="data"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="address"/> is not of the
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork"/> family.
        /// </exception>
        public NullRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, byte[] data)
            : base(owner, DnsRecordType.Null, cls, ttl) {

            Guard.NotNull(data, "data");

            _data = data;
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

            writer.WriteBytes(this.Data);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1}", base.ToString(), DnsUtility.ToString(this.Data));
        }

        /// <summary>
        /// Gets or sets the resource record RDATA.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public byte[] Data {

            get { return _data; }
            set {
                Guard.NotNull(value, "value");
                _data = value;
            }
        }

        #endregion
    }
}
