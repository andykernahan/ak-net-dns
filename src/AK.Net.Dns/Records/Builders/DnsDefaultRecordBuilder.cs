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

namespace AK.Net.Dns.Records.Builders
{
    /// <summary>
    /// Provides a default <see cref="AK.Net.Dns.IDnsRecordBuilder"/> implementation
    /// which always builds <see cref="AK.Net.Dns.Records.XRecord"/> instances.
    /// This class cannot be inherited.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class DnsDefaultRecordBuilder : IDnsRecordBuilder
    {
        #region Private Impl.

        private DnsDefaultRecordBuilder()
        {
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Provides a default instance. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly DnsDefaultRecordBuilder Instance =
            new DnsDefaultRecordBuilder();

        /// <summary>
        /// This method always returns <see langword="true"/>.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <returns>
        /// <see langword="true"/> if the bulider can build records of the
        /// specified <paramref name="type"/>, otherwise; <see langword="false"/>.
        /// </returns>
        public bool CanBuild(DnsRecordType type)
        {
            return true;
        }

        /// <summary>
        /// Builds a record of the specified <paramref name="type"/> using the
        /// specified owner name, resource record class, TTL and the reply reader
        /// from which the RDLENGTH and RDATA section of the resource record will
        /// be read.
        /// </summary>
        /// <param name="type">The type of record to build.</param>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="reader">The reply reader.</param>
        /// <returns>The built <see cref="AK.Net.Dns.DnsRecord"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="reader"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when the builder is not capable of building records of the
        /// specified <paramref name="type"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the RDATA section of the record could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public DnsRecord Build(DnsName owner, DnsRecordType type, DnsRecordClass cls,
            TimeSpan ttl, IDnsReader reader)
        {
            return new XRecord(owner, type, cls, ttl, reader);
        }

        #endregion
    }
}
