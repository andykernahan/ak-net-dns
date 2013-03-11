// Copyright (C) 2008 Andy Kernahan
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
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace AK.Net.Dns
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Validation Helpers.

        internal static void NotNull<T>(T obj, string paramName) where T : class {

            if(obj == null) {
                throw new ArgumentNullException(paramName);
            }
        }

        internal static void NotEmpty(string s, string paramName) {

            Guard.NotNull(s, paramName);
            if(s.Length == 0) {
                throw new ArgumentException(Format(Messages.MustNotBeAnEmptyString, paramName), paramName);
            }
        }

        internal static void IsValidBufferArgs(byte[] buffer, int offset, int count) {

            Guard.NotNull(buffer, "buffer");
            Guard.InRange(offset >= 0 && offset < buffer.Length, "offset");
            Guard.InRange(count >= 0 && offset + count <= buffer.Length, "count");
        }

        internal static void IsIPv4(IPAddress addr, string paramName) {

            Guard.NotNull(addr, paramName);
            if(addr.AddressFamily != AddressFamily.InterNetwork) {
                throw new ArgumentException(Format(Messages.MustBeAnIPv4Addr, paramName), paramName);
            }
        }

        internal static void IsIPv6(IPAddress addr, string paramName) {

            Guard.NotNull(addr, paramName);
            if(addr.AddressFamily != AddressFamily.InterNetworkV6) {
                throw new ArgumentException(Format(Messages.MustBeAnIPv6Addr, paramName), paramName);
            }
        }

        internal static void InRange(bool condition, string paramName) {

            if(!condition) {
                throw ArgumentOutOfRange(paramName);
            }
        }

        internal static void IsUInt16(int value, string paramName) {

            Guard.InRange(value >= UInt16.MinValue && value <= UInt16.MaxValue, paramName);
        }

        internal static void IsUInt32(long value, string paramName) {

            Guard.InRange(value >= UInt32.MinValue && value <= UInt32.MaxValue, paramName);
        }

        internal static void Internal(string message) {

            message = Format(Messages.InternalError, message);

            Debug.Fail(message);
            Trace.Fail(message);

            throw new ApplicationException(message);
        }

        #endregion

        #region Exception Helpers.

        internal static ArgumentException CharStringTooLong(string paramName) {

            return new ArgumentException(Messages.CharStringTooLong, paramName);
        }

        internal static ArgumentException DnsRecordMustBeAorAaaa(string paramName) {

            return new ArgumentException(Messages.DnsRecordMustBeAorAaaa, paramName);
        }

        internal static ArgumentException DnsQuestionAlreadyAMember(string paramName) {

            return new ArgumentException(Messages.DnsQuestionAlreadyAMember, paramName);
        }

        internal static DnsFormatException EndOfDnsStreamReached() {

            return new DnsFormatException(Messages.EndOfDnsStreamReached);
        }

        internal static InvalidOperationException UnableToConcatToAbsoluteDnsName() {

            return new InvalidOperationException(Messages.UnableToConcatToAbsoluteDnsName);
        }

        internal static ArgumentException UnableToMakeRelativeNotAChildOfName(DnsName @this, DnsName name, string paramName) {

            return new ArgumentException(Format(Messages.UnableToMakeRelativeNotASubDomainOfName,
                @this, name), paramName);
        }

        internal static DnsFormatException InvalidDnsHeaderFormat(Exception inner) {

            return new DnsFormatException(Messages.InvalidDnsHeaderFormat, inner);
        }

        internal static DnsFormatException InvalidDnsMessageFormat(Exception inner) {

            return new DnsFormatException(Messages.InvalidDnsMessageFormat, inner);
        }

        internal static ArgumentException MustBeAnIPAddressOrDnsName(string paramName) {

            return new ArgumentException(Format(Messages.MustBeAnIPAddressOrDnsName, paramName), paramName);
        }

        internal static DnsResolutionException DnsResolverNegativeResponseCodeReceived(
            DnsResponseCode code, DnsQuestion question) {

            return new DnsResolutionException(Format(Messages.DnsResolverNegativeResponseCodeReceived,
                question, code), code);
        }

        internal static DnsFormatException DnsQueryExpected() {

            return new DnsFormatException(Messages.DnsQueryExpected);
        }

        internal static DnsFormatException DnsReplyExpected() {

            return new DnsFormatException(Messages.DnsReplyExpected);
        }

        internal static DnsFormatException InvalidDnsQuestionFormat(Exception inner) {

            return new DnsFormatException(Messages.InvalidDnsQuestionFormat, inner);
        }

        internal static DnsFormatException DnsNameHasTooManyRefs(int max) {

            return new DnsFormatException(Format(Messages.DnsNameHasTooManyRefs, max));
        }

        internal static DnsFormatException UnsupportedDnsNameLabelType(int type) {

            return new DnsFormatException(Format(Messages.UnsupportedDnsNameLabelType, type));
        }

        internal static DnsTransportException DnsTransportFailed(Exception inner) {

            return new DnsTransportException(Format(Messages.DnsTransportFailed, inner.Message), inner);
        }

        internal static DnsTransportException DnsTransportNoEndPointsReplied() {

            return new DnsTransportException(Messages.DnsTransportNoEndPointsReplied);
        }

        internal static DnsTransportException DnsTransportReceivedEmptyMessage() {

            return new DnsTransportException(Messages.DnsTransportReceivedEmptyMessage);
        }

        internal static DnsTransportException DnsTransportIncomingMessageToLarge(int size, int maximum) {

            return new DnsTransportException(Format(Messages.DnsTransportIncomingMessageToLarge, size, maximum));
        }

        internal static InvalidOperationException AsyncResultEndAlreadyCalled() {

            return new InvalidOperationException(Messages.AsyncResultEndAlreadyCalled);
        }

        internal static ArgumentException MustBeAnIPv4OrIPv6Addr(string paramName) {

            return new ArgumentException(Format(Messages.MustBeAnIPv4OrIPv6Addr, paramName), paramName);
        }

        internal static ArgumentException InvalidAsyncResult(string paramName) {

            return new ArgumentException(Format(Messages.InvalidAsyncResult, paramName), paramName);
        }

        internal static DnsFormatException MustBeAValidDnsName(string paramName, string value) {

            return new DnsFormatException(Format(Messages.MustBeAValidDnsName, value));
        }

        internal static NotSupportedException NotSupported() {

            return new NotSupportedException();
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName) {

            return new ArgumentOutOfRangeException(paramName);
        }

        internal static ObjectDisposedException ObjectDisposed(string name) {

            return new ObjectDisposedException(name);
        }

        internal static ArgumentException CollectionMustBeReadonly(string paramName) {

            return new ArgumentException(Messages.CollectionMustBeReadonly, paramName);
        }

        internal static ConfigurationErrorsException ConfigurationTypeIsNotAssignableFrom(
            Type baseType, Type actualType) {

            return new ConfigurationErrorsException(Format(Messages.ConfigurationTypeIsNotAssignableFrom,
                baseType, actualType != null ? actualType.FullName : "(null)"));
        }

        private static string Format(string format, params object[] args) {

            return string.Format(Messages.Culture, format, args);
        }

        #endregion
    }
}
