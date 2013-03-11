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

namespace AK.Net.Dns
{
    /// <summary>
    /// Represents the header of a DNS message.
    /// </summary>
    [Serializable]
    public class DnsHeader
    {
        #region Header Format.

        /*
         *                                  1  1  1  1  1  1
              0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      ID                       |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    QDCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    ANCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    NSCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    ARCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         */

        #endregion

        #region Private Fields.

        private int _id;
        private int _questionCount;
        private int _answerCount;
        private int _authorityCount;
        private int _additionalCount;
        private bool _isQuery;
        private bool _isAuthorative;
        private bool _isTruncated;
        private bool _isRecursionDesired;
        private bool _isRecursionAllowed;
        private DnsOpCode _opCode = DnsOpCode.Query;
        private DnsResponseCode _responseCode = DnsResponseCode.NoError;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the maximum number of records in a section. This field is constant.
        /// </summary>
        public const int MaxSectionCount = ushort.MaxValue;

        /// <summary>
        /// Defines the maximum Id value. This field is constant.
        /// </summary>
        public const int MaxId = ushort.MaxValue;

        /// <summary>
        /// Parses the values of this header from the specified query reader.
        /// </summary>
        /// <param name="reader">The quey reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the header could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public virtual void Read(IDnsReader reader) {

            Guard.NotNull(reader, "reader");

            try {
                this.Id = reader.ReadUInt16();

                byte b3 = reader.ReadByte();

                this.IsQuery = (b3 & 0x80) == 0x00;
                this.OpCode = (DnsOpCode)(b3 >> 3 & 0x0F);
                this.IsAuthorative = (b3 & 0x04) == 0x04;
                this.IsTruncated = (b3 & 0x02) == 0x02;
                this.IsRecursionDesired = (b3 & 0x01) == 0x01;

                byte b4 = reader.ReadByte();

                this.IsRecursionAllowed = (b4 & 0x80) == 0x80;
                this.ResponseCode = (DnsResponseCode)(b4 & 0x0F);

                this.QuestionCount = reader.ReadUInt16();
                this.AnswerCount = reader.ReadUInt16();
                this.AuthorityCount = reader.ReadUInt16();
                this.AdditionalCount = reader.ReadUInt16();
            } catch(DnsException exc) {
                throw Guard.InvalidDnsHeaderFormat(exc);
            }
        }

        /// <summary>
        /// Formats and writes this header to the specified query writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Write(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            writer.WriteUInt16((ushort)this.Id);

            byte b3 = 0;

            if(!this.IsQuery)
                b3 |= 0x80;
            b3 |= (byte)(((byte)this.OpCode & 0x0F) << 3);
            if(this.IsAuthorative)
                b3 |= 0x04;
            if(this.IsTruncated)
                b3 |= 0x02;
            if(this.IsRecursionDesired)
                b3 |= 0x01;
            writer.WriteByte(b3);

            byte b4 = (byte)this.ResponseCode;

            if(this.IsRecursionAllowed)
                b4 |= 0x80;
            writer.WriteByte(b4);

            writer.WriteUInt16(this.QuestionCount);
            writer.WriteUInt16(this.AnswerCount);
            writer.WriteUInt16(this.AuthorityCount);
            writer.WriteUInt16(this.AdditionalCount);
        }

        /// <summary>
        /// Gets or sets the number of additional records.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="AK.Net.Dns.DnsHeader.MaxSectionCount"/>.
        /// </exception>
        public int AdditionalCount {

            get { return _additionalCount; }
            set {
                Guard.InRange(value >= 0 && value <= DnsHeader.MaxSectionCount, "value");
                _additionalCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of name server records.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="AK.Net.Dns.DnsHeader.MaxSectionCount"/>.
        /// </exception>
        public int AuthorityCount {

            get { return _authorityCount; }
            set {
                Guard.InRange(value >= 0 && value <= DnsHeader.MaxSectionCount, "value");
                _authorityCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of answer records.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="AK.Net.Dns.DnsHeader.MaxSectionCount"/>.
        /// </exception>
        public int AnswerCount {

            get {return _answerCount; }
            set {
                Guard.InRange(value >= 0 && value <= DnsHeader.MaxSectionCount, "value");
                _answerCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of questions records.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="AK.Net.Dns.DnsHeader.MaxSectionCount"/>.
        /// </exception>
        public int QuestionCount {

            get { return _questionCount; }
            set {
                Guard.InRange(value >= 0 && value <= DnsHeader.MaxSectionCount, "value");
                _questionCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public DnsResponseCode ResponseCode {

            get { return _responseCode; }
            set { _responseCode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if recursion is allowed.
        /// </summary>
        public bool IsRecursionAllowed {

            get { return _isRecursionAllowed; }
            set { _isRecursionAllowed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if recursion is desired.
        /// </summary>
        public bool IsRecursionDesired {

            get { return _isRecursionDesired; }
            set { _isRecursionDesired = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the message data is truncated.
        /// </summary>
        public bool IsTruncated {

            get { return _isTruncated; }
            set { _isTruncated = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the answer is authoratative.
        /// </summary>
        public bool IsAuthorative {

            get { return _isAuthorative; }
            set { _isAuthorative = value; }
        }

        /// <summary>
        /// Gers or sets the operation code.
        /// </summary>
        public DnsOpCode OpCode {

            get { return _opCode; }
            set { _opCode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the DNS message is a query.
        /// </summary>
        public bool IsQuery {

            get { return _isQuery; }
            set { _isQuery = value; }
        }

        /// <summary>
        /// Gets or sets the query identification.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="AK.Net.Dns.DnsHeader.MaxId"/>.
        /// </exception>
        public int Id {

            get { return _id; }
            set {
                Guard.InRange(value >= 0 && value <= DnsHeader.MaxId, "value");
                _id = value;
            }
        }

        #endregion
    }
}
