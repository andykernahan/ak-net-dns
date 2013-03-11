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
    /// Defines the different types of class of resource records.
    /// </summary>
    [Serializable]
    public enum DnsRecordClass
    {
        /// <summary>
        /// The Internet system.
        /// </summary>
        IN = 1,
        /// <summary>
        /// The Chaos system.
        /// </summary>
        CH = 3,
        /// <summary>
        /// The Hesiod systems.
        /// </summary>
        HS = 4
    }
}
