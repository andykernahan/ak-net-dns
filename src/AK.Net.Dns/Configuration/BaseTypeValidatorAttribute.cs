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
    /// property is assignable to the base type specified by the attribute. This
    /// class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BaseTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        #region Private Fields.

        private Type _baseType;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the BaseTypeValidatorAttribute class.
        /// </summary>
        /// <param name="baseType">The base <see cref="System.Type"/> that the
        /// configuration property must be assignable to.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="baseType"/> is <see langword="null"/>.
        /// </exception>
        public BaseTypeValidatorAttribute(Type baseType) {

            Guard.NotNull(baseType, "baseType");

            _baseType = baseType;            
        }

        /// <summary>
        /// Gets or sets the base <see cref="System.Type"/> that the configuration
        /// property value must be assignable to.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public Type BaseType {

            get { return _baseType; }
            set {
                Guard.NotNull(value, "value");
                _baseType = value;
            }
        }

        /// <summary>
        /// Gets the attribute validator instance.
        /// </summary>
        public override ConfigurationValidatorBase ValidatorInstance {

            get { return new BaseTypeValidator(this.BaseType); }
        }

        #endregion
    }
}
