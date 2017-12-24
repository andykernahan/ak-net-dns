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

using System.ComponentModel;

namespace AK.Net.Dns.Configuration.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.CustomTypeDescriptor"/> for
    /// <see cref="System.Net.IPAddress"/> instances. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class IPAddressCustomTypeDescriptor : CustomTypeDescriptor
    {
        #region Public Interface.

        /// <summary>
        /// An instance of the IPAddressCustomTypeDescriptor class. This field is
        /// readonly.
        /// </summary>
        public static readonly IPAddressCustomTypeDescriptor Instance =
            new IPAddressCustomTypeDescriptor();

        /// <summary>
        /// Returns the <see cref="System.ComponentModel.TypeConverter"/> for
        /// the described type.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ComponentModel.TypeConverter"/> for the
        /// described type.
        /// </returns>
        public override TypeConverter GetConverter()
        {
            return IPAddressConverter.Instance;
        }

        #endregion
    }
}
