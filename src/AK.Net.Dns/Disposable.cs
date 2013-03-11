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
    /// Defines a base for a class which implements the Disposable pattern.
    /// This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Disposable : MarshalByRefObject, IDisposable
    {
        #region Private Fields.

        private bool _isDisposed;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets a value indicating if this instance has been disposed of.
        /// </summary>
        public bool IsDisposed {

            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if being called explicitly,
        /// otherwise; <see langword="false"/> to indicate being called implicitly by
        /// the GC.</param>
        protected virtual void Dispose(bool disposing) {

            if(!this.IsDisposed) {
                this.IsDisposed = true;
                // No point calling SuppressFinalize if were are being called from
                // the finalizer.
                if(disposing)
                    GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Helper method to throw a <see cref="System.ObjectDisposedException"/>
        /// if this instance has been disposed of.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed of.
        /// </exception>
        protected void CheckDisposed() {

            if(this.IsDisposed)
                throw Guard.ObjectDisposed(GetType().FullName);
        }

        #endregion

        #region Explicit Interface.

        void IDisposable.Dispose() {

            Dispose(true);
        }

        #endregion
    }
}
