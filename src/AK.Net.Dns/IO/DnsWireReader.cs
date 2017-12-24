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

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// Reads DNS related types read from an underlying data stream according
    /// to RFC1035.
    /// </summary>
    [Serializable]
    public class DnsWireReader : Disposable, IDnsReader
    {
        #region Private Fields.

        private int _pos;
        private readonly int _offset;
        private readonly int _length;
        private readonly byte[] _buffer;

        private static readonly bool s_isLittleEndian = BitConverter.IsLittleEndian;

        /// <summary>
        /// Defines the maximim recursion depth whilst uncompressing a single
        /// qname label. This field is constant.
        /// </summary>
        private const int MaxRecurDepth = 30;

        /// <summary>
        /// Label type mask. This field is constant.
        /// </summary>
        private const int LabelTypeMask = 0xC0;

        /// <summary>
        /// Label length mask. This field is constant.
        /// </summary>
        private const int LabelLenMask = 0x3F;

        /// <summary>
        /// Compress label type. This field is constant.
        /// </summary>
        private const int LabelTypeComp = 0xC0;

        /// <summary>
        /// Normal label type. This field is constant.
        /// </summary>
        private const int LabelTypeNorm = 0;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsWireReader"/> class
        /// and specifies the data buffer.
        /// </summary>
        /// <param name="buffer">The data buffer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        public DnsWireReader(byte[] buffer)
        {
            Guard.NotNull(buffer, "buffer");

            _buffer = buffer;
            _pos = 0;
            _offset = 0;
            _length = _buffer.Length;
        }

        /// <summary>
        /// Initialises a new instance of the DnsWireReader class and specifies the
        /// data buffer the offset in <paramref name="buffer"/> at which decoding
        /// begins and the number of bytes available.
        /// </summary>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which
        /// decoding begins.</param>
        /// <param name="count">The number of bytes available within
        /// <paramref name="buffer"/>.</param>
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
        public DnsWireReader(byte[] buffer, int offset, int count)
        {
            Guard.IsValidBufferArgs(buffer, offset, count);

            _buffer = buffer;
            _pos = offset;
            _offset = _pos;
            _length = offset + count;
        }

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
        public DnsName ReadName()
        {
            CheckDisposed();
            AssertRemaining(1);

            return ReadName(_buffer, _pos, out _pos);
        }

        /// <summary>
        /// Reads <see cref="AK.Net.Dns.DnsRecord"/> from the underlying data stream.
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
        public DnsRecord ReadRecord()
        {
            var owner = ReadName();
            var type = ReadRecordType();
            var cls = ReadRecordClass();
            var ttl = ReadTtl();

            return DnsRecord.GetBuilder(type).Build(owner, type, cls, ttl, this);
        }

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
        public byte ReadByte()
        {
            CheckDisposed();
            AssertRemaining(1);

            return _buffer[_pos++];
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying data stream.
        /// </summary>
        /// <param name="count">The number of bytes to read from the data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="count"/> is negative
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
        public byte[] ReadBytes(int count)
        {
            Guard.InRange(count >= 0 && count <= Remaining, "count");

            CheckDisposed();

            var buf = new byte[count];

            ReadBytes(buf, 0, count);

            return buf;
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying data stream.
        /// </summary>
        /// <param name="buffer">The buffer into which the data is written.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which
        /// writing beings.</param>
        /// <param name="count">The number of bytes to read from the data stream.</param>
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
        public void ReadBytes(byte[] buffer, int offset, int count)
        {
            Guard.IsValidBufferArgs(buffer, offset, count);

            CheckDisposed();
            Buffer.BlockCopy(_buffer, _pos, buffer, offset, count);
            _pos += count;
        }

        /// <summary>
        /// Reads a <see cref="System.UInt16"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="System.UInt16"/> read from the data stream</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public int ReadUInt16()
        {
            CheckDisposed();
            AssertRemaining(2);

            if (s_isLittleEndian)
            {
                return (ushort)((_buffer[_pos++] << 8) | _buffer[_pos++]);
            }

            return (ushort)(_buffer[_pos++] | (_buffer[_pos++] << 8));
        }

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
        public short ReadInt16()
        {
            CheckDisposed();
            AssertRemaining(2);

            if (s_isLittleEndian)
            {
                return (short)((_buffer[_pos++] << 8) | _buffer[_pos++]);
            }

            return (short)(_buffer[_pos++] | (_buffer[_pos++] << 8));
        }

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
        public int ReadInt32()
        {
            CheckDisposed();
            AssertRemaining(4);

            if (s_isLittleEndian)
            {
                return (_buffer[_pos++] << 24) | (_buffer[_pos++] << 16) | (_buffer[_pos++] << 8) | _buffer[_pos++];
            }

            return _buffer[_pos++] | (_buffer[_pos++] << 8) | (_buffer[_pos++] << 16) | (_buffer[_pos++] << 24);
        }

        /// <summary>
        /// Reads a <see cref="System.UInt32"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="System.UInt32"/> read from the data stream</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public long ReadUInt32()
        {
            CheckDisposed();
            AssertRemaining(4);

            if (s_isLittleEndian)
            {
                return (uint)((_buffer[_pos++] << 24) | (_buffer[_pos++] << 16) | (_buffer[_pos++] << 8) | _buffer[_pos++]);
            }

            return (uint)(_buffer[_pos++] | (_buffer[_pos++] << 8) | (_buffer[_pos++] << 16) | (_buffer[_pos++] << 24));
        }

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
        public DnsQueryClass ReadQueryClass()
        {
            return (DnsQueryClass)ReadUInt16();
        }

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
        public DnsQueryType ReadQueryType()
        {
            return (DnsQueryType)ReadUInt16();
        }

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
        public DnsOpCode ReadOpCode()
        {
            return (DnsOpCode)ReadUInt16();
        }

        /// <summary>
        /// Reads a <see cref="AK.Net.Dns.DnsRecordClass"/> from the data stream.
        /// </summary>
        /// <returns>A <see cref="AK.Net.Dns.DnsRecordClass"/> read from the data
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public DnsRecordClass ReadRecordClass()
        {
            return (DnsRecordClass)ReadUInt16();
        }

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
        public DnsRecordType ReadRecordType()
        {
            return (DnsRecordType)ReadUInt16();
        }

        /// <summary>
        /// Reads the remaining buffer and returns it as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The remaining buffer as a <see cref="System.String"/>.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public string ReadString()
        {
            return ReadString(Remaining);
        }

        /// <summary>
        /// Reads the specified number of bytes from the underlying data stream and
        /// returns it as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The specified number of bytes as an ASCII encoded
        /// <see cref="System.String"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="count"/> is negative
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
        public string ReadString(int count)
        {
            Guard.InRange(count >= 0 && count <= Remaining, "count");

            CheckDisposed();

            if (count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(count);

            while (count-- > 0)
            {
                sb.Append((char)_buffer[_pos++]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Reads a DNS &lt;character-string&gt; from the underlying data stream.
        /// </summary>
        /// <returns>A &lt;character-string&gt; string from the underlying buffer
        /// source.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public string ReadCharString()
        {
            return ReadString(ReadByte());
        }

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
        public TimeSpan ReadTtl()
        {
            return ConvertTtl(ReadUInt32());
        }

        /// <summary>
        /// Reads an IPv4 network address from the underlying data stream.
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
        public IPAddress ReadIPv4Address()
        {
            return ReadIPAddress(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Reads an IPv6 network address from the underlying data stream.
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
        public IPAddress ReadIPv6Address()
        {
            return ReadIPAddress(AddressFamily.InterNetworkV6);
        }

        /// <summary>
        /// Reads a <see cref="System.Net.IPAddress"/> from the data stream.
        /// </summary>
        /// <param name="family">The network address family.</param>
        /// <returns>A <see cref="System.Net.IPAddress"/> from the underlying source
        /// stream.</returns>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the number of bytes remaining on the underlying data stream
        /// is not sufficient to satify the requirments of the operation.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public IPAddress ReadIPAddress(AddressFamily family)
        {
            byte[] buf;

            switch (family)
            {
                case AddressFamily.InterNetwork:
                    buf = new byte[DnsWireWriter.IPv4AddrLength];
                    break;
                case AddressFamily.InterNetworkV6:
                    buf = new byte[DnsWireWriter.IPv6AddrLength];
                    break;
                default:
                    throw Guard.MustBeAnIPv4OrIPv6Addr("family");
            }
            ReadBytes(buf, 0, buf.Length);
            return new IPAddress(buf);
        }

        /// <summary>
        /// Converts the specified TTL value into it's <see cref="System.TimeSpan"/>
        /// representation.
        /// </summary>
        /// <param name="ttl">The TTL value.</param>
        /// <returns>The <see cref="System.TimeSpan"/> representation of the specified
        /// TTL.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="ttl"/> greater than the maximum allowable TTL value
        /// (<see cref="System.UInt32.MaxValue"/>).
        /// </exception>
        public static TimeSpan ConvertTtl(long ttl)
        {
            Guard.InRange(ttl >= 0 && ttl <= uint.MaxValue, "ttl");

            return (ttl & 0x80000000) == 0 ? new TimeSpan(ttl * TimeSpan.TicksPerSecond) : TimeSpan.Zero;
        }

        #endregion

        #region Private Impl.

        private void AssertRemaining(int count)
        {
            if (Remaining < count)
            {
                throw Guard.EndOfDnsStreamReached();
            }
        }

        private int Remaining => _length - _pos;

        private static DnsName ReadName(byte[] buffer, int offset, out int next)
        {
            var sb = new StringBuilder();

            ReadName(sb, buffer, offset, 0, out next);

            return DnsName.Parse(sb.ToString());
        }

        private static void ReadName(StringBuilder sb, byte[] buffer, int offset, int depth, out int next)
        {
            if (depth > MaxRecurDepth)
            {
                throw Guard.DnsNameHasTooManyRefs(MaxRecurDepth);
            }

            var pos = offset;

            while (pos < buffer.Length)
            {
                var type = buffer[pos] & LabelTypeMask;
                if (type == LabelTypeComp)
                {
                    ReadName(sb, buffer, GetPtrOffset(buffer, pos, out pos), depth + 1, out _);
                    break;
                }
                if (type == LabelTypeNorm)
                {
                    var len = buffer[pos++] & LabelLenMask;
                    if (len != 0)
                    {
                        var end = len + pos;
                        if (end > buffer.Length)
                        {
                            throw Guard.EndOfDnsStreamReached();
                        }
                        if (sb.Length > 0)
                        {
                            sb.Append('.');
                        }
                        do
                        {
                            sb.Append((char)buffer[pos++]);
                        } while (pos < end);
                    }
                    else
                    {
                        sb.Append('.');
                        break;
                    }
                }
                else
                {
                    throw Guard.UnsupportedDnsNameLabelType(type);
                }
            }

            next = pos;
        }

        private static int GetPtrOffset(byte[] buffer, int offset, out int next)
        {
            var pos = offset;
            var b0 = buffer[pos++];

            if (pos < buffer.Length)
            {
                var labelOffset = ((b0 & LabelLenMask) << 8) | buffer[pos++];

                if (labelOffset < buffer.Length)
                {
                    next = pos;
                    return labelOffset;
                }
            }
            throw Guard.EndOfDnsStreamReached();
        }

        #endregion
    }
}
