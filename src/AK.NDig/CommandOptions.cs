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
    /// Defines the various options for <see cref="AK.NDig.Command"/>
    /// instances.
    /// </summary>
    [Flags]
    [Serializable]
    public enum CommandOptions
    {
        /// <summary>
        /// Indicates no options for the command.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the command is the default command and should be
        /// executed if no other commands match the input.
        /// </summary>
        IsDefault = 1,
        /// <summary>
        /// Indicates that the command requires all arguments from the input.
        /// Commands without this option are not passed the command name as
        /// part of the arguments.
        /// </summary>
        RequiresAllArgs = 2,
        /// <summary>
        /// Indicates that once the command has completed execution, the program
        /// should be terminated.
        /// </summary>
        IsTerminal = 4,
    }
}

