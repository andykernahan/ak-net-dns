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
using System.Runtime.InteropServices;

namespace AK.Net.Dns
{
    /// <summary>
    /// Contains the result of a <see cref="AK.Net.Dns.IDnsCache"/> operation.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct DnsCacheResult
    {
        #region Private Fields.

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines a failed cache result. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly DnsCacheResult Failed =
            new DnsCacheResult(DnsCacheResultType.Failed);

        /// <summary>
        /// Intitialises a new instance of the <see cref="DnsCacheResult"/> type.
        /// </summary>
        /// <param name="type">The cache result type.</param>
        public DnsCacheResult(DnsCacheResultType type)
        {
            Type = type;
            Reply = null;
        }

        /// <summary>
        /// Intitialises a new instance of the <see cref="DnsCacheResult"/> type.
        /// </summary>
        /// <param name="type">The cache result type.</param>
        /// <param name="reply">The reply returned by the operation.</param>
        public DnsCacheResult(DnsCacheResultType type, DnsReply reply)
            : this(type)
        {
            Reply = reply;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return string.Format("Type={0}, Reply={1}", Type, Reply);
        }

        /// <summary>
        /// Gets the type of this cache result.
        /// </summary>
        public DnsCacheResultType Type { get; }

        /// <summary>
        /// Gets the reply returned by the cache operation.
        /// </summary>
        public DnsReply Reply { get; }

        #endregion
    }
}
