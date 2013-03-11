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
    /// Defines the functionality required to build
    /// <see cref="AK.Net.Dns.DnsRecord"/> instances of a specific
    /// <see cref="AK.Net.Dns.DnsRecordType"/>.
    /// </summary>
    /// <remarks>
    /// Implementations <b>MUST</b> be thread safe.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public interface IDnsRecordBuilder
    {
        /// <summary>
        /// Returns a value indicating if the bulider is capable of building
        /// records of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <returns>
        /// <see langword="true"/> if the bulider can build records of the
        /// specified <paramref name="type"/>, otherwise; <see langword="false"/>.
        /// </returns>
        bool CanBuild(DnsRecordType type);

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
        /// <param name="reader">The reader.</param>
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
        DnsRecord Build(DnsName owner, DnsRecordType type, DnsRecordClass cls,
            TimeSpan ttl, IDnsReader reader);
    }
}

