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
using System.Collections.ObjectModel;

namespace AK.NDig
{
    /// <summary>
    /// Represents a collection of <see cref="AK.NDig.Command"/>
    /// instances. This collection does not allow <see langword="null"/>
    /// references to be added.
    /// </summary>
    [Serializable]
    public class CommandCollection : KeyedCollection<string, Command>
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the CommandCollection class.
        /// </summary>
        public CommandCollection()
            : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Gets the command with the specified <paramref name="name"/>. If no
        /// command exists with the specified <paramref name="name"/>; the default
        /// command is returned.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <returns>The command with the specified <paramref name="name"/>, or the
        /// default command if no exact match was found.</returns>
        public Command Get(string name) {

            if(Contains(name))
                return this[name];

            return this.Default;
        }       

        /// <summary>
        /// Gets the first default <see cref="AK.NDig.Command"/>.
        /// </summary>
        public Command Default {

            get {
                foreach(Command command in this) {
                    if(command.HasOption(CommandOptions.IsDefault))
                        return command;
                }
                return null;
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Returns the key for the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to return the key for.</param>
        /// <returns>The key for the specified <paramref name="item"/>.</returns>
        protected override string GetKeyForItem(Command item) {

            return item.Name;
        }

        /// <summary>
        /// Inserts an item into this collection at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected override void InsertItem(int index, Command item) {

            ValidateItem(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces the element at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The index of the element to replace.</param>
        /// <param name="item">The replacement item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected override void SetItem(int index, Command item) {

            ValidateItem(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Validates the specified item
        /// </summary>
        /// <param name="item">The question to item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void ValidateItem(Command item) {

            if(item == null)
                throw Error.ArgumentNull("item");         
        }

        #endregion        
    }
}

