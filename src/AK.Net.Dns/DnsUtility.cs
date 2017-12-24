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
using System.Globalization;
using System.Net;

namespace AK.Net.Dns
{
    /// <summary>
    /// Utility class for the <see cref="AK.Net.Dns"/> namespace. This class
    /// is <see langword="static"/>.
    /// </summary>
    public static class DnsUtility
    {
        #region Public Interface.

        /// <summary>
        /// Defines the DNS culture. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly CultureInfo DnsCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Returns a formatted string using the <see cref="F:AK.Net.Dns.DnsRes.Culture"/>
        /// formatting rules.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The formatted <see cref="System.String"/>.</returns>
        public static string Format(string format, params object[] args)
        {
            return string.Format(DnsCulture, format, args);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <paramref name="System.TimeSpan"/>.
        /// </summary>
        /// <param name="value">The span.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <paramref name="System.TimeSpan"/>.</returns>
        public static string ToString(TimeSpan value)
        {
            return value.TotalSeconds.ToString(DnsCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsQueryType"/>.
        /// </summary>
        /// <param name="value">The type.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsQueryType"/>.</returns>
        public static string ToString(DnsQueryType value)
        {
            switch (value)
            {
                case DnsQueryType.A:
                    return "A";
                case DnsQueryType.NS:
                    return "NS";
                case DnsQueryType.CName:
                    return "CNAME";
                case DnsQueryType.Soa:
                    return "SOA";
                case DnsQueryType.MB:
                    return "MB";
                case DnsQueryType.MG:
                    return "MG";
                case DnsQueryType.MR:
                    return "MR";
                case DnsQueryType.Null:
                    return "NULL";
                case DnsQueryType.Wks:
                    return "WKS";
                case DnsQueryType.Ptr:
                    return "PTR";
                case DnsQueryType.HInfo:
                    return "HINFO";
                case DnsQueryType.MInfo:
                    return "MINFO";
                case DnsQueryType.MX:
                    return "MX";
                case DnsQueryType.Txt:
                    return "TXT";
                case DnsQueryType.RP:
                    return "RP";
                case DnsQueryType.AsfDB:
                    return "ASFDB";
                case DnsQueryType.X25:
                    return "X25";
                case DnsQueryType.Isdn:
                    return "ISDN";
                case DnsQueryType.RT:
                    return "RT";
                case DnsQueryType.Aaaa:
                    return "AAAA";
                case DnsQueryType.Loc:
                    return "LOC";
                case DnsQueryType.Srv:
                    return "SRV";
                case DnsQueryType.DN:
                    return "DN";
                case DnsQueryType.Spf:
                    return "SPF";
                case DnsQueryType.Axfr:
                    return "AXFR";
                case DnsQueryType.MailB:
                    return "MAILB";
                case DnsQueryType.All:
                    return "ALL";
                default:
#if DEBUG
                    throw Guard.ArgumentOutOfRange("value");
#else
                    return value.ToString().ToUpperInvariant();
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsQueryClass"/>.
        /// </summary>
        /// <param name="value">The class.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsQueryClass"/>.</returns>
        public static string ToString(DnsQueryClass value)
        {
            switch (value)
            {
                case DnsQueryClass.IN:
                    return "IN";
                case DnsQueryClass.CH:
                    return "CH";
                case DnsQueryClass.HS:
                    return "HS";
                case DnsQueryClass.Any:
                    return "ANY";
                default:
#if DEBUG
                    throw Guard.ArgumentOutOfRange("value");
#else
                    return value.ToString().ToUpperInvariant();
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsRecordType"/>.
        /// </summary>
        /// <param name="value">The type.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsRecordType"/>.</returns>
        public static string ToString(DnsRecordType value)
        {
            // All record types are valid query types.
            return ToString((DnsQueryType)value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsRecordClass"/>.
        /// </summary>
        /// <param name="value">The class.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsRecordClass"/>.</returns>
        public static string ToString(DnsRecordClass value)
        {
            switch (value)
            {
                case DnsRecordClass.IN:
                    return "IN";
                case DnsRecordClass.CH:
                    return "CH";
                case DnsRecordClass.HS:
                    return "HS";
                default:
#if DEBUG
                    throw Guard.ArgumentOutOfRange("value");
#else
                    return value.ToString().ToUpperInvariant();
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsOpCode"/>.
        /// </summary>
        /// <param name="value">The operation code.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <see cref="AK.Net.Dns.DnsOpCode"/>.</returns>
        public static string ToString(DnsOpCode value)
        {
            switch (value)
            {
                case DnsOpCode.Query:
                    return "QUERY";
                case DnsOpCode.IQuery:
                    return "IQUERY";
                case DnsOpCode.Status:
                    return "STATUS";
                case DnsOpCode.Notify:
                    return "NOTIFY";
                case DnsOpCode.Update:
                    return "UPDATE";
                default:
#if DEBUG
                    throw Guard.ArgumentOutOfRange("value");
#else
                    return value.ToString().ToUpperInvariant();
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the specified
        /// <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>A <see cref="System.String"/> representation of the specified
        /// <paramref name="buffer"/>.</returns>
        public static string ToString(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Empty array of <see cref="System.Net.IPAddress"/> instances. This
        /// field is <see langword="readonly"/>.
        /// </summary>
        internal static readonly IPAddress[] EMPTY_IP_ARRAY = { };

        /// <summary>
        /// Empty array of <see cref="System.Net.IPEndPoint"/> instances. This
        /// field is <see langword="readonly"/>.
        /// </summary>
        internal static readonly IPEndPoint[] EMPTY_EP_ARRAY = { };

        /// <summary>
        /// Empty array of <see cref="System.String"/>. This field is 
        /// <see langword="readonly"/>.
        /// </summary>
        internal static readonly string[] EMPTY_STRING_ARRAY = { };

        #endregion
    }
}
