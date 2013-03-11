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

namespace AK.Net.Dns.Records.Builders
{
    /// <summary>
    /// Provides a <see cref="AK.Net.Dns.IDnsRecordBuilder"/> implementation
    /// which builds the records that are natively supported by this library.
    /// This class cannot be inherited.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class DnsNativeRecordBuilder : IDnsRecordBuilder
    {
        #region Public Interface.

        /// <summary>
        /// Provides a default instance. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly DnsNativeRecordBuilder Instance =
            new DnsNativeRecordBuilder();

        /// <summary>
        /// Returns a value indicating if the bulider is capable of building
        /// records of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <returns>
        /// <see langword="true"/> if the bulider can build records of the
        /// specified <paramref name="type"/>, otherwise; <see langword="false"/>.
        /// </returns>
        public bool CanBuild(DnsRecordType type) {

            switch(type) {
                case DnsRecordType.A:
                case DnsRecordType.Aaaa:
                case DnsRecordType.NS:
                case DnsRecordType.CName:
                case DnsRecordType.Soa:
                case DnsRecordType.Null:
                case DnsRecordType.Wks:
                case DnsRecordType.Ptr:
                case DnsRecordType.HInfo:                
                case DnsRecordType.MB:
                case DnsRecordType.MG:                
                case DnsRecordType.MInfo:
                case DnsRecordType.MR:
                case DnsRecordType.MX:
                case DnsRecordType.Txt:
                case DnsRecordType.DN:
                case DnsRecordType.Spf:
                case DnsRecordType.Srv:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Builds a record of the specified <paramref name="type"/> using the
        /// specified owner name, resource record class, TTL and the reply reader
        /// from which the RDLENGTH and RDATA section of the resource record will
        /// be read.
        /// </summary>
        /// <param name="type">The type of record to build.</param>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="reader">The reply reader.</param>
        /// <returns>The built <see cref="AK.Net.Dns.DnsRecord"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="reader"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when the builder is not capable of building records of the
        /// specified <paramref name="type"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the RDATA section of the record could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public DnsRecord Build(DnsName owner, DnsRecordType type, DnsRecordClass cls,
            TimeSpan ttl, IDnsReader reader) {

            switch(type) {
                case DnsRecordType.A:
                    return new ARecord(owner, ttl, reader);
                case DnsRecordType.Aaaa:
                    return new AaaaRecord(owner, ttl, reader);
                case DnsRecordType.NS:
                    return new NSRecord(owner, cls, ttl, reader);
                case DnsRecordType.CName:
                    return new CNameRecord(owner, cls, ttl, reader);
                case DnsRecordType.Soa:
                    return new SoaRecord(owner, cls, ttl, reader);
                case DnsRecordType.Null:
                    return new NullRecord(owner, cls, ttl, reader);
                case DnsRecordType.Wks:
                    return new WksRecord(owner, ttl, reader);
                case DnsRecordType.Ptr:
                    return new PtrRecord(owner, cls, ttl, reader);
                case DnsRecordType.HInfo:
                    return new HInfoRecord(owner, cls, ttl, reader);
                case DnsRecordType.MB:
                    return new MBRecord(owner, cls, ttl, reader);
                case DnsRecordType.MG:
                    return new MGRecord(owner, cls, ttl, reader);
                case DnsRecordType.MInfo:
                    return new MInfoRecord(owner, cls, ttl, reader);
                case DnsRecordType.MR:
                    return new MRRecord(owner, cls, ttl, reader);
                case DnsRecordType.MX:
                    return new MXRecord(owner, cls, ttl, reader);
                case DnsRecordType.Txt:
                    return new TxtRecord(owner, cls, ttl, reader);
                case DnsRecordType.DN:
                    return new DNRecord(owner, cls, ttl, reader);
                case DnsRecordType.Spf:
                    return new SpfRecord(owner, cls, ttl, reader);
                case DnsRecordType.Srv:
                    return new SrvRecord(owner, cls, ttl, reader);
                default:
                    throw Guard.NotSupported();
            }
        }

        #endregion

        #region Private Impl.

        private DnsNativeRecordBuilder() { }

        #endregion
    }
}