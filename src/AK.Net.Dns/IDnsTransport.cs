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
    /// Defines a transport mechanism for DNS queries.
    /// </summary>
    /// <remarks>
    /// Implementations <b>MUST</b> (as a minimum) protect against message spoofing
    /// by:
    /// <list type="bullet">
    /// <item>
    /// ensuring the identity of the received message matches the sent message
    /// </item>
    /// <item>
    /// ensuring the question section of the received message matches the sent
    /// message
    /// </item>
    /// <item>
    /// ensuring the end point of the received message matches the end point of the
    /// sent message
    /// </item>
    /// </list>
    /// <para>Implementations <b>MUST</b> be thread safe.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public interface IDnsTransport
    {
        /// <summary>
        /// Sends the specified <see cref="AK.Net.Dns.DnsQuery"/> to the specified
        /// end point and return the <see cref="AK.Net.Dns.DnsReply"/>.
        /// </summary>        
        /// <param name="query">The query to send.</param>
        /// <param name="endpoint">The transport end point.</param>
        /// <returns>
        /// The <see cref="AK.Net.Dns.DnsReply"/> to the <see cref="AK.Net.Dns.DnsQuery"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="query"/> or <paramref name="endpoint"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurs.
        /// </exception>
        DnsReply Send(DnsQuery query, IPEndPoint endpoint);

        /// <summary>
        /// Begins an asynchronous operation to send the specified 
        /// <see cref="AK.Net.Dns.DnsQuery"/> to the specified end point
        /// and return the <see cref="AK.Net.Dns.DnsReply"/>.
        /// </summary>
        /// <param name="query">The query to send.</param>
        /// <param name="endpoint">The transport end point.</param>
        /// <param name="callback">The method to invoke once the asynchronous operation
        /// has completed.</param>
        /// <param name="state">The user-defined state object to associate with the
        /// asynchronous operation.</param>
        /// <returns>The asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="query"/>, <paramref name="endpoint"/> or
        /// <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        IAsyncResult BeginSend(DnsQuery query, IPEndPoint endpoint, AsyncCallback callback,
            object state);

        /// <summary>
        /// Ends an asynchronous operation to send the a <see cref="AK.Net.Dns.DnsQuery"/> 
        /// to an end point and return the <see cref="AK.Net.Dns.DnsReply"/>.
        /// </summary>
        /// <param name="iar">The asynchronous operation result.</param>
        /// <returns>
        /// The <see cref="AK.Net.Dns.DnsReply"/> to the <see cref="AK.Net.Dns.DnsQuery"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="iar"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified async result does not belong to this operation.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <see cref="AK.Net.Dns.IDnsTransport.EndSend(System.IAsyncResult)"/>
        /// has already been called on the specified async result.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsTransportException">
        /// Thrown when a transport error occurred during the asynchronous operation.
        /// </exception>
        DnsReply EndSend(IAsyncResult iar);
    }
}

