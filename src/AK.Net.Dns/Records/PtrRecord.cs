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
using System.Net.Sockets;
using System.Text;

namespace AK.Net.Dns.Records
{
    /// <summary>
    /// A DNS PTR [RFC1035] record which contains a pointer to another part of the
    /// domain name space.
    /// </summary>
    [Serializable]
    public class PtrRecord : DnsRecord
    {
       #region Private Fields.

        private DnsName _domain;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the suffix for PTR IPv4 query names. This field is constant.
        /// </summary>
        public const string IPv4NameSuffix = "in-addr.arpa.";

        /// <summary>
        /// Defines the suffix for PTR IPv6 query names. This field is constant.
        /// </summary>
        public const string IPv6NameSuffix = "ip6.arpa.";

        /// <summary>
        /// Defines an empty array of PtrRecord records. This field is readonly.
        /// </summary>
        new public static readonly PtrRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the PtrRecord class and specifies the owner name,
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
        public PtrRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Ptr, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _domain = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the PtrRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the domain name pointer.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="domain">The domain name of <paramref name="owner"/>'s authoritative
        /// name server.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> <paramref name="domain"/> is
        /// <see langword="null"/>.
        /// </exception>
        public PtrRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName domain)
            : base(owner, DnsRecordType.Ptr, cls, ttl) {

            Guard.NotNull(domain, "domain");

            _domain = domain;
        }

        /// <summary>
        /// Makes a fully qualified query name using the specified
        /// <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The fully qualified query name</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="address"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="address"/> is not an
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork"/> or
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetworkV6"/> address.
        /// </exception>
        public static DnsName MakeName(IPAddress address) {

            Guard.NotNull(address, "address");   

            switch(address.AddressFamily) {
                case AddressFamily.InterNetwork:
                    return MakeIPv4Name(address);
                case AddressFamily.InterNetworkV6:
                    return MakeIPv6Name(address);
                default:
                    throw Guard.MustBeAnIPv4OrIPv6Addr("address");
            }
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

            writer.WriteName(this.Domain);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1}", base.ToString(), this.Domain);
        }

        /// <summary>
        /// Gets or sets the domain name to which this record points.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Domain {

            get { return _domain; }
            set {
                Guard.NotNull(value, "value");
                _domain = value;
            }
        }

        #endregion

        #region Private Impl.

        private static DnsName MakeIPv4Name(IPAddress address) {
            
            byte[] bytes = address.GetAddressBytes();
            StringBuilder sb = new StringBuilder();

            for(int i = bytes.Length - 1; i >= 0; --i)
                sb.Append(bytes[i].ToString(DnsUtility.DnsCulture)).Append('.');
            sb.Append(PtrRecord.IPv4NameSuffix);

            return DnsName.Parse(sb.ToString());
        }

        private static DnsName MakeIPv6Name(IPAddress address) {            

            byte[] bytes = address.GetAddressBytes();
            StringBuilder sb = new StringBuilder();            

            for(int i = bytes.Length - 1; i >= 0; --i) {                
                sb.Append((bytes[i] & 0x0F).ToString("x", DnsUtility.DnsCulture)).Append('.');
                sb.Append((bytes[i] >> 4).ToString("x", DnsUtility.DnsCulture)).Append('.');
            }
            sb.Append(PtrRecord.IPv6NameSuffix);

            return DnsName.Parse(sb.ToString());
        }

        #endregion
    }
}
