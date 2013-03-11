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

namespace AK.Net.Dns.Configuration.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.TypeConverter"/> for
    /// <see cref="AK.Net.Dns.DnsName"/> instances. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class DnsNameConverter : TypeConverter
    {
        #region Public Interface.

        /// <summary>
        /// An instance of the DnsNameConverter class. This field is readonly.
        /// </summary>
        public static readonly DnsNameConverter Instance =
            new DnsNameConverter();

        /// <summary>
        /// Returns whether this converter can convert an object of the given
        /// type to the type of this converter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="AK.Net.Dns.DnsName"/> that represents the type you want to
        /// convert from.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this converter can perform the conversion;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {

            if(sourceType == typeof(string))
                return true;

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
        /// A <see cref="AK.Net.Dns.DnsName"/> that represents the type you want to
        /// convert to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this converter can perform the conversion;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            if(destinationType == typeof(DnsName))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the
        /// pecified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
        /// provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="System.Globalization.CultureInfo"/> to use as the
        /// current culture.
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
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value) {

            string nameString = value as string;

            if(nameString != null)
                return DnsName.Parse(nameString);

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
        /// The <see cref="System.Globalization.CultureInfo"/> to use as the
        /// current culture.
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
            object value, Type destinationType) {

            if(destinationType == typeof(DnsName))
                return value.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
