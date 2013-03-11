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
    /// Represents a DNS reply message.
    /// </summary>
    [Serializable]
    public class DnsReply : DnsMessage
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsReply"/> class.
        /// </summary>
        public DnsReply() { }

        /// <summary>
        /// Reads the values of this <see cref="DnsReply"/> from the specified
        /// <see cref="AK.Net.Dns.IDnsReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the <see cref="DnsReply"/> could not be read from the 
        /// <paramref name="reader"/>.
        /// </exception>
        public override void Read(IDnsReader reader) {

            Guard.NotNull(reader, "reader");

            base.Read(reader);
            if(this.Header.IsQuery)
                throw Guard.DnsReplyExpected();
        }

        #endregion
    }
}
