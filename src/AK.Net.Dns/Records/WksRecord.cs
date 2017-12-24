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
using System.Collections;
using System.Net;
using AK.Net.Dns.IO;

namespace AK.Net.Dns.Records
{
    /// <summary>
    /// A DNS Well Known Server (WKS) [RFC1035] record which details the services
    /// supported by a host.
    /// </summary>
    [Serializable]
    public class WksRecord : DnsRecord
    {
        #region Private Fields.

        private byte _protocol;
        private IPAddress _address;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of WksRecord records. This field is readonly.
        /// </summary>
        public new static readonly WksRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the WksRecord class and specifies the owner name,
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
        public WksRecord(DnsName owner, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Wks, DnsRecordClass.IN, ttl)
        {
            Guard.NotNull(reader, "reader");

            byte[] buf;
            var rdl = reader.ReadUInt16();

            _address = reader.ReadIPv4Address();
            _protocol = reader.ReadByte();
            buf = new byte[rdl - DnsWireWriter.IPv4AddrLength - 1];
            reader.ReadBytes(buf, 0, buf.Length);
            Bitmap = new BitArray(buf);
        }

        /// <summary>
        /// Initialises a new instance of the TxtRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record, the host address, protocol and
        /// the map of services supported.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="address">The address of the host.</param>
        /// <param name="protocol">The host protocol.</param>
        /// <param name="bitmap">The services supported by the host.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/>, <paramref name="address"/> or
        /// <paramref name="bitmap"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="address"/> is not of the
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork"/> family.
        /// </exception>
        public WksRecord(DnsName owner, TimeSpan ttl, IPAddress address, byte protocol, BitArray bitmap)
            : base(owner, DnsRecordType.Wks, DnsRecordClass.IN, ttl)
        {
            Guard.IsIPv4(address, "address");
            Guard.NotNull(bitmap, "bitmap");

            _address = address;
            _protocol = protocol;
            Bitmap = bitmap;
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

            const int BITS_IN_BYTE = 8;

            byte[] buf;

            writer.WriteIPAddress(Address);
            writer.WriteByte(Protocol);
            if (Bitmap.Count % BITS_IN_BYTE != 0)
            {
                Bitmap.Length = Bitmap.Count + Bitmap.Count % BITS_IN_BYTE;
            }
            buf = new byte[Bitmap.Length / BITS_IN_BYTE];
            Bitmap.CopyTo(buf, 0);
            writer.WriteBytes(buf);
        }

        /// <summary>
        /// Gets or sets the host's IP address.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="value"/> is not of the
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork"/> family.
        /// </exception>
        public IPAddress Address
        {
            get => _address;
            set
            {
                Guard.IsIPv4(value, "value");
                _address = value;
            }
        }

        /// <summary>
        /// Gets or sets the protocol number.
        /// </summary>
        public byte Protocol
        {
            get => _protocol;
            set => _protocol = value;
        }

        /// <summary>
        /// Gets the service bitmap.
        /// </summary>
        public BitArray Bitmap { get; }

        #endregion
    }
}
