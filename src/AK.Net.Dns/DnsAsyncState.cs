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
using System.Threading;
using log4net;

namespace AK.Net.Dns
{
    /// <summary>
    /// An <see cref="System.IAsyncResult"/> implementation.
    /// </summary>
    /// <typeparam name="TResult">The operation result
    /// <see cref="System.Type"/>.</typeparam>
    internal class DnsAsyncState<TResult> : IAsyncResult
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DnsAsyncState&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="callback">The user's callback method.</param>
        /// <param name="state">The user's state object.</param>
        public DnsAsyncState(AsyncCallback callback, object state)
        {
            _callback = callback;
            AsyncState = state;
        }

        /// <summary>
        /// Gets a value indicating if the End* method has been called.
        /// </summary>
        public bool IsEndCalled => _isEndCalled;

        /// <summary>
        /// Gets or sets the exception that ocurred during the operation.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the result of the operation.
        /// </summary>
        public TResult Result { get; set; }

        #region Protected Interface.

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

        /// <summary>
        /// Gets the user's asynchronous operation state object.
        /// </summary>
        public object AsyncState { get; }

        /// <summary>
        /// Gets the <see cref="System.Threading.WaitHandle"/> that can be used
        /// to wait until the asynchronous operation completes.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_waitHandle == null)
                {
                    lock (_syncRoot)
                    {
                        if (_waitHandle == null)
                        {
                            // Set the initial state of the handle to be the completed
                            // state of the operation to ensure we do not lock the caller
                            // if the operation was completed whilst aquiring the lock.
                            // This prevents the following race condition:
                            // Thread A -> calls SetComplete which locks _syncRoot
                            // Thread B -> calls get_AsyncWaitHandle which waits on _syncRoot
                            // Thread A -> sets _isComplete (but does not signal _waitHandle
                            //             as it is null) and releases the lock on _syncRoot
                            // Thread B -> creates _waitHandle in an un-signaled state which
                            //             is then returned to B which then waits on it;
                            //             deadlock.
                            _waitHandle = new ManualResetEvent(_isCompleted);
                        }
                    }
                }
                return _waitHandle;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the operation completed synchronously.
        /// </summary>
        public bool CompletedSynchronously
        {
            get => _completedSynchronously;
            set => _completedSynchronously = value;
        }

        /// <summary>
        /// Gets a value indicating if the operation has completed.
        /// </summary>
        public bool IsCompleted => _isCompleted;

        /// <summary>
        /// Attemps to queue the specified operation on the 
        /// <see cref="System.Threading.ThreadPool"/> and returns a value indicating
        /// whether the operation has been queued to execute asynchronously or has been
        /// executed synchronously.
        /// </summary>
        /// <param name="callback">The method that represents the operation.</param>
        /// <returns><see langword="true"/> if the operation was queued to be executed
        /// asynchronously, otherwise; <see langword="false"/> to indicate that the
        /// method has been executed synchronously.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public bool QueueOperation(WaitCallback callback)
        {
            Guard.NotNull(callback, "callback");

            var queued = false;

            try
            {
                queued = ThreadPool.QueueUserWorkItem(callback);
            }
            catch (NotSupportedException exc)
            {
                // According to the documentation, a NotSupportedException is thrown when
                // the CLR is hosted and the host does not support the thread pool.
                Log.Error(exc);
            }
            catch (ApplicationException exc)
            {
                // Work item could not be queued.
                Log.Error(exc);
            }

            if (!queued)
            {
                Log.Warn("failed to queue an asynchronous operation on the thread " +
                         "pool, completing synchronously instead");
                CompletedSynchronously = true;
                callback(this);
            }

            return queued;
        }

        /// <summary>
        /// Waits for the operation to complete, asserts that end has not already been
        /// called and re-throws any exceptions caught during the operation.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when end has already been called on this result.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Any exception that was caught during the operation.
        /// </exception>
        public void OnEndCalled()
        {
            lock (_syncRoot)
            {
                if (!_isCompleted)
                {
                    Monitor.Wait(_syncRoot);
                }
                if (_isEndCalled)
                {
                    throw Guard.AsyncResultEndAlreadyCalled();
                }
                _isEndCalled = true;
                if (Exception != null)
                {
                    throw Exception;
                }
            }
        }

        /// <summary>
        /// Sets the operation as complete, signals the wait handle and invokes
        /// the user's callback.
        /// </summary>
        public void OnComplete()
        {
            lock (_syncRoot)
            {
                if (_isCompleted)
                {
                    return;
                }
                _isCompleted = true;
                if (_waitHandle != null)
                {
                    _waitHandle.Set();
                }
                // Release any threads waiting in OnEndCalled.
                Monitor.PulseAll(_syncRoot);
            }
            if (_callback != null)
            {
                _callback(this);
            }
        }

        #region Private Fields.

        private volatile bool _isCompleted;
        private volatile bool _isEndCalled;
        private volatile bool _completedSynchronously;
        private volatile ManualResetEvent _waitHandle;
        private ILog _log;
        private readonly AsyncCallback _callback;
        private readonly object _syncRoot = new object();

        #endregion
    }
}
