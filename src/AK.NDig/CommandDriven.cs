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
using System.IO;

namespace AK.NDig
{
    /// <summary>
    /// Provides a useful base class for <see cref="AK.NDig.Command"/>
    /// driven sessions. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class CommandDriven : MarshalByRefObject
    {
        #region Private Fields.

        private string _linePrefix;
        private string _pageBreak;
        
        private static readonly char[] COMMAND_DELIMS = { ' ' };
        private static readonly string[] EMPTY_ARGS = { };        

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the CommandDriven class.
        /// </summary>
        protected CommandDriven() {            

            this.Commands = new CommandCollection();
            this.Input = Console.In;
            this.Output = Console.Out;
        }        

        /// <summary>
        /// Processed the commands from the input until the end is reached,
        /// a terminal command is executed or an unhandled exception is thrown.
        /// </summary>
        protected virtual void ProcessCommands() {

            string input;
            string[] args;
            Command command;

            WriteLinePrefix();
            while((input = ReadLine()) != null) {
                WritePageBreak();
                if(TryParseCommandAndArgs(input.Trim(), out command, out args)) {
                    command.Handler(args);
                    if(command.HasOption(CommandOptions.IsTerminal))
                        break;
                }
                WritePageBreak();
                WriteLinePrefix();
            }
        }

        /// <summary>
        /// Attemps to parse a command from the specified <paramref name="line"/>. This
        /// method will write appropiate usage messages if no command was parsed or
        /// the number of arguments does not meet the command specification.
        /// </summary>
        /// <param name="line">The line read from the input.</param>
        /// <param name="command">On success, this parameter will contain the command to
        /// be executed.</param>
        /// <param name="args">On success, this parameter will contain the arguments to
        /// pass to the command.</param>
        /// <returns><see langword="true"/> if a command and it's arguments were parsed,
        /// otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="line"/> is <see langword="null"/>.
        /// </exception>
        protected virtual bool TryParseCommandAndArgs(string line, out Command command, out string[] args) {

            if(line == null)
                throw Error.ArgumentNull("line");

            command = null;
            args = EMPTY_ARGS;            

            int delimIndex = line.IndexOfAny(COMMAND_DELIMS);
            string commandKey = delimIndex > -1 ? line.Substring(0, delimIndex) : line;

            if((command = this.Commands.Get(commandKey)) == null) {
                OnUnknownCommandKeyGiven(commandKey);
                return false;
            }

            if(command.HasOption(CommandOptions.RequiresAllArgs))
                args = line.Split(COMMAND_DELIMS, StringSplitOptions.RemoveEmptyEntries);
            else if(delimIndex > -1) {
                args = line.Substring(delimIndex + 1).Split(COMMAND_DELIMS,
                    StringSplitOptions.RemoveEmptyEntries);
            }

            if(!command.IsValidArgsLength(args.Length)) {
                OnInvalidNumberOfArgumentsGiven(command);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The handler that is invoked when an unknown command key has been
        /// parsed from the input. The default behaviour is to print an error
        /// message to the output.
        /// </summary>
        /// <param name="commandKey">The command key given.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="commandKey"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void OnUnknownCommandKeyGiven(string commandKey) {

            if(commandKey == null)
                throw Error.ArgumentNull("commandKey");

            WriteUsage("'{0}' does not match any command", commandKey);
        }

        /// <summary>
        /// The handler that is invoked when an incorrect number of arguments
        /// have been given for a command. The default behaviour is to print an
        /// error message to the output.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="command"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void OnInvalidNumberOfArgumentsGiven(Command command) {

            if(command == null)
                throw Error.ArgumentNull("command");

            WriteUsage("{0}: invalid number of arguments ({1}-{2})", command,
                command.MinArgsLength, command.MaxArgsLength);
        }

        /// <summary>
        /// Writes a usage message to the output.
        /// </summary>
        protected virtual void WriteUsage() {

            WriteUsage(null, null);
        }

        /// <summary>
        /// Writes a formatted usage message to the output.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <param name="args">The format arguments.</param>
        protected virtual void WriteUsage(string format, params object[] args) {

            if(!string.IsNullOrEmpty(format))
                WriteLine(format, args);
        }

        /// <summary>
        /// Writes the line prefix to the output.
        /// </summary>
        protected virtual void WriteLinePrefix() {

            if(!string.IsNullOrEmpty(this.LinePrefix))
                this.Output.Write(this.LinePrefix);
        }

        /// <summary>
        /// Writes the page break to the output.
        /// </summary>
        protected virtual void WritePageBreak() {

            if(!string.IsNullOrEmpty(this.PageBreak)) {
                WriteLinePrefix();
                this.Output.WriteLine(this.PageBreak);
            }
        }

        /// <summary>
        /// Writes a prefixed line to the output.
        /// </summary>
        protected virtual void WriteLine() {

            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the specified <paramref name="value"/> to the output. The
        /// value is prefixed before it is written.
        /// </summary>
        /// <param name="value">The value to print.</param>
        protected virtual void WriteLine(string value) {

            WriteLinePrefix();
            this.Output.WriteLine(value);
        }
        
        /// <summary>
        /// Writes out a formatted string to the output. The value is prefixed
        /// before it is written.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <param name="args">The format arguments.</param>
        protected virtual void WriteLine(string format, params object[] args) {

            WriteLinePrefix();
            this.Output.WriteLine(format, args);
        }        

        /// <summary>
        /// Reads a line from the input.
        /// </summary>
        /// <returns>The line read from the input, or <see langword="null"/> if the end
        /// has been reached.</returns>
        protected virtual string ReadLine() {

            return this.Input.ReadLine();
        }

        /// <summary>
        /// Gets or sets the command line prefix.
        /// </summary>
        protected string LinePrefix {

            get { return _linePrefix; }
            set { _linePrefix = value; }
        }        

        /// <summary>
        /// Gets or sets the string which is printed before and after a command
        /// is executed.
        /// </summary>
        protected string PageBreak {

            get { return _pageBreak; }
            set { _pageBreak = value; }
        }

        /// <summary>
        /// Gets the input.
        /// </summary>
        protected TextReader Input { get; private set; }

        /// <summary>
        /// Gets the output.
        /// </summary>
        protected TextWriter Output { get; private set; }

        /// <summary>
        /// Gets the collection of commands.
        /// </summary>
        protected CommandCollection Commands { get; private set; }

        #endregion
    }
}
