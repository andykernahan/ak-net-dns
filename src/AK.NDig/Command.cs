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

namespace AK.NDig
{
    /// <summary>
    /// Specifies a command.
    /// </summary>
    [Serializable]
    public class Command
    {
        #region Public Interface.

        /// <summary>
        /// Intialises a new instance of the Command class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="handler">The command handler.</param>
        /// <param name="minArgsLength">The minimum number of required arguments.</param>
        /// <param name="maxArgsLength">The maximum number of acceptable arguments.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> or <paramref name="handler"/> is 
        /// <see langword="null"/>.
        /// </exception>
        public Command(string name, Action<string[]> handler, int minArgsLength, int maxArgsLength)
            : this(name, handler, minArgsLength, maxArgsLength, CommandOptions.None) {
        }

        /// <summary>
        /// Intialises a new instance of the Command class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="handler">The command handler.</param>
        /// <param name="minArgsLength">The minimum number of required arguments.</param>
        /// <param name="maxArgsLength">The maximum number of acceptable arguments.</param>
        /// <param name="options">The command options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> or <paramref name="handler"/> is 
        /// <see langword="null"/>.
        /// </exception>
        public Command(string name, Action<string[]> handler, int minArgsLength, int maxArgsLength,
            CommandOptions options) {

            if(name == null || handler == null)
                throw Error.ArgumentNull(name == null ? "name" : "handler");

            this.Name = name;
            this.Handler = handler;
            this.MinArgsLength = minArgsLength;
            this.MaxArgsLength = maxArgsLength;
            this.Options = options;
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="length"/>
        /// is a value number of arguments for this command.
        /// </summary>
        /// <param name="length">The number of arguments.</param>
        /// <returns><see langword="true"/> if <paramref name="length"/> is a valid
        /// number of arguments for this command, otherwise; <see langword="false"/>.</returns>
        public bool IsValidArgsLength(int length) {

            return length >= this.MinArgsLength &&
                (this.MaxArgsLength == -1 || length <= this.MaxArgsLength);
        }

        /// <summary>
        /// Returns a value indicating if this command has the specified
        /// <paramref name="option"/> applied.
        /// </summary>
        /// <param name="option">The option to test.</param>
        /// <returns><see langword="true"/> if this command as the specified
        /// <paramref name="option"/> applied, otherwise; <see langword="false"/>.</returns>
        public bool HasOption(CommandOptions option) {

            return (this.Options & option) == option;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            if(!this.Name.Equals(string.Empty))
                return this.Name;
            if(HasOption(CommandOptions.IsDefault))
                return "$default$";

            return base.ToString();
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the minimum number of arguments required by the command.
        /// </summary>
        public int MinArgsLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of arguments accepted by the command.
        /// </summary>
        public int MaxArgsLength { get; private set; }

        /// <summary>
        /// Gets the options for this command.
        /// </summary>
        public CommandOptions Options { get; private set; }

        /// <summary>
        /// Gets the command handler.
        /// </summary>
        public Action<string[]> Handler { get; private set; }

        #endregion
    }
}

