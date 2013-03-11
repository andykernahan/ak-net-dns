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
using System.Runtime.Serialization;

namespace AK.Net.Dns
{
    /// <summary>
    /// Defines the base class for all DNS related exceptions thrown by this
    /// library.
    /// </summary>
    [Serializable]
    public class DnsException : Exception
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the DnsException class.
        /// </summary>
        public DnsException()
            : base() {
        }

        /// <summary>
        /// Initialises a new instance of the DnsException class and specifies a
        /// message describing the error.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public DnsException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initialises a new instance of the DnsException class and specifies a
        /// message describing the error and an inner exception which is the cause
        /// of this exception.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">The inner exception which is the cause of
        /// this exception.</param>
        public DnsException(string message, Exception innerException)
            : base(message, innerException) {
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// De-serialization constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected DnsException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion
    }
}
