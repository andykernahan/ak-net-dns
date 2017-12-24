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
    /// A DNS DNAME [RFC2672] record which specifies that all queries for
    /// the owner name should be redirected the record's target.
    /// </summary>    
    [Serializable]
    public class DNRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _target;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of DNRecord records. This field is readonly.
        /// </summary>
        public new static readonly DNRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the DNRecord class and specifies the owner name,
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
        public DNRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.CName, cls, ttl)
        {
            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _target = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the DNRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the redirection target.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="target">The canonical name of the owner name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="target"/> is
        /// <see langword="null"/>.
        /// </exception>
        public DNRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName target)
            : base(owner, DnsRecordType.CName, cls, ttl)
        {
            Guard.NotNull(target, "target");

            _target = target;
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

            // The RFC states that the RDATA section should not be compressed.
            writer.WriteName(Target, false);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0} {1}", base.ToString(), Target);
        }

        /// <summary>
        /// Gets or sets the redirection target.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Target
        {
            get => _target;
            set
            {
                Guard.NotNull(value, "value");
                _target = value;
            }
        }

        #endregion
    }
}
