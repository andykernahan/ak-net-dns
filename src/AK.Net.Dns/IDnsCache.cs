﻿// Copyright 2008 Andy Kernahan
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

using AK.Net.Dns.Records;

namespace AK.Net.Dns
{
    /// <summary>
    /// Defines a mechanism for caching DNS replies.
    /// </summary>
    /// <remarks>
    /// Implementations <b>MUST</b> be thread safe.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public interface IDnsCache
    {
        /// <summary>
        /// Fetches a cached <see cref="AK.Net.Dns.DnsReply"/> to the specified
        /// <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The question.</param>        
        /// <returns>The <see cref="AK.Net.Dns.DnsCacheResult"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        DnsCacheResult Get(DnsQuestion question);

        /// <summary>
        /// Puts the records contained within the specified <paramref name="reply"/>
        /// into the cache.
        /// </summary>
        /// <param name="reply">The reply containing the records to cache.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>
        void Put(DnsReply reply);
    }
}
