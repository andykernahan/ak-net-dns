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
    /// Defines a writer which writes DNS types to an underlying data stream.
    /// </summary>
    public interface IDnsWriter : IDisposable
    {
        /// <summary>
        /// Writes the <see cref="AK.Net.Dns.DnsRecord"/> to the underlying stream.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="record"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteRecord(DnsRecord record);

        /// <summary>
        /// Writes the specified name to the underlying stream, compressing it
        /// if possible.
        /// </summary>
        /// <param name="name">The name to write.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteName(DnsName name);

        /// <summary>
        /// Writes the specified name to the underlying stream.
        /// </summary>
        /// <param name="name">The name to write.</param>        
        /// <param name="compress"><see langword="true"/> if the name can be 
        /// compressed, otherwise; <see langword="false"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteName(DnsName name, bool compress);

        /// <summary>
        /// Write a byte to the underlying stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream read from the
        /// specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteBytes(byte[] buffer);

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream read from the
        /// specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to write.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which
        /// reading begins.</param>
        /// <param name="count">The number of bytes to write.</param>
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
        /// </list>
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteBytes(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes a <see cref="System.UInt16"/> to the underlying stream.
        /// </summary>
        /// <remarks>
        /// In order to be CLS compliant the signature of this method has been changed to
        /// the nearest CLS compliant data type. Implementations must only write a
        /// <see cref="System.UInt16"/> to the data stream.
        /// </remarks>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid <see cref="System.UInt16"/>
        /// value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteUInt16(int value);

        /// <summary>
        /// Writes a <see cref="System.Int16"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteInt16(short value);

        /// <summary>
        /// Writes a <see cref="System.Int32"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteInt32(int value);

        /// <summary>
        /// Writes a <see cref="System.UInt32"/> to the underlying stream.
        /// </summary>
        /// <remarks>
        /// In order to be CLS compliant the signature of this method has been changed to
        /// the nearest CLS compliant data type. Implementations must only write a
        /// <see cref="System.UInt32"/> to the data stream.
        /// </remarks>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid <see cref="System.UInt32"/>
        /// value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteUInt32(long value);

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsQueryClass"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteQueryClass(DnsQueryClass value);

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsQueryType"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteQueryType(DnsQueryType value);

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsOpCode"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteOpCode(DnsOpCode value);

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsRecordClass"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteRecordClass(DnsRecordClass value);

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsRecordType"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteRecordType(DnsRecordType value);

        /// <summary>
        /// Writes a <see cref="System.Net.IPAddress"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteIPAddress(IPAddress value);

        /// <summary>
        /// Writes a TTL value to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteTtl(TimeSpan value);

        /// <summary>
        /// Writes a <see cref="System.String"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteString(string value);

        /// <summary>
        /// Writes a DNS &lt;character-string&gt; to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the length of <paramref name="value"/> has exceeded the maximim
        /// &lt;character-string&gt; length (256).
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        void WriteCharString(string value);
    }
}
