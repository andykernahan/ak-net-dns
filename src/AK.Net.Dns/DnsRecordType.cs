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
    /// Defines the different types of resouce records.
    /// </summary>
    [Serializable]
    public enum DnsRecordType
    {
        /// <summary>
        /// A resource record which contains a host's IPv4 Internet Protocol address.
        /// </summary>
        A = 1,
        /// <summary>
        /// A recourd record which identifies the authoritative name server for the domain.
        /// </summary>
        NS = 2,
        /// <summary>
        /// A resource record which identifies the canonical name of an alias.
        /// </summary>
        CName = 5,
        /// <summary>
        /// A resource record which identifies the start of a zone of authority.
        /// </summary>
        Soa = 6,
        /// <summary>
        /// A resource record which specifies the host which the queried mailbox.
        /// </summary>
        MB = 7,
        /// <summary>
        /// A resource record which specifies a mailbox which is a member of the mail group
        /// specified by the domain name.
        /// </summary>
        MG = 8,
        /// <summary>
        /// A resource record which specifies a mailbox which is the proper rename of the specified
        /// mailbox.
        /// </summary>
        MR = 9,
        /// <summary>
        /// A resource record which contains binary data.
        /// </summary>
        Null = 10,
        /// <summary>
        /// A resource record which provides a Well Known Service description.
        /// </summary>
        Wks = 11,
        /// <summary>
        /// A resource record which identifies a pointer to another part of the domain name space.
        /// </summary>
        Ptr = 12,
        /// <summary>
        /// A resource record which identifies the CPU and OS used by a host.
        /// </summary>
        HInfo = 13,
        /// <summary>
        /// A resource record which contains mailbox or mail list information.
        /// </summary>
        MInfo = 14,
        /// <summary>
        /// A resource record which identifies a mail exchange for the domain.
        /// </summary>
        MX = 15,
        /// <summary>
        /// A resource record which contains text string.
        /// </summary>
        Txt = 16,
        /// <summary>
        /// A resource record which identifies the person responsible for a host.
        /// </summary>
        RP = 17,
        /// <summary>
        /// A resource record which defines the location of an ASF database.
        /// </summary>
        AsfDB = 18,
        /// <summary>
        /// A resource record which identifies the PSDN address in the X.121 numbering plan
        /// associated with a name.
        /// </summary>
        X25 = 19,
        /// <summary>
        /// A resource record which identities then ISDN number and subaddress associated with a name.
        /// </summary>
        Isdn = 20,
        /// <summary>
        /// A resource record which provides a route-through binding for hosts that do not have their
        /// own direct wide area network addresses
        /// </summary>
        RT = 21,
        /// <summary>
        /// A resource record which contains a host's IPv6 Internet Protocol address.
        /// </summary>
        Aaaa = 28,
        /// <summary>
        /// A resource record which express location information.
        /// </summary>
        Loc = 29,
        /// <summary>
        /// A resource record which specifies the location of a service.
        /// </summary>
        Srv = 33,
        /// <summary>
        /// A resource record which maps multiple DNS nodes to another domain.
        /// </summary>
        DN = 39,
        /// <summary>
        /// A resource record which contains the Send Policy Framework (SPF)
        /// specification string for the owner.
        /// </summary>
        Spf = 99
    }
}
