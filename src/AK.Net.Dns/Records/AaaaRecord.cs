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
using System.Net;

namespace AK.Net.Dns.Records
{
    /// <summary>
    /// A DNS AAAA [RFC3596] record which contains the IPv6
    /// <see cref="System.Net.IPAddress"/> of the owner domain.
    /// </summary>
    [Serializable]
    public class AaaaRecord : DnsRecord
    {
        #region Private Fields.

        private IPAddress _address;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of AaaaRecord records. This field is readonly.
        /// </summary>
        public new static readonly AaaaRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the AaaaRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the reply reader from
        /// which the RDLENGTH and RDATA section of the resource record will be read.
        /// </summary>
        /// <param name="owner">The owner name.</param>
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
        public AaaaRecord(DnsName owner, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Aaaa, DnsRecordClass.IN, ttl)
        {
            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _address = reader.ReadIPv6Address();
        }

        /// <summary>
        /// Initialises a new instance of the AaaaRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the owner's IP address.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="address">The owner's IP address.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="address"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="address"/> is not of the
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetworkV6"/> family.
        /// </exception>
        public AaaaRecord(DnsName owner, TimeSpan ttl, IPAddress address)
            : base(owner, DnsRecordType.Aaaa, DnsRecordClass.IN, ttl)
        {
            Guard.IsIPv6(address, "address");

            _address = address;
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

            writer.WriteIPAddress(Address);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0} {1}", base.ToString(), Address);
        }

        /// <summary>
        /// Gets or sets the owner's IP address.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="value"/> is not of the
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetworkV6"/> family.
        /// </exception>
        public IPAddress Address
        {
            get => _address;
            set
            {
                Guard.IsIPv6(value, "value");
                _address = value;
            }
        }

        #endregion
    }
}
