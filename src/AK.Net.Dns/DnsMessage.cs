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

using AK.Net.Dns.Records;

namespace AK.Net.Dns
{
    /// <summary>
    /// Represents a DNS message.
    /// </summary>
    [Serializable]
    public class DnsMessage
    {
        #region Private Fields.

        private DnsHeader _header;
        private DnsQuestionCollection _questions;
        private DnsRecordCollection _answers;
        private DnsRecordCollection _authorities;
        private DnsRecordCollection _additionals;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsMessage"/> class.
        /// </summary>
        public DnsMessage() {
            
            _header = new DnsHeader();            
        }

        /// <summary>
        /// Reads the values of this <see cref="DnsMessage"/> from the specified
        /// <see cref="AK.Net.Dns.IDnsReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the <see cref="DnsMessage"/> could not be read from the 
        /// <paramref name="reader"/>.
        /// </exception>
        public virtual void Read(IDnsReader reader) {

            Guard.NotNull(reader, "reader");

            PreRead();
            try {                
                this.Header.Read(reader);
                for(int i = 0, len = this.Header.QuestionCount; i < len; ++i)
                    this.Questions.Add(new DnsQuestion(reader));
                for(int i = 0, len = this.Header.AnswerCount; i < len; ++i)
                    this.Answers.Add(reader.ReadRecord());
                for(int i = 0, len = this.Header.AuthorityCount; i < len; ++i)
                    this.Authorities.Add(reader.ReadRecord());
                for(int i = 0, len = this.Header.AdditionalCount; i < len; ++i)
                    this.Additionals.Add(reader.ReadRecord());
            } catch(DnsException exc) {
                throw Guard.InvalidDnsMessageFormat(exc);
            }
        }        

        /// <summary>
        /// Writes the values of this <see cref="DnsMessage"/> using the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Write(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            PreWrite();
            this.Header.Write(writer);
            Write(_questions, writer);
            Write(_answers, writer);
            Write(_authorities, writer);
            Write(_additionals, writer);
        }

        /// <summary>
        /// Gets the <see cref="AK.Net.Dns.DnsHeader"/> for this
        /// <see cref="DnsMessage"/>.
        /// </summary>
        public DnsHeader Header {

            get { return _header; }
        }

        /// <summary>
        /// Gets the collection of questions.
        /// </summary>
        public DnsQuestionCollection Questions {

            get {
                if(_questions == null)
                    _questions = new DnsQuestionCollection();
                return _questions;
            }
        }

        /// <summary>
        /// Gets the collection of answers records.
        /// </summary>
        public DnsRecordCollection Answers {

            get {
                if(_answers == null)
                    _answers = new DnsRecordCollection();
                return _answers;
            }
        }

        /// <summary>
        /// Gets the collection of authority records.
        /// </summary>
        public DnsRecordCollection Authorities {

            get {
                if(_authorities == null)
                    _authorities = new DnsRecordCollection();
                return _authorities;
            }
        }

        /// <summary>
        /// Gets the collection of additional records.
        /// </summary>
        public DnsRecordCollection Additionals {

            get {
                if(_additionals == null)
                    _additionals = new DnsRecordCollection();
                return _additionals;
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Performs any operations required before this <see cref="DnsMessage"/>
        /// is written.
        /// </summary>
        protected virtual void PreWrite() {

            this.Header.QuestionCount = Count(_questions);
            this.Header.AnswerCount = Count(_answers);
            this.Header.AuthorityCount = Count(_authorities);
            this.Header.AdditionalCount = Count(_additionals);
        }

        /// <summary>
        /// Performs any operations required before this <see cref="DnsMessage"/>
        /// is read.
        /// </summary>
        protected virtual void PreRead() {

            Clear(_questions);
            Clear(_answers);
            Clear(_authorities);
            Clear(_additionals);
        }

        #endregion

        #region Private Impl.

        private static void Clear<T>(ICollection<T> collection) {

            if(collection != null)
                collection.Clear();
        }

        private static int Count<T>(ICollection<T> collection) {

            return collection != null ? collection.Count : 0;
        }

        private static void Write(DnsQuestionCollection collection, IDnsWriter writer) {

            if(collection != null)
                collection.Write(writer);
        }

        private static void Write(DnsRecordCollection collection, IDnsWriter writer) {

            if(collection != null)
                collection.Write(writer);
        }

        #endregion
    }
}
