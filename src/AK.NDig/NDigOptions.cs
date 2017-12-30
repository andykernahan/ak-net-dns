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
    /// Specifies the user defined options for <see cref="AK.NDig.Program"/>.
    /// </summary>
    [Flags]
    [Serializable]
    public enum NDigOptions
    {
        /// <summary>
        /// No options are applied.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the response header should be displayed.
        /// </summary>
        WriteHeader = 1,
        /// <summary>
        /// Specifies that the response question section be displayed.
        /// </summary>
        WriteQuestion = 2,
        /// <summary>
        /// Specifies that the response answer section be displayed.
        /// </summary>
        WriteAnswer = 4,
        /// <summary>
        /// Specifies that the response authority section be displayed.
        /// </summary>
        WriteAuthority = 8,
        /// <summary>
        /// Specifies that the response additional section be displayed.
        /// </summary>
        WriteAdditional = 16,
        /// <summary>
        /// Specifies that the query statistics should be displayed.
        /// </summary>
        WriteStats = 32,
        /// <summary>
        /// Specifies that the display of empty sections should be suppressed.
        /// </summary>
        SuppressEmptySections = 64,
        /// <summary>
        /// Specifies that the application logo should be suppressed.
        /// </summary>
        SuppressLogo = 128,
        /// <summary>
        /// Specifies the default <see cref="AK.NDig.Program"/> display options.
        /// </summary>
        WriteDefault = (WriteHeader | WriteQuestion | WriteAnswer |
            WriteAuthority | WriteAdditional | SuppressEmptySections),
        /// <summary>
        /// Specifies that the records within a section should be sorted before
        /// display.
        /// </summary>
        SortRecords = 256,
        /// <summary>
        /// Specifies that IP addresses should be reversed before display.
        /// </summary>
        ReverseAddrs = 512,
        /// <summary>
        /// Specifies the default <see cref="AK.NDig.Program"/> options.
        /// </summary>
        Default = WriteDefault | SortRecords | ReverseAddrs
    }
}
