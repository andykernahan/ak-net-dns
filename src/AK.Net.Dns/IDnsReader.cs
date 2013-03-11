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

namespace AK.Net.Dns
{
    /// <summary>
    /// Defines a reader which reads DNS types from an underlying data stream.
    /// </summary>
    public interface IDnsReader : IDisposable
    {
        /// <summary>
        /// Gets a value indicating the reader has been disposed of.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Reads a name from the data stream.
        /// </summary>
        /// <returns>The read name.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsName ReadName();

        /// <summary>
        /// Reads a single byte from the data stream and advances the position by one.
        /// </summary>
        /// <returns>A byte read from the data stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        byte ReadByte();

        /// <summary>
        /// Reads a sequence of bytes from the underlying data.
        /// </summary>
        /// <param name="count">The number of bytes to read from the data.</param>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a sequence of bytes from the underlying data.
        /// </summary>
        /// <param name="buffer">The buffer into which the data is written.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which
        /// writing beings.</param>
        /// <param name="count">The number of bytes to read from the data.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="offset"/> is negative or greater than the length
        /// of <paramref name="buffer"/>
        /// </item>
        /// <item>
        /// <paramref name="count"/> is negative or greater than the length of
        /// <paramref name="buffer"/> given <paramref name="offset"/>.
        /// </item>
        /// <item>
        /// <paramref name="count"/> is greater than the number of bytes remaining
        /// on the underlying data stream.
        /// </item>
        /// </list>
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        void ReadBytes(byte[] buffer, int offset, int count);

        /// <summary>
        /// Reads a <see cref="System.UInt16"/> from the data stream.
        /// </summary>
        /// <remarks>
        /// In order to be CLS compliant the return type of this method has been changed to
        /// the nearest CLS compliant data type. Implementations must only read a
        /// <see cref="System.UInt16"/> from the data stream.
        /// </remarks>
        /// <returns>A <see cref="System.UInt16"/> read from the data stream</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        int ReadUInt16();

        /// <summary>
        /// Reads a <see cref="System.Int16"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="System.UInt16"/> read from the data stream</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        short ReadInt16();

        /// <summary>
        /// Reads a <see cref="System.Int32"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> read from the data stream</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        int ReadInt32();

        /// <summary>
        /// Reads a <see cref="System.UInt32"/> from the data stream.
        /// </summary>
        /// <remarks>
        /// In order to be CLS compliant the return type of this method has been changed to
        /// the nearest CLS compliant data type. Implementations must only read a
        /// <see cref="System.UInt32"/> from the data stream.
        /// </remarks>
        /// <returns>A <see cref="System.UInt32"/> read from the data stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        long ReadUInt32();

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsQueryClass"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsQueryClass"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsQueryClass ReadQueryClass();

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsQueryType"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsQueryType"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsQueryType ReadQueryType();

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsOpCode"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsOpCode"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsOpCode ReadOpCode();

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsRecordClass"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsRecordClass"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        DnsRecordClass ReadRecordClass();

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsRecordType"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsRecordType"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsRecordType ReadRecordType();

        /// <summary>
        /// Reads the remaining data and returns it as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The remaining data as a <see cref="System.String"/>.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        string ReadString();

        /// <summary>
        /// Reads the specified number of bytes from the underlying data source and
        /// returns it as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The specified number of bytes as an ASCII encoded
        /// <see cref="System.String"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is greater than the number of bytes
        /// remaining on the underlying data stream.
        /// </exception>
        string ReadString(int count);

        /// <summary>
        /// Reads a DNS &lt;character-string&gt; from the underlying data source.
        /// </summary>
        /// <returns>A &lt;character-string&gt; string from the underlying data
        /// source.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        string ReadCharString();

        /// <summary>
        /// Reads a TTL value from the underlying data source.
        /// </summary>
        /// <returns>A TTL value from the underlying data source.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        TimeSpan ReadTtl();

        /// <summary>
        /// Reads an IPv4 network address from the underlying data source.
        /// </summary>
        /// <returns>A <see cref="System.Net.IPAddress"/> from the underlying source
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        IPAddress ReadIPv4Address();

        /// <summary>
        /// Reads an IPv6 network address from the underlying data source.
        /// </summary>
        /// <returns>A <see cref="System.Net.IPAddress"/> from the underlying source
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        IPAddress ReadIPv6Address();

        /// <summary>
        /// Reads <see cref="AK.Net.Dns.DnsRecord"/> from the underlying data source.
        /// </summary>
        /// <returns>
        /// A <see cref="AK.Net.Dns.DnsRecord"/> from the underlying source stream.
        /// </returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        DnsRecord ReadRecord();
    }
}
