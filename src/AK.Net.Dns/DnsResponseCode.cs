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
    /// Defines the response codes to a DNS query.
    /// </summary>
    [Serializable]
    public enum DnsResponseCode
    {
        /// <summary>
        /// Indicates the query was a success.
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Indicates the name server was unable to interpret the query.
        /// </summary>
        FormatError = 1,
        /// <summary>
        /// Indicates the name server was unable to process this query due
        /// to a problem with the name server.
        /// </summary>
        ServerFailure = 2,
        /// <summary>
        /// Meaningful only for resources from an authoritative name server,
        /// this code signifies that the domain name referenced in the query
        /// does not exist.
        /// </summary>
        NameError = 3,
        /// <summary>
        /// Indicates the name server does not support the requested type of
        /// query.
        /// </summary>
        NotImplemented = 4,
        /// <summary>
        /// Indicates the name server refused to perform the specified operation
        /// for policy reasons.
        /// </summary>
        Refused = 5,
        /// <summary>
        /// Indicates that the query failed as the queried name exists when it
        /// should not.
        /// </summary>
        YxDomain = 6,
        /// <summary>
        /// Indicates that the query failed as the queried record type exists
        /// when it should not.
        /// </summary>
        YxRrSet = 7,
        /// <summary>
        /// Indicates that the query failed as the queried record type does not
        /// exists when it should.
        /// </summary>
        NxRrSet = 8,
        /// <summary>
        /// Indicates that the query failed as the requestor is not authorised.
        /// </summary>
        NotAuth = 9,
    }
}
