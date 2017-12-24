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
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace AK.Net.Dns.Configuration.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.TypeConverter"/> for
    /// <see cref="System.Net.IPAddress"/> instances. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class IPAddressConverter : TypeConverter
    {
        #region Public Interface.

        /// <summary>
        /// An instance of the IPAddressConverter class. This field is
        /// readonly.
        /// </summary>
        public static readonly IPAddressConverter Instance = new IPAddressConverter();

        /// <summary>
        /// Returns whether this converter can convert an object of the given
        /// type to the type of this converter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="System.Type"/> that represents the type you want to
        /// convert from.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this converter can perform the conversion;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert type converter type to
        /// the given type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="System.Type"/> that represents the type you want to
        /// convert to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this converter can perform the conversion;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(IPAddress))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// An object that represents the converted value.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        /// <exception cref="System.FormatException">
        /// Thrown when <paramref name="value"/> is not valid string representation of an
        /// <see cref="System.Net.IPAddress"/>.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            var ipString = value as string;

            if (ipString != null)
            {
                return IPAddress.Parse(ipString);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="destinationType">
        /// The type to convert the value parameter to.
        /// </param>
        /// <returns>
        /// An object that represents the converted value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="destinationType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
