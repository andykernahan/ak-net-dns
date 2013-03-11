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
using System.Net.Sockets;

namespace AK.Net.Dns.IO
{
    /// <summary>
    /// An <see langword="abstract"/> implementation of 
    /// <see cref="AK.Net.Dns.IDnsTransport"/> which provides a default
    /// asynchronous operation implementation. This class is 
    /// <see langword="abstract"/>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public abstract class DnsTransport : IDnsTransport
    {
        #region Private Fields.

        private log4net.ILog _log;
        private int _sendTimeout = DnsTransport.DefaultTimeout;
        private int _receiveTimeout = DnsTransport.DefaultTimeout;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the default timeout, in milliseconds, for both send and
        /// receive operations. This field is constant.
        /// </summary>
        public const int DefaultTimeout = 10 * 1000;

        /// <summary>
        /// Defines the default port for DNS transport end point. This field is
        /// constant.
        /// </summary>
        public const int DnsPort = 53;

        /// <summary>
        /// When overriden in a derived class; sends the specified 
        /// <see cref="AK.Net.Dns.DnsQuery"/> to the specified
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
        public abstract DnsReply Send(DnsQuery query, IPEndPoint endpoint);

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
        public virtual IAsyncResult BeginSend(DnsQuery query, IPEndPoint endpoint,
            AsyncCallback callback, object state) {

            Guard.NotNull(endpoint, "endpoint");
            Guard.NotNull(callback, "callback");

            SendAsyncState asyncState = new SendAsyncState(callback, state);

            asyncState.QueueOperation(delegate {
                try {
                    asyncState.Result = Send(query, endpoint);
                } catch(DnsTransportException exc) {
                    asyncState.Exception = exc;
                } finally {
                    asyncState.OnComplete();
                }
            });

            return asyncState;
        }

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
        public virtual DnsReply EndSend(IAsyncResult iar) {

            Guard.NotNull(iar, "iar");

            SendAsyncState state = iar as SendAsyncState;

            if(state == null)
                throw Guard.InvalidAsyncResult("iar");
            
            state.OnEndCalled();            

            return state.Result;
        }

        /// <summary>
        /// Gets or sets, in milliseconds, the transport send timeout.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a positive number.
        /// </exception>
        public int SendTimeout {

            get { return _sendTimeout; }
            set {
                Guard.InRange(value > 0, "value");
                _sendTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets, in milliseconds, the transport receive timeout.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a positive number.
        /// </exception>
        public int ReceiveTimeout {

            get { return _receiveTimeout; }
            set {
                Guard.InRange(value > 0, "value");
                _receiveTimeout = value;
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Returns a value indicating if <paramref name="received"/> and
        /// <paramref name="sent"/> queries are considered equal. This method
        /// determines equality based only on the query Id and questions.
        /// </summary>
        /// <param name="received">The query that was received during the
        /// operation.</param>
        /// <param name="sent">The query that was sent during the operation.</param>
        /// <returns><see langword="true"/> if the queries match, otherwise;
        /// <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sent"/> or <paramref name="received"/> is
        /// <see langword="null"/>.
        /// </exception>
        protected virtual bool QueriesAreEqual(DnsMessage received, DnsMessage sent) {

            Guard.NotNull(received, "received");
            Guard.NotNull(sent, "send");
            
            if(sent.Header.Id != received.Header.Id) {
                this.Log.Warn("received reply with mismatching id");
                return false;
            }

            if(!DnsQuestionCollection.Equals(sent.Questions, received.Questions)) {
                this.Log.Warn("received reply with mismatching question section");                    
                return false;
            }

            return true;
        }

        /// <summary>
        /// Encodes and returns the specified <see cref="AK.Net.Dns.DnsQuery"/>.
        /// </summary>
        /// <param name="query">The query to encode.</param>
        /// <returns>The encoded <see cref="AK.Net.Dns.DnsQuery"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="query"/> is <see langword="null"/>.
        /// </exception>
        protected ArraySegment<byte> WriteQuery(DnsQuery query) {

            Guard.NotNull(query, "query");

            using(DnsWireWriter writer = new DnsWireWriter()) {
                query.Write(writer);
                return writer.GetBuffer();
            }
        }

        /// <summary>
        /// Attemps to read a <see cref="AK.Net.Dns.DnsReply"/> from the specified
        /// <paramref name="buffer"/> and returns a value indicating the result of
        /// the operation.
        /// </summary>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="reply">On success, this argument will contain the
        /// read <see cref="AK.Net.Dns.DnsReply"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="AK.Net.Dns.DnsReply"/>
        /// was successfully read from the specified <paramref name="buffer"/>,
        /// otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        protected virtual bool TryReadReply(byte[] buffer, out DnsReply reply) {

            Guard.NotNull(buffer, "buffer");

            return TryReadReply(buffer, 0, buffer.Length, out reply);
        }

        /// <summary>
        /// Attemps to read a <see cref="AK.Net.Dns.DnsReply"/> from the specified
        /// <paramref name="buffer"/> and returns a value indicating the result of
        /// the operation.
        /// </summary>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which
        /// reading begins.</param>
        /// <param name="count">The number of bytes available within
        /// <paramref name="buffer"/>.</param>
        /// <param name="reply">On success, this argument will contain the
        /// read <see cref="AK.Net.Dns.DnsReply"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="AK.Net.Dns.DnsReply"/>
        /// was successfully read from the specified <paramref name="buffer"/>,
        /// otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="offset"/> is negative or greater than the length
        /// of <paramref name="buffer"/>
        /// </item>
        /// <item>
        /// <paramref name="count"/> is negative or greater than the length of
        /// <paramref name="buffer"/> given <paramref name="offset"/>.
        /// </item>
        /// </list>
        /// </exception>
        protected virtual bool TryReadReply(byte[] buffer, int offset, int count,
            out DnsReply reply) {

            reply = new DnsReply();

            try {
                using(DnsWireReader reader = new DnsWireReader(buffer, offset, count))
                    reply.Read(reader);
            } catch(DnsFormatException) {
                reply = null;                
            }

            return reply != null;
        }

        /// <summary>
        /// Configures the specified <see cref="System.Net.Sockets.Socket"/>.
        /// </summary>
        /// <param name="socket">The socket to configure</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="socket"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void ConfigureSocket(Socket socket) {

            Guard.NotNull(socket, "socket");

            socket.ReceiveTimeout = this.ReceiveTimeout;
            socket.SendTimeout = this.SendTimeout;
            socket.Blocking = true;
        }

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>
        protected log4net.ILog Log {

            get {
                if(_log == null)
                    _log = log4net.LogManager.GetLogger(GetType());
                return _log;
            }
        }

        #endregion

        #region Private Impl.

        private sealed class SendAsyncState : DnsAsyncState<DnsReply>
        {
            public SendAsyncState(AsyncCallback callback, object state)
                : base(callback, state) { }
        }

        #endregion
    }
}


