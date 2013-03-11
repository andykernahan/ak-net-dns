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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Configuration;

namespace AK.Net.Dns.Configuration
{
    /// <summary>
    /// Validates that the <see cref="System.Type"/> specified by a configuration
    /// property is assignable to the base type specified by the validator. This
    /// class cannot be inherited.
    /// </summary>
    public sealed class BaseTypeValidator : ConfigurationValidatorBase
    {
        #region Private Fields.

        private readonly Type _baseType;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the BaseTypeValidator class.
        /// </summary>
        /// <param name="baseType">The base <see cref="System.Type"/> that the
        /// configuration property must be assignable to.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="baseType"/> is <see langword="null"/>.
        /// </exception>
        public BaseTypeValidator(Type baseType) {

            Guard.NotNull(baseType, "baseType");

            _baseType = baseType;
        }

        /// <summary>
        /// Returns a value indicating if this validator can validate property
        /// values of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The property value type.</param>
        /// <returns><see langword="true"/> if this validator can validate property
        /// values of the specified type, otherwise; <see langword="false"/>.</returns>
        public override bool CanValidate(Type type) {

            return type != null;
        }

        /// <summary>
        /// Validates the speciifed configuration property value.
        /// </summary>
        /// <param name="value">The property value to validate.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// Thrown when the base type is not assignable from the specified
        /// <paramref name="value"/>.
        /// </exception>
        public override void Validate(object value) {

            Type type = value as Type;

            if(type != null && this.BaseType.IsAssignableFrom(type))
                return;

            throw Guard.ConfigurationTypeIsNotAssignableFrom(this.BaseType, type);
        }

        /// <summary>
        /// Gets the base <see cref="System.Type"/> that the configuration property
        /// value must be assignable to.
        /// </summary>
        public Type BaseType {

            get { return _baseType; }
        }

        #endregion
    }
}
