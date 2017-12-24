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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Writes DNS related types to an underlying data stream according to RFC1035.
    /// </summary>
    [Serializable]
    public class DnsWireWriter : Disposable, IDnsWriter
    {
        #region Private Fields.

        private MemoryStream _buffer;
        private Dictionary<string, int> _references;

        /// <summary>
        /// The initial capacity of the buffer. This should be large enough to
        /// allow most queries to be encoded without the MemoryStream needing to
        /// re-allocate it's internal buffer.
        /// </summary>
        private const int InitBufCapacity = 64;

        private static readonly bool s_isLittleEndian = BitConverter.IsLittleEndian;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the maximum allowed length of a DNS &lt;character-string&gt;.
        /// This field is constant.
        /// </summary>
        public const int MaxCharStringLength = 256;

        /// <summary>
        /// Defines thenumber of bytes in an IPv4 network address. This field is constant.
        /// </summary>
        public const int IPv4AddrLength = 4;

        /// <summary>
        /// Defines the number of bytes in an IPv6 network address.  This field is constant.
        /// </summary>
        public const int IPv6AddrLength = 16;

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsWireWriter"/> class.
        /// </summary>
        public DnsWireWriter()
        {
            Buffer = new MemoryStream(InitBufCapacity);
            References = new Dictionary<string, int>(DnsName.LabelComparer);
        }

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
        public void WriteName(DnsName name)
        {
            WriteName(name, true);
        }

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
        public void WriteName(DnsName name, bool compress)
        {
            Guard.NotNull(name, "name");

            CheckDisposed();

            if (!compress)
            {
                // Do not cache references to names that should not be compressed.
                foreach (var label in name.Labels)
                {
                    WriteLabel(label);
                }
                Buffer.WriteByte(0);
            }
            else if (References.ContainsKey(name.Name))
            {
                WritePtr(References[name.Name]);
            }
            else
            {
                var terminate = true;
                var subDomain = name.Name;
                var labels = name.Labels;

                if (labels.Count > 0)
                {
                    SaveReference(subDomain);
                    WriteLabel(labels[0]);
                    if (labels.Count > 1)
                    {
                        subDomain = subDomain.Substring(labels[0].Length + 1);
                        for (var i = 1; i < labels.Count; ++i)
                        {
                            if (References.ContainsKey(subDomain))
                            {
                                WritePtr(References[subDomain]);
                                // Pointers aren't terminated.
                                terminate = false;
                                break;
                            }
                            SaveReference(subDomain);
                            WriteLabel(labels[i]);
                            if (i < labels.Count - 1)
                            {
                                subDomain = subDomain.Substring(labels[i].Length + 1);
                            }
                        }
                    }
                }
                if (terminate)
                {
                    Buffer.WriteByte(0);
                }
            }
        }

        /// <summary>
        /// Encodes the specified domain name.
        /// </summary>
        /// <param name="domain">The domain name to encode.</param>
        /// <returns>The encoded domain name.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        public static byte[] EncodeName(DnsName domain)
        {
            Guard.NotNull(domain, "domain");

            var pos = 0;
            var ascii = Encoding.ASCII;
            var buf = new byte[domain.Name.Length + 2];
            var labels = domain.Labels;

            for (var i = 0; i < labels.Count; ++i)
            {
                buf[pos++] = (byte)labels[i].Length;
                pos += ascii.GetBytes(labels[i], 0, labels[i].Length, buf, pos);
            }
            buf[pos] = 0;

            return buf;
        }

        /// <summary>
        /// Writes the <see cref="AK.Net.Dns.DnsRecord"/> to the underlying stream.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="record"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteRecord(DnsRecord record)
        {
            Guard.NotNull(record, "record");

            int rdlPos;
            int endPos;

            WriteName(record.Owner);
            WriteRecordType(record.Type);
            WriteRecordClass(record.Class);
            WriteTtl(record.Ttl);
            rdlPos = Position;
            // Reserve RDLENGTH.
            Position += sizeof(ushort);
            record.WriteData(this);
            endPos = Position;
            Position = rdlPos;
            WriteUInt16(endPos - rdlPos - sizeof(ushort));
            Position = endPos;
        }

        /// <summary>
        /// Write a byte to the underlying stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteByte(byte value)
        {
            CheckDisposed();

            Buffer.WriteByte(value);
        }

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
        public void WriteBytes(byte[] buffer)
        {
            Guard.NotNull(buffer, "buffer");

            CheckDisposed();

            Buffer.Write(buffer, 0, buffer.Length);
        }

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
        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            Buffer.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a <see cref="System.UInt16"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid <see cref="System.UInt16"/>
        /// value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteUInt16(int value)
        {
            Guard.IsUInt16(value, "value");

            CheckDisposed();

            if (s_isLittleEndian)
            {
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)value);
            }
            else
            {
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 8));
            }
        }

        /// <summary>
        /// Writes a <see cref="System.Int16"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteInt16(short value)
        {
            CheckDisposed();

            if (s_isLittleEndian)
            {
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)value);
            }
            else
            {
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 8));
            }
        }

        /// <summary>
        /// Writes a <see cref="System.Int32"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteInt32(int value)
        {
            CheckDisposed();

            if (s_isLittleEndian)
            {
                Buffer.WriteByte((byte)(value >> 24));
                Buffer.WriteByte((byte)(value >> 16));
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)value);
            }
            else
            {
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)(value >> 16));
                Buffer.WriteByte((byte)(value >> 24));
            }
        }

        /// <summary>
        /// Writes a <see cref="System.UInt32"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid <see cref="System.UInt32"/>
        /// value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteUInt32(long value)
        {
            Guard.IsUInt32(value, "value");

            CheckDisposed();

            if (s_isLittleEndian)
            {
                Buffer.WriteByte((byte)(value >> 24));
                Buffer.WriteByte((byte)(value >> 16));
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)value);
            }
            else
            {
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)(value >> 16));
                Buffer.WriteByte((byte)(value >> 24));
            }
        }

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsQueryClass"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteQueryClass(DnsQueryClass value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsQueryType"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteQueryType(DnsQueryType value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsOpCode"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteOpCode(DnsOpCode value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsRecordClass"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteRecordClass(DnsRecordClass value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes a <see cref="AK.Net.Dns.DnsRecordType"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteRecordType(DnsRecordType value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes a <see cref="System.Net.IPAddress"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteIPAddress(IPAddress value)
        {
            Guard.NotNull(value, "value");

            WriteBytes(value.GetAddressBytes());
        }

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
        public void WriteTtl(TimeSpan value)
        {
            Guard.InRange(value >= TimeSpan.Zero, "value");

            WriteUInt32((uint)value.TotalSeconds);
        }

        /// <summary>
        /// Writes a <see cref="System.String"/> to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteString(string value)
        {
            Guard.NotNull(value, "value");

            for (var i = 0; i < value.Length; ++i)
            {
                Buffer.WriteByte((byte)value[i]);
            }
        }

        /// <summary>
        /// Writes a DNS &lt;character-string&gt; to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the length of <paramref name="value"/> has exceeded the maximim
        /// &lt;character-string&gt; length (256).
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public void WriteCharString(string value)
        {
            Guard.NotNull(value, "value");
            if (value.Length > MaxCharStringLength - 1)
            {
                throw Guard.CharStringTooLong("value");
            }

            Buffer.WriteByte((byte)value.Length);
            WriteString(value);
        }

        /// <summary>
        /// Returns the underlying data buffer.
        /// </summary>
        /// <returns>The data buffer.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        public ArraySegment<byte> GetBuffer()
        {
            CheckDisposed();

            var buf = new ArraySegment<byte>(Buffer.GetBuffer(),
                0, (int)Buffer.Length);

            Debug.WriteLine(string.Format("DnsWireWriter::GetBuffer - length={0}, initialCapacity={1}",
                buf.Count, InitBufCapacity));

            return buf;
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if being called explicitly,
        /// otherwise; <see langword="false"/> to indicate being called implicitly by
        /// the GC.</param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                References.Clear();
                References = null;
                ((IDisposable)Buffer).Dispose();
                Buffer = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the underlying stream.
        /// </summary>
        protected MemoryStream Buffer
        {
            get => _buffer;
            set => _buffer = value;
        }

        /// <summary>
        /// Gets or sets the position of the writer.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Throw when <paramref name="value"/> is greater than the length of the
        /// underlying data stream.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the writer has been disposed of.
        /// </exception>
        protected int Position
        {
            get
            {
                CheckDisposed();
                return (int)Buffer.Position;
            }
            set
            {
                CheckDisposed();
                Buffer.Position = value;
            }
        }

        #endregion

        #region Private Impl.

        private void SaveReference(string domain)
        {
            const int minLen = 3;

            if (domain.Length >= minLen && !References.ContainsKey(domain))
            {
                References.Add(domain, Position);
            }
        }

        private void WritePtr(int pos)
        {
            Debug.Assert((pos & 0xC000) == 0);
            WriteUInt16((ushort)(0xC0 | ((pos & 0x3F00) >> 8) | (pos & 0xFF)));
        }

        private void WriteLabel(string label)
        {
            Debug.Assert(label.Length <= DnsName.MaxLabelLength);
            WriteCharString(label);
        }

        private Dictionary<string, int> References
        {
            get => _references;
            set => _references = value;
        }

        #endregion
    }
}
