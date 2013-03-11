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
using System.Threading;

namespace AK.Net.Dns.Configuration.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.TypeDescriptionProvider"/> for
    /// <see cref="AK.Net.Dns.DnsName"/> instances. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class DnsNameTypeDescriptionProvider : TypeDescriptionProvider
    {
        #region Private Impl.

        private static int _registeredProvider;

        #endregion

        #region Public Interface.

        /// <summary>
        /// An instance of the DnsNameTypeDescriptionProvider class. This field is
        /// readonly.
        /// </summary>
        public static readonly DnsNameTypeDescriptionProvider Instance =
            new DnsNameTypeDescriptionProvider();

        /// <summary>
        /// Returns the type descriptor for the specified type and instance.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="instance">The object instance.</param>
        /// <returns>The type descriptor for the specified type and instance.</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {

            return DnsNameCustomTypeDescriptor.Instance;
        }

        /// <summary>
        /// Registers the provider with the
        /// <see cref="System.ComponentModel.TypeDescriptor"/> infrastructure.
        /// </summary>
        /// <remarks>
        /// This method is safe to call multiple times.
        /// </remarks>
        public static void RegisterProvider() {

            if(Interlocked.Exchange(ref _registeredProvider, 1) == 0)
                TypeDescriptor.AddProvider(DnsNameTypeDescriptionProvider.Instance, typeof(DnsName));
        }

        #endregion
    }
}
