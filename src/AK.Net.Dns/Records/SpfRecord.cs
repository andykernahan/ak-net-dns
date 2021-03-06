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
    /// A DNS Send Policy Framework (SPF) [RFC4408] record which contains
    /// SPF specification associated with the owner.
    /// </summary>
    [Serializable]
    public class SpfRecord : DnsRecord
    {
        #region Private Fields.

        private string _specification;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of SpfRecord records. This field is readonly.
        /// </summary>
        new public static readonly SpfRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the SpfRecord class and specifies the owner name,
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
        public SpfRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Spf, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _specification = reader.ReadCharString();
        }

        /// <summary>
        /// Initialises a new instance of the SpfRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the SPF specification.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="specification">The SPF specification.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="specification"/> is
        /// <see langword="null"/>.
        /// </exception>
        public SpfRecord(DnsName owner, TimeSpan ttl, string specification)
            : base(owner, DnsRecordType.Spf, DnsRecordClass.IN, ttl) {

                Guard.NotNull(specification, "specification");

            _specification = specification;
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

            writer.WriteCharString(this.Specification);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} \"{1}\"", base.ToString(), this.Specification);
        }

        /// <summary>
        /// Gets or sets the SPF specification.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public string Specification {

            get { return _specification; }
            set {
                Guard.NotNull(value, "value");
                _specification = value;
            }
        }

        #endregion
    }
}
