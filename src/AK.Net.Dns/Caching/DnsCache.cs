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
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using AK.Net.Dns.Records;

namespace AK.Net.Dns.Caching
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DnsCache : IDnsCache
    {
        #region Private Fields.

        private long _hits;
        private long _misses;
        private long _questions;
        private readonly DateTime _createdOn;
        private readonly Dictionary<DnsName, Node> _nodes;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the DnsCache class.
        /// </summary>
        public DnsCache() {

            

            _createdOn = DnsClock.Now();
            _nodes = new Dictionary<DnsName, Node>();
        }

        /*/// <summary>
        /// This method always returns a failed result.
        /// </summary>
        /// <param name="owner">The domain to lookup.</param>
        /// <typeparam name="T">The type of record to lookup.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> is <see langword="null"/>.
        /// </exception>
        public DnsCacheResult<T> Get<T>(DnsName owner) where T : DnsRecord {

            if(owner == null)
                throw Error.ArgumentNull("owner");            

            Node node = GetNode(owner, typeof(T));

            return DnsCacheResult<T>.Failed;
        }*/

        /// <summary>
        /// Fetches a cached <see cref="AK.Net.Dns.DnsReply"/> to the specified
        /// <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The question.</param>        
        /// <returns>The <see cref="AK.Net.Dns.DnsCacheResult"/> if the operation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        public DnsCacheResult Get(DnsQuestion question) {

            Guard.NotNull(question, "question");

            IncrementQuestions();
            IncrementMisses();

            return DnsCacheResult.Failed;
        }

        /// <summary>
        /// This method does not add any records to the cache.
        /// </summary>
        /// <param name="reply">The reply containing the records to cache.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        public void Put(DnsReply reply) {

            Guard.NotNull(reply, "reply");
        }

        /// <summary>
        /// Gets the number of questions this cache has been asked.
        /// </summary>
        public long Questions {

            get { return _questions; }
        }

        /// <summary>
        /// Gets the number of cache misses.
        /// </summary>
        public long Misses {

            get { return _misses; }
        }

        /// <summary>
        /// Gets the number of cache hits.
        /// </summary>
        public long Hits {

            get { return _hits; }
        }

        /// <summary>
        /// Gets the cache hit ratio.
        /// </summary>
        public double HitRatio {

            get {
                if(this.Questions == 0)
                    return 0.0D;
                if(this.Misses == 0)
                    return 1.0D;
                return (double)this.Hits / (double)this.Misses;
            }
        }

        #endregion

        #region Protected Interface.

        protected virtual Node GetNode(DnsName owner, Type type) {

            Node head;
            Node match = null;
            DateTime now = DnsClock.Now();

            IncrementQuestions();
            lock(this.Nodes) {
                if(this.Nodes.TryGetValue(owner, out head)) {
                    Node next;
                    Node cur = head;
                    // Descend the list and look for a node of the correct type.
                    do {
                        next = cur.Next;
                        if(cur.IsAlive(now)) {
                            if(cur.Type == type) {
                                Debug.Assert(match == null, "DnsCache::GetNode - duplicate node");
                                match = cur;
                                if(match != head) {
                                    // Move to the head of the list.
                                    match.Remove();
                                    head.Previous = match;
                                    match.Next = head;
                                    this.Nodes[owner] = match;
                                }
                                // Even though we have found a match we still descend the list 
                                // looking for dead nodes.
                            }
                        } else
                            cur.Remove();
                    } while((cur = next) != null);
                }
            }
            if(match != null) {
                IncrementHits();
                return match;
            } else {
                IncrementMisses();
                return null;
            }
        }

        /// <summary>
        /// Increments the number of questions this cache has been asked.
        /// </summary>
        protected void IncrementQuestions() {

            Interlocked.Increment(ref _questions);
        }

        /// <summary>
        /// Increments the number of cache misses for this cache.
        /// </summary>
        protected void IncrementMisses() {

            Interlocked.Increment(ref _misses);
        }

        /// <summary>
        /// Increments the number of cache hits for this cache.
        /// </summary>
        protected void IncrementHits() {

            Interlocked.Increment(ref _hits);
        }

        /// <summary>
        /// Gets the dictionary of cache nodes contained by this cache.
        /// </summary>
        protected Dictionary<DnsName, Node> Nodes {

            get { return _nodes; }
        }

        #region CacheNode Def.

        /// <summary>
        /// Represents a Dns cache node. This class cannot be inherited.
        /// </summary>
        [Serializable]
        public sealed class Node
        {
            #region Private Fields.

            private Node _next;
            private Node _prev;
            private readonly Type _type;
            private readonly bool _isAuth;
            private readonly DnsName _owner;
            private readonly DnsRecord[] _records;

            #endregion

            #region Public Interface.

            /// <summary>
            /// Initialses a new instance of the Node class and specifies the records the node is 
            /// to contain and a value indicating if the records are from an authoratative source.
            /// </summary>
            /// <param name="records">The resource records.</param>
            /// <param name="isAuthoratative">True if the records are from an authoratative source, 
            /// otherwise; false.</param>
            public Node(DnsRecord[] records, bool isAuthoratative) {

                Guard.NotNull(records, "records");  

                _isAuth = isAuthoratative;
                _records = records;
                _owner = _records[0].Owner;   
                _type = _records[0].GetType();                             
            }

            /// <summary>
            /// Returns a value indicating if this node is alive.
            /// </summary>
            /// <param name="dt">The current date time.</param>
            /// <returns>True if this node is alive, otherwise; false.</returns>
            public bool IsAlive(DateTime dt) {

                for(int i = 0; i < this.Records.Length; ++i) {
                    if(!this.Records[i].IsAlive(dt))
                        return false;
                }

                return true;
            }

            /// <summary>
            /// Removes this node from the list.
            /// </summary>
            /// <returns></returns>
            public Node Remove() {

                Node prev = this.Previous;
                Node next = this.Next;

                if(prev != null)
                    prev.Next = next;
                if(next != null)
                    next.Previous = prev;

                this.Next = this.Previous = null;

                return prev ?? next;
            }

            /// <summary>
            /// Gets the owner of the records contained within this node.
            /// </summary>
            public DnsName Owner {

                get { return _owner; }
            }

            /// <summary>
            /// Gets the records contained by this node.
            /// </summary>
            public DnsRecord[] Records {

                get { return _records; }
            }

            /// <summary>
            /// Gets a value indicating if this node is from an authoratative source.
            /// </summary>
            public bool IsAuthorative {

                get { return _isAuth; }
            }

            /// <summary>
            /// Gets the type of record contained within this node.
            /// </summary>
            public Type Type {

                get { return _type; }
            }

            /// <summary>
            /// Gets or sets the previous node is this node list.
            /// </summary>
            public Node Previous {

                get { return _prev; }
                set { _prev = !object.ReferenceEquals(this, value) ? value : null; }
            }

            /// <summary>
            /// Gets or sets the next node is this node list.
            /// </summary>
            public Node Next {

                get { return _next; }
                set { _next = !object.ReferenceEquals(this, value) ? value : null; }
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
