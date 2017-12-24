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
using System.Threading;
using log4net;

namespace AK.Net.Dns.Caching
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DnsCache2 : IDnsCache2
    {
        #region Private Fields.

        private long _hits;
        private long _misses;
        private long _questions;
        private ILog _log;
        private readonly Timer _statisticsTimer;

        private static readonly TimeSpan STAT_LOG_INTERVAL = TimeSpan.FromSeconds(5);

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DnsCache2"/> class.
        /// </summary>
        public DnsCache2()
        {
            CreatedOn = DnsClock.Now();
            CacheNodes = new Dictionary<DnsName, CacheNode>();
            _statisticsTimer = new Timer(o => LogStatistics(), null,
                STAT_LOG_INTERVAL, STAT_LOG_INTERVAL);
        }

        /// <summary>
        /// Fetches a cached <see cref="AK.Net.Dns.DnsReply"/> to the specified
        /// <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The question.</param>        
        /// <returns>The <see cref="AK.Net.Dns.DnsCacheResult"/> if the operation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        public DnsCacheResult2 Get(DnsQuestion question)
        {
            Guard.NotNull(question, "question");

            IncrementQuestions();
            if (_questions % 2 == 0)
            {
                IncrementHits();
            }
            else
            {
                IncrementMisses();
            }

            return DnsCacheResult2.Empty;
        }

        /// <summary>
        /// Puts the records contained within the specified <paramref name="reply"/>
        /// into the cache.
        /// </summary>
        /// <param name="reply">The reply containing the records to cache.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>
        public void Put(DnsReply reply)
        {
            Guard.NotNull(reply, "reply");
        }

        /// <summary>
        /// Gets the number of questions this cache has been asked.
        /// </summary>
        public long Questions => _questions;

        /// <summary>
        /// Gets the number of cache misses.
        /// </summary>
        public long Misses => _misses;

        /// <summary>
        /// Gets the number of cache hits.
        /// </summary>
        public long Hits => _hits;

        /// <summary>
        /// Gets the cache hit ratio.
        /// </summary>
        public double HitRatio
        {
            get
            {
                if (Questions == 0 || Hits == 0)
                {
                    return 0.0d;
                }
                return Hits / (double)Questions;
            }
        }

        /// <summary>
        /// Gets the time at which this instance was created.
        /// </summary>
        public DateTime CreatedOn { get; }

        #endregion

        #region Protected Interface.

        protected virtual CacheNode GetCacheNode(DnsQuestion question)
        {
            //CacheNode head;
            //CacheNode match = null;
            //DateTime now = DnsClock.Now();

            //IncrementQuestions();
            //lock(this.CacheNodes) {
            //    if(this.CacheNodes.TryGetValue(owner, out head)) {
            //        CacheNode next;
            //        CacheNode cur = head;
            //        // Descend the list and look for a node of the correct type.
            //        do {
            //            next = cur.Next;
            //            if(cur.IsAlive(now)) {
            //                if(cur.Type == type) {
            //                    Debug.Assert(match == null, "DnsCache2::GetCacheNode - duplicate node");
            //                    match = cur;
            //                    if(match != head) {
            //                        // Move to the head of the list.
            //                        match.Remove();
            //                        head.Previous = match;
            //                        match.Next = head;
            //                        this.CacheNodes[owner] = match;
            //                    }
            //                    // Even though we have found a match we still descend the list 
            //                    // looking for dead nodes.
            //                }
            //            } else
            //                cur.Remove();
            //        } while((cur = next) != null);
            //    }
            //}
            //if(match != null) {
            //    IncrementHits();
            //    return match;
            //} else {
            //    IncrementMisses();
            return null;
            //}
        }

        /// <summary>
        /// Increments the number of questions this cache has been asked.
        /// </summary>
        protected void IncrementQuestions()
        {
            Interlocked.Increment(ref _questions);
        }

        /// <summary>
        /// Increments the number of cache misses for this cache.
        /// </summary>
        protected void IncrementMisses()
        {
            Interlocked.Increment(ref _misses);
        }

        /// <summary>
        /// Increments the number of cache hits for this cache.
        /// </summary>
        protected void IncrementHits()
        {
            Interlocked.Increment(ref _hits);
        }

        /// <summary>
        /// Logs informative, periodic, statistics.
        /// </summary>
        public virtual void LogStatistics()
        {
            Log.InfoFormat("Id={0}, CreatedOn={1}, Questions={2}, Hits={3}, Misses={4}, HitRatio={5:0.00}",
                GetHashCode(), CreatedOn, Questions, Hits,
                Misses, HitRatio);
        }

        /// <summary>
        /// Gets the dictionary of cache nodes contained by this cache.
        /// </summary>
        protected Dictionary<DnsName, CacheNode> CacheNodes { get; }

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>
        protected ILog Log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

        #region CacheNode.

        /// <summary>
        /// Represents a DNS cache node. This class cannot be inherited.
        /// </summary>
        [Serializable]
        protected sealed class CacheNode
        {
            #region Private Fields.

            private readonly DateTime _expires;

            #endregion

            #region Public Interface.

            public CacheNode(DnsResponseCode responseCode, DnsRecordType recordType,
                ICollection<DnsRecord> records, CacheNodeSource source)
            {
                Guard.NotNull(records, "records");

                Records = records;
                Source = source;
            }

            /// <summary>
            /// Returns a value indicating if this node is alive.
            /// </summary>
            /// <param name="dateTime">The current date time.</param>
            /// <returns><se langword="true"/> if this node is alive, otherwise;
            /// <see langword="false"/>.</returns>
            public bool IsAlive(DateTime dateTime)
            {
                return false;
            }

            /// <summary>
            /// Gets the records contained by this node.
            /// </summary>
            public ICollection<DnsRecord> Records { get; }

            /// <summary>
            /// Gets the source of this node.
            /// </summary>
            public CacheNodeSource Source { get; }

            #endregion
        }

        [Serializable]
        protected enum CacheNodeSource
        {
            Authority,
            AnswerSection,
            AdditionalSection
        }

        #endregion

        #endregion
    }
}
