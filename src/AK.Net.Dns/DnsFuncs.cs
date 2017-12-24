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

using System.Net;
using AK.Net.Dns.Records;

namespace AK.Net.Dns
{
    /// <summary>
    /// Provides a comprehensive set of unary predicates and functors. This
    /// class is <see langword="static"/>.
    /// </summary>
    public static class DnsFuncs
    {
        #region Private Impl.

        private static bool IsOfType(DnsRecord record, DnsRecordType type)
        {
            return record != null && record.Type == type;
        }

        #endregion

        #region Public Interface.

        #region Unary Predicates.

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.A"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.A"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsA(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.A);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.NS"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.NS"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsNS(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.NS);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.CName"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.CName"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsCName(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.CName);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Soa"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Soa"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsSoa(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Soa);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MB"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MB"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsMB(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.MB);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MG"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MG"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsMG(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.MG);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MR"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MR"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsMR(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.MR);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Null"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Null"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsNull(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Null);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Wks"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Wks"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsWks(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Wks);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Ptr"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Ptr"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsPtr(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Ptr);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.HInfo"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.HInfo"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsHInfo(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.HInfo);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MInfo"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MInfo"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsMInfo(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.MInfo);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MX"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.MX"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsMX(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.MX);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Txt"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Txt"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsTxt(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Txt);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.RP"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.RP"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsRP(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.RP);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.AsfDB"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.AsfDB"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsAsfDB(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.AsfDB);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.X25"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.X25"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsX25(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.X25);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Isdn"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Isdn"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsIsdn(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Isdn);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.RT"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.RT"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsRT(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.RT);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Aaaa"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Aaaa"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsAaaa(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Aaaa);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Loc"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Loc"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsLoc(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Loc);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Srv"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Srv"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsSrv(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Srv);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Spf"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.Spf"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsSpf(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.Spf);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.DN"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.DN"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsDN(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.DN);
        }

        /// <summary>
        /// Returns a value indicating if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.A"/> or
        /// <see cref="AK.Net.Dns.DnsRecordType.Aaaa"/>.
        /// </summary>
        /// <param name="record">The record to test.</param>
        /// <returns><see langword="true"/> if the record is of type
        /// <see cref="AK.Net.Dns.DnsRecordType.A"/> or
        /// <see cref="AK.Net.Dns.DnsRecordType.Aaaa"/>, otherwise;
        /// <see langword="false"/>.</returns>
        public static bool IsAOrAaaa(DnsRecord record)
        {
            return IsOfType(record, DnsRecordType.A) ||
                   IsOfType(record, DnsRecordType.Aaaa);
        }

        #endregion

        #region Binary Predicates.

        #endregion

        #region Converters.

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.ARecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.ARecord"/>.</returns>
        public static ARecord ToA(DnsRecord record)
        {
            return (ARecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.AaaaRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.AaaaRecord"/>.</returns>
        public static AaaaRecord ToAaaa(DnsRecord record)
        {
            return (AaaaRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.CNameRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.CNameRecord"/>.</returns>
        public static CNameRecord ToCName(DnsRecord record)
        {
            return (CNameRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.HInfoRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.HInfoRecord"/>.</returns>
        public static HInfoRecord ToHInfo(DnsRecord record)
        {
            return (HInfoRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.MInfoRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.MInfoRecord"/>.</returns>
        public static MInfoRecord ToMInfo(DnsRecord record)
        {
            return (MInfoRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.MXRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.MXRecord"/>.</returns>
        public static MXRecord ToMX(DnsRecord record)
        {
            return (MXRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.NSRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.NSRecord"/>.</returns>
        public static NSRecord ToNS(DnsRecord record)
        {
            return (NSRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.DNRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.DNRecord"/>.</returns>
        public static DNRecord ToDN(DnsRecord record)
        {
            return (DNRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.SpfRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.SpfRecord"/>.</returns>
        public static SpfRecord ToSpf(DnsRecord record)
        {
            return (SpfRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.NullRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.NullRecord"/>.</returns>
        public static NullRecord ToNull(DnsRecord record)
        {
            return (NullRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.PtrRecord"/>.
        /// </summary>
        /// <param name="record">The record to wrap.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.PtrRecord"/>.</returns>
        public static PtrRecord ToPtr(DnsRecord record)
        {
            return (PtrRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.SoaRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.SoaRecord"/>.</returns>
        public static SoaRecord ToSoa(DnsRecord record)
        {
            return (SoaRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.TxtRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.TxtRecord"/>.</returns>
        public static TxtRecord ToTxt(DnsRecord record)
        {
            return (TxtRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.WksRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.WksRecord"/>.</returns>
        public static WksRecord ToWks(DnsRecord record)
        {
            return (WksRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.SrvRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.SrvRecord"/>.</returns>
        public static SrvRecord ToSrv(DnsRecord record)
        {
            return (SrvRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.MBRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.MBRecord"/>.</returns>
        public static MBRecord ToMB(DnsRecord record)
        {
            return (MBRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.MGRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.MGRecord"/>.</returns>
        public static MGRecord ToMG(DnsRecord record)
        {
            return (MGRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.MRRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.MRRecord"/>.</returns>
        public static MRRecord ToMR(DnsRecord record)
        {
            return (MRRecord)record;
        }

        /// <summary>
        /// Casts the specified <paramref name="record"/> to type
        /// <see cref="AK.Net.Dns.Records.XRecord"/>.
        /// </summary>
        /// <param name="record">The record to cast.</param>
        /// <returns>The specified <paramref name="record"/> cast to type
        /// <see cref="AK.Net.Dns.Records.XRecord"/>.</returns>
        public static XRecord ToX(DnsRecord record)
        {
            return (XRecord)record;
        }

        /// <summary>
        /// Extracts the <see cref="System.Net.IPAddress"/> from a record of type
        /// <see cref="AK.Net.Dns.DnsRecordType.A"/> or 
        /// <see cref="AK.Net.Dns.DnsRecordType.Aaaa"/>.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>The extracted <see cref="System.Net.IPAddress"/>.</returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the <paramref name="record"/> cannot be converted to an IP address.
        /// </exception>
        public static IPAddress ToIP(DnsRecord record)
        {
            if (record == null)
            {
                return null;
            }

            switch (record.Type)
            {
                case DnsRecordType.A:
                    return ToA(record).Address;
                case DnsRecordType.Aaaa:
                    return ToAaaa(record).Address;
                default:
                    throw Guard.DnsRecordMustBeAorAaaa("record");
            }
        }

        #endregion

        #endregion
    }
}
