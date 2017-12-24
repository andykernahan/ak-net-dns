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
    /// A DNS HINFO [RFC1035] record which specifies both the CPU and OS type
    /// of the host that corresponds to the owner.
    /// </summary>
    [Serializable]
    public class HInfoRecord : DnsRecord
    {
        #region Private Fields.

        private string _cpu;
        private string _os;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of HInfoRecord records. This field is readonly.
        /// </summary>
        public new static readonly HInfoRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the HInfoRecord class and specifies the owner name,
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
        public HInfoRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.HInfo, cls, ttl)
        {
            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _cpu = reader.ReadCharString();
            _os = reader.ReadCharString();
        }

        /// <summary>
        /// Initialises a new instance of the HInfoRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record, the CPU type and the OS type
        /// of the host.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="cpu">The CPU type.</param>
        /// <param name="os">The OS type.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="cpu"/> <paramref name="os"/> is
        /// <see langword="null"/>.
        /// </exception>
        public HInfoRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, string cpu, string os)
            : base(owner, DnsRecordType.HInfo, cls, ttl)
        {
            Guard.NotNull(cpu, "cpu");
            Guard.NotNull(os, "os");

            _cpu = cpu;
            _os = os;
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

            writer.WriteCharString(Cpu);
            writer.WriteCharString(Os);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0} \"{1}\" \"{2}\"", base.ToString(), Cpu, Os);
        }

        /// <summary>
        /// Gets or sets the OS type.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public string Os
        {
            get => _os;
            set
            {
                Guard.NotNull(value, "value");
                _os = value;
            }
        }

        /// <summary>
        /// Gets or sets the CPU type.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public string Cpu
        {
            get => _cpu;
            set
            {
                Guard.NotNull(value, "value");
                _cpu = value;
            }
        }

        #endregion
    }
}
