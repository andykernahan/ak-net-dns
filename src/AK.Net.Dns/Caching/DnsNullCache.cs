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

using AK.Net.Dns.Records;

namespace AK.Net.Dns.Caching
{
    /// <summary>
    /// Provides a <see cref="AK.Net.Dns.IDnsCache"/> implementation which
    /// does not cache any information. This class cannot be inherited.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class DnsNullCache : IDnsCache
    {
        #region Public Interface.

        /// <summary>
        /// This method always returns a failed result.
        /// </summary>
        /// <param name="question">The question.</param>        
        /// <returns>The <see cref="AK.Net.Dns.DnsCacheResult"/> if the operation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        public DnsCacheResult Get(DnsQuestion question) {

            Guard.NotNull(question, "question");

            return DnsCacheResult.Failed;
        }

        /// <summary>
        /// This method does not add any records to the cache.
        /// </summary>
        /// <param name="reply">The reply containing the records to cache.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>
        public void Put(DnsReply reply) {

            Guard.NotNull(reply, "reply");
        }

        #endregion
    }
}
