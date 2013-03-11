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

namespace AK.Net.Dns
{   
    /// <summary>
    /// Contains the result of a <see cref="AK.Net.Dns.IDnsCache"/> operation.
    /// This class is <see langword="sealed"/>.
    /// </summary>
    [Serializable]    
    public class DnsCacheResult2
    {
        #region Private Fields.

        private readonly bool _isEmpty;
        private readonly DnsResponseCode _responseCode;
        private readonly ICollection<DnsRecord> _records;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty <see cref="DnsCacheResult2"/>. This field is 
        /// <see langword="readonly"/>.
        /// </summary>
        public static readonly DnsCacheResult2 Empty = new DnsCacheResult2();

        /// <summary>
        /// Intitialises a new instance of the <see cref="DnsCacheResult2"/> type.
        /// </summary>
        /// <param name="responseCode">The cache response code.</param>
        public DnsCacheResult2(DnsResponseCode responseCode)
            : this(responseCode, DnsRecord.EmptyArray) { }

        /// <summary>
        /// Intitialises a new instance of the <see cref="DnsCacheResult2"/> type.
        /// </summary>
        /// <param name="responseCode">The cache response code.</param>
        /// <param name="records">The records returned by the operation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="records"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="records"/> is not readonly.
        /// </exception>
        public DnsCacheResult2(DnsResponseCode responseCode,
            ICollection<DnsRecord> records) {

            Guard.NotNull(records, "records");
            if(!records.IsReadOnly)
                throw Guard.CollectionMustBeReadonly("records");

            _responseCode = responseCode;
            _records = records;
            _isEmpty = false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return string.Format("IsEmpty={0}, ResponseCode={1}, Records={2}",
                this.IsEmpty, this.ResponseCode, this.Records.Count);
        }

        /// <summary>
        /// Gets the response code of this cache result.
        /// </summary>
        public DnsResponseCode ResponseCode {

            get { return _responseCode; }
        }

        /// <summary>
        /// Gets the records returned by the cache operation.
        /// </summary>
        public ICollection<DnsRecord> Records {

            get { return _records; }
        }

        /// <summary>
        /// Gets a value indicating if this cache result is empty.
        /// </summary>
        public bool IsEmpty {

            get { return _isEmpty; }
        }

        #endregion

        #region Private Impl.

        private DnsCacheResult2() {

            _isEmpty = true;
            _responseCode = DnsResponseCode.NoError;
            _records = DnsRecord.EmptyArray;
        }

        #endregion
    }
}
