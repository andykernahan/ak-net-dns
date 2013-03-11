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
    /// Defines the kind of a <see cref="AK.Net.Dns.DnsName"/>.
    /// </summary>
    [Serializable]
    public enum DnsNameKind
    {
        /// <summary>
        /// Indicates that the name is absolute.
        /// </summary>
        Absolute,
        /// <summary>
        /// Indicates that the name is relative to another.
        /// </summary>
        Relative
    }
}
