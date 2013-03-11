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

namespace AK.Net.Dns
{
    /// <summary>
    /// Defines a mechanism for querying the Domain Name System.
    /// </summary>
    /// <remarks>
    /// Implementations <b>MUST</b> be thread safe.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public interface IDnsResolver
    {
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
        DnsReply Resolve(DnsQuestion question);

        /// <summary>
        /// Begins an asynchronous operation to resolve the
        /// <see cref="AK.Net.Dns.DnsReply"/> instance for the specified
        /// <paramref name="question"/>.
        /// </summary>
        /// <param name="question">The <see cref="AK.Net.Dns.DnsQuestion"/> to
        /// resolve.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="question"/> or <paramref name="callback"/> is
        /// <see langword="null"/>.
        /// </exception>
        IAsyncResult BeginResolve(DnsQuestion question, AsyncCallback callback, object state);

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
        /// <see cref="M:AK.Net.Dns.IDnsResovler.EndResolve(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        DnsReply EndResolve(IAsyncResult iar);

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
        IPHostEntry GetHostEntry(string hostOrAddress);

        /// <summary>
        /// Resolves and returns an <see cref="System.Net.IPHostEntry"/> for the
        /// specified <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="address">A host's IP address.</param>
        /// <returns>
        /// An <see cref="System.Net.IPHostEntry"/> containing information
        /// associated with the host.
        /// </returns>
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
        IPHostEntry GetHostEntry(IPAddress address);

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
        IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback callback, object state);

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
        /// <see cref="M:AK.Net.Dns.IDnsResovler.EndGetHostEntry(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        IPHostEntry EndGetHostEntry(IAsyncResult iar);

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
        DnsName[] GetNameServers(DnsName domain);

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
        IAsyncResult BeginGetNameServers(DnsName domain, AsyncCallback callback, object state);

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
        DnsName[] EndGetNameServers(IAsyncResult iar);

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
        MXInfo GetMXInfo(DnsName domain);

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
        IAsyncResult BeginGetMXInfo(DnsName domain, AsyncCallback callback, object state);

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
        /// <see cref="M:AK.Net.Dns.IDnsResovler.EndGetMXInfo(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsResolutionException">
        /// Thrown when an error occurred during the resolution, such as the query
        /// not being answered.
        /// </exception>
        MXInfo EndGetMXInfo(IAsyncResult iar);

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.IDnsCache"/> responsible for
        /// caching query replies.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        IDnsCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.IDnsTransport"/> responsible for
        /// transfering queries.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        IDnsTransport Transport { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AK.Net.Dns.DnsName"/> suffix used to convert
        /// relative names to absolute names before they are queried.
        /// </summary>
        DnsName NameSuffix { get; set; }
    }
}

