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
using System.Linq;
using System.Net;
using AK.Net.Dns.Records;
using log4net;
using F = AK.Net.Dns.DnsFuncs;

namespace AK.Net.Dns.Resolvers
{
    /// <summary>
    /// Defines a base class for classes which provide functionality for querying
    /// the Domain Name System. This class is <see langword="abstract"/>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public abstract class DnsResolver : IDnsResolver
    {
        #region Private Fields.

        private ILog _log;
        private IDnsCache _cache;
        private IDnsTransport _transport;

        private static readonly Random QUERY_ID_GEN = new Random();

        #endregion

        #region Public Interface.

        /// <summary>
        /// Accepts responsibility for completely resolving the specified 
        /// <paramref name="question"/> and returning the reply.
        /// </summary>
        /// <param name="question">The <see cref="AK.Net.Dns.DnsQuestion"/> to
        /// resolve.</param>
        /// <returns>The <see cref="AK.Net.Dns.DnsReply"/> containing the answer to
        /// the question.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual DnsReply Resolve(DnsQuestion question)
        {
            Guard.NotNull(question, "question");

            DnsReply reply;
            var cacheResult = Cache.Get(question);

            if (cacheResult.Type == DnsCacheResultType.Failed)
            {
                reply = Resolve(CreateQuery(question));
                Cache.Put(reply);
            }
            else
            {
                reply = cacheResult.Reply;
            }

            AssertIsPositiveReply(reply);

            return reply;
        }

        /// <summary>
        /// Begins an asynchronous operation to resolve the <see cref="AK.Net.Dns.DnsReply"/>
        /// instance for the specified <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The <see cref="AK.Net.Dns.DnsQuestion"/> to
        /// resolve.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        public virtual IAsyncResult BeginResolve(DnsQuestion question, AsyncCallback callback, object state)
        {
            Guard.NotNull(question, "question");

            var asyncState = new ResolveAsyncState(callback, state);

            asyncState.QueueOperation(delegate
            {
                try
                {
                    asyncState.Result = Resolve(question);
                }
                catch (DnsException exc)
                {
                    asyncState.Exception = exc;
                }
                finally
                {
                    asyncState.OnComplete();
                }
            });

            return asyncState;
        }

        /// <summary>
        /// Ends an asynchronous operation to resolve the <see cref="AK.Net.Dns.DnsReply"/>
        /// instance for a question or query.
        /// </summary>
        /// <param name="iar">The asynchronous operation result.</param>
        /// <returns>The <see cref="AK.Net.Dns.DnsReply"/> containing the answer to
        /// the query.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="iar"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified async result does not belong to this operation.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when 
        /// <see cref="M:AK.Net.Dns.Resolvers.DnsResovler.EndResolve(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual DnsReply EndResolve(IAsyncResult iar)
        {
            Guard.NotNull(iar, "iar");

            var state = iar as ResolveAsyncState;

            if (state == null)
            {
                throw Guard.InvalidAsyncResult("iar");
            }

            state.OnEndCalled();

            return state.Result;
        }

        /// <summary>
        /// Resolves and returns an <see cref="System.Net.IPHostEntry"/> for the
        /// specified host name or <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="hostOrAddress">A host name or IP address.</param>
        /// <returns>
        /// An <see cref="System.Net.IPHostEntry"/> containing information
        /// associated with the host.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="hostOrAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="hostOrAddress"/> is not a valid DNS name
        /// or IP address.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual IPHostEntry GetHostEntry(string hostOrAddress)
        {
            Guard.NotEmpty(hostOrAddress, "hostOrAddress");

            DnsName qname;
            IPAddress address;

            if (IPAddress.TryParse(hostOrAddress, out address))
            {
                return GetHostEntry(address);
            }
            if (DnsName.TryParse(hostOrAddress, out qname))
            {
                DnsReply reply;

                qname = AppendNameSuffix(qname);
                reply = Resolve(new DnsQuestion(qname, DnsQueryType.A, DnsQueryClass.IN));
                if (reply.Answers.Count > 0)
                {
                    return new IPHostEntry
                    {
                        HostName = (from cn in reply.Answers
                                       where F.IsCName(cn) && cn.Owner.Equals(qname)
                                       select F.ToCName(cn).Canonical).FirstOrDefault() ?? qname,
                        AddressList = (from a in reply.Answers
                            where F.IsAOrAaaa(a)
                            select F.ToIP(a)).ToArray(),
                        Aliases = DnsUtility.EMPTY_STRING_ARRAY
                    };
                }
                return new IPHostEntry
                {
                    HostName = qname,
                    AddressList = DnsUtility.EMPTY_IP_ARRAY,
                    Aliases = DnsUtility.EMPTY_STRING_ARRAY
                };
            }

            throw Guard.MustBeAnIPAddressOrDnsName("hostOrAddress");
        }

        /// <summary>
        /// Resolves and returns an <see cref="System.Net.IPHostEntry"/> for the
        /// specified <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="address">The host's IP address.</param>
        /// <returns>An <see cref="System.Net.IPHostEntry"/> containing information
        /// associated with the host.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="address"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual IPHostEntry GetHostEntry(IPAddress address)
        {
            Guard.NotNull(address, "address");

            var qname = PtrRecord.MakeName(address);
            var reply = Resolve(new DnsQuestion(qname, DnsQueryType.Ptr, DnsQueryClass.IN));

            return new IPHostEntry
            {
                AddressList = new[] {address},
                Aliases = DnsUtility.EMPTY_STRING_ARRAY,
                HostName = (from ptr in reply.Answers
                               where F.IsPtr(ptr) && ptr.Owner.Equals(qname)
                               select F.ToPtr(ptr).Domain).FirstOrDefault() ?? null
            };
        }

        /// <summary>
        /// Begins an asynchronous operation to resolve the 
        /// <see cref="System.Net.IPHostEntry"/> for the specified
        /// <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="address">The host's IP address.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="address"/> is <see langword="null"/>.
        /// </exception>
        public virtual IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback callback, object state)
        {
            Guard.NotNull(address, "address");

            var asyncState = new GetHostEntryAsyncState(callback, state);

            asyncState.QueueOperation(delegate
            {
                try
                {
                    asyncState.Result = GetHostEntry(address);
                }
                catch (DnsException exc)
                {
                    asyncState.Exception = exc;
                }
                finally
                {
                    asyncState.OnComplete();
                }
            });

            return asyncState;
        }

        /// <summary>
        /// Ends an asynchronous operation to resolve the <see cref="System.Net.IPHostEntry"/>
        /// instance for a host or address.
        /// </summary>
        /// <param name="iar">The asynchronous operation result.</param>
        /// <returns>An <see cref="System.Net.IPHostEntry"/> containing information
        /// associated with the host.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="iar"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified async result does not belong to this operation.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when 
        /// <see cref="M:AK.Net.Dns.Resolvers.DnsResovler.EndGetHostEntry(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual IPHostEntry EndGetHostEntry(IAsyncResult iar)
        {
            Guard.NotNull(iar, "iar");

            var state = iar as GetHostEntryAsyncState;

            if (state == null)
            {
                throw Guard.InvalidAsyncResult("iar");
            }

            state.OnEndCalled();

            return state.Result;
        }

        /// <summary>
        /// Resolves and returns the <see cref="AK.Net.Dns.MXInfo"/> instance for the 
        /// specified <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <returns>The mail exchange information associated with the 
        /// <paramref name="domain"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual MXInfo GetMXInfo(DnsName domain)
        {
            var qname = AppendNameSuffix(domain);
            var reply = Resolve(new DnsQuestion(qname, DnsQueryType.MX, DnsQueryClass.IN));

            if (reply.Answers.Count > 0)
            {
                return new MXInfo(qname,
                    (from ans in reply.Answers
                        where F.IsMX(ans)
                        let mx = F.ToMX(ans)
                        orderby mx
                        select mx.Exchange).ToArray()
                );
            }

            return new MXInfo(domain);
        }

        /// <summary>
        /// Begins an asynchronous operation to resolve the <see cref="AK.Net.Dns.MXInfo"/>
        /// instance for the specified <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        public virtual IAsyncResult BeginGetMXInfo(DnsName domain, AsyncCallback callback, object state)
        {
            Guard.NotNull(domain, "domain");

            var asyncState = new GetMXInfoAsyncState(callback, state);

            asyncState.QueueOperation(delegate
            {
                try
                {
                    asyncState.Result = GetMXInfo(domain);
                }
                catch (DnsException exc)
                {
                    asyncState.Exception = exc;
                }
                finally
                {
                    asyncState.OnComplete();
                }
            });

            return asyncState;
        }

        /// <summary>
        /// Ends an asynchronous operation to resolve the <see cref="AK.Net.Dns.MXInfo"/>
        /// instance for a domain.
        /// </summary>
        /// <param name="iar">The asynchronous operation result.</param>
        /// <returns>The mail exchange information associated with the domain.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="iar"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified async result does not belong to this operation.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when 
        /// <see cref="M:AK.Net.Dns.Resolvers.DnsResovler.EndGetMXInfo(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual MXInfo EndGetMXInfo(IAsyncResult iar)
        {
            Guard.NotNull(iar, "iar");

            var state = iar as GetMXInfoAsyncState;

            if (state == null)
            {
                throw Guard.InvalidAsyncResult("iar");
            }

            state.OnEndCalled();

            return state.Result;
        }

        /// <summary>
        /// Resolves and returns a <see cref="AK.Net.Dns.DnsName"/> instance for
        /// each of the authoritative name servers for the specified 
        /// <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns>
        /// The list of <see cref="AK.Net.Dns.DnsName"/> instances for each of the
        /// authoritative name servers for the specified <paramref name="domain"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual DnsName[] GetNameServers(DnsName domain)
        {
            var qname = AppendNameSuffix(domain);
            var reply = Resolve(new DnsQuestion(qname, DnsQueryType.NS, DnsQueryClass.IN));

            return reply.Answers.Where(F.IsNS).Select(x => F.ToNS(x).Domain).ToArray();
        }

        /// <summary>
        /// Begins an asynchronous operation to resolve a <see cref="AK.Net.Dns.DnsName"/>
        /// instance for each of the authoritative name servers for the specified 
        /// <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="domain"/> is <see langword="null"/>.
        /// </exception>
        public virtual IAsyncResult BeginGetNameServers(DnsName domain, AsyncCallback callback, object state)
        {
            Guard.NotNull(domain, "domain");

            var asyncState = new GetNameServersAsyncState(callback, state);

            asyncState.QueueOperation(delegate
            {
                try
                {
                    asyncState.Result = GetNameServers(domain);
                }
                catch (DnsException exc)
                {
                    asyncState.Exception = exc;
                }
                finally
                {
                    asyncState.OnComplete();
                }
            });

            return asyncState;
        }

        /// <summary>
        /// Begins an asynchronous operation to resolve a
        /// <see cref="System.Net.IPHostEntry"/> for each of the authoritative name
        /// servers for a domain.
        /// </summary>
        /// <param name="iar">The asynchronous operation result.</param>
        /// <returns>
        /// The list of <see cref="AK.Net.Dns.DnsName"/> instances for each of the
        /// authoritative name servers for the specified <paramref name="domain"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="iar"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified async result does not belong to this operation.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when 
        /// <see cref="M:AK.Net.Dns.IDnsResovler.EndGetNameServers(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        public virtual DnsName[] EndGetNameServers(IAsyncResult iar)
        {
            Guard.NotNull(iar, "iar");

            var state = iar as GetNameServersAsyncState;

            if (state == null)
            {
                throw Guard.InvalidAsyncResult("iar");
            }

            state.OnEndCalled();

            return state.Result;
        }

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.IDnsCache"/> responsible for
        /// caching query replies.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public IDnsCache Cache
        {
            get => _cache;
            set
            {
                Guard.NotNull(value, "value");
                _cache = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.IDnsTransport"/> responsible for
        /// transfering queries.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public IDnsTransport Transport
        {
            get => _transport;
            set
            {
                Guard.NotNull(value, "value");
                _transport = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.DnsName"/> suffix used to convert
        /// relative names to absolute names before they are queried.
        /// </summary>
        public DnsName NameSuffix { get; set; }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; accepts responsibility for completely
        /// resolving the specified <paramref name="query"/> and returning the reply.
        /// </summary>
        /// <param name="query">The <see cref="AK.Net.Dns.DnsQuery"/> to resolve.</param>
        /// <returns>The <see cref="AK.Net.Dns.DnsReply"/> containing the answer to
        /// the query.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>        
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurs during the resolution, such as the query
        /// not being answered.
        /// </exception>
        protected abstract DnsReply Resolve(DnsQuery query);

        /// <summary>
        /// Appends the <see cref="AK.Net.Dns.Resolvers.DnsResolver.NameSuffix"/> to the
        /// specified <paramref name="name"/> if <paramref name="name"/> is not
        /// absolute and <see cref="AK.Net.Dns.Resolvers.DnsResolver.NameSuffix"/> is not
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="name">The name to append to.</param>
        /// <returns>A new name.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        protected DnsName AppendNameSuffix(DnsName name)
        {
            Guard.NotNull(name, "name");

            var suffix = NameSuffix;

            return suffix != null && name.Kind == DnsNameKind.Relative ? name.Concat(suffix) : name;
        }

        /// <summary>
        /// Creates a query containing the specified <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The DNS question.</param>
        /// <returns>A new <see cref="AK.Net.Dns.DnsQuery"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> is <see langword="null"/>.
        /// </exception>
        protected virtual DnsQuery CreateQuery(DnsQuestion question)
        {
            Guard.NotNull(question, "question");

            var query = new DnsQuery();

            query.Header.Id = GenerateQueryId();
            query.Header.IsRecursionDesired = true;
            query.Questions.Add(question);

            return query;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reply">The <see cref="AK.Net.Dns.DnsReply"/> to check.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reply"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when
        /// </exception>
        protected void AssertIsPositiveReply(DnsReply reply)
        {
            Guard.NotNull(reply, "reply");

            if (reply.Header.ResponseCode != DnsResponseCode.NoError)
            {
                throw Guard.DnsResolverNegativeResponseCodeReceived(
                    reply.Header.ResponseCode, reply.Questions.FirstOrDefault());
            }
        }

        /// <summary>
        /// Gets a query identifier.
        /// </summary>
        /// <returns>A query identifier.</returns>
        protected static int GenerateQueryId()
        {
            return QUERY_ID_GEN.Next(10, DnsHeader.MaxId);
        }

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

        #endregion

        #region Private Impl.

        private sealed class GetMXInfoAsyncState : DnsAsyncState<MXInfo>
        {
            public GetMXInfoAsyncState(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        private sealed class GetNameServersAsyncState : DnsAsyncState<DnsName[]>
        {
            public GetNameServersAsyncState(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        private sealed class ResolveAsyncState : DnsAsyncState<DnsReply>
        {
            public ResolveAsyncState(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        private sealed class GetHostEntryAsyncState : DnsAsyncState<IPHostEntry>
        {
            public GetHostEntryAsyncState(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        #endregion
    }
}
