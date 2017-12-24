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
    /// The exception that is thrown when the resolution of a question fails.
    /// </summary>
    [Serializable]
    public class DnsResolutionException : DnsException
    {
        #region Private Fields.

        private const string CODE_PROP = "ResponseCode";

        #endregion

        #region Protected Interface.

        /// <summary>
        /// De-serialization constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="info"/> is <see langword="null"/>.
        /// </exception>
        protected DnsResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ResponseCode = (DnsResponseCode?)info.GetValue(CODE_PROP, typeof(DnsResponseCode?));
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsResolutionException"/> class.
        /// </summary>
        public DnsResolutionException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsResolutionException"/> class
        /// and specifies a message describing the error.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public DnsResolutionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsResolutionException"/> class
        /// and specifies a message describing the error and an inner exception which is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">The inner exception which is the cause of this
        /// exception.</param>
        public DnsResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsResolutionException"/> class
        /// and specifies a message describing the error and the
        /// <see cref="AK.Net.Dns.DnsResponseCode"/> that has caused this exception to
        /// be thrown.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="responseCode">The response code.</param>
        public DnsResolutionException(string message, DnsResponseCode responseCode)
            : base(message)
        {
            ResponseCode = responseCode;
        }

        /// <summary>
        /// Populates the specified <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// with the information needed to serialise this instance.
        /// </summary>
        /// <param name="info">The serialisation information container.</param>
        /// <param name="context">The context of the serialisation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="info"/> is <see langword="null"/>.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(CODE_PROP, ResponseCode, typeof(DnsResponseCode?));
        }

        /// <summary>
        /// Gets the <see cref="AK.Net.Dns.DnsResponseCode"/> that caused this exception to
        /// be thrown.
        /// </summary>
        public DnsResponseCode? ResponseCode { get; }

        #endregion
    }
}
