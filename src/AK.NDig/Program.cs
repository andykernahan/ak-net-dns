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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

using AK.NDig.Configuration;
using AK.Net.Dns;
using AK.Net.Dns.IO;
using AK.Net.Dns.Records;
using AK.Net.Dns.Resolvers;

namespace AK.NDig
{
    /// <summary>
    /// The main application. This class cannot be inherited.
    /// </summary>
    public sealed class Program : CommandDriven
    {
        #region Private Fields.
        
        private const string LINE_PREFIX = "> ";
        private static readonly string PAGE_BREAK = new string('-', 70);
        private const string BOOL_OPT_DESC = "(yes,no,on,off,1,0)";
        private static readonly log4net.ILog _log = 
            log4net.LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The host provided arguments.</param>
        public static void Main(string[] args) {            

            log4net.Config.XmlConfigurator.Configure();
            Console.Title = AssemblyInfo.VersionedTitle;            

            try {
                new Program().Run();
            } catch(Exception exc) {
                if(IsFatal(exc))
                    throw;                
                _log.Fatal(exc);                
                Console.Error.WriteLine(exc.ToString());
            }
        }        

        /// <summary>
        /// Initialises a new instance of the Program class.
        /// </summary>
        public Program() {
            
            this.LinePrefix = LINE_PREFIX;
            this.PageBreak = PAGE_BREAK;
            this.Commands.Add(new Command("set", DoSetCommand, 1, 2));
            this.Commands.Add(new Command("config", DoConfigCommand, 0, 0));
            this.Commands.Add(new Command("servers", DoServersCommand, 0, 0));
            this.Commands.Add(new Command("version", DoVersionCommand, 0, 0));
            this.Commands.Add(new Command("help", DoUsageCommand, 0, 0));            
            this.Commands.Add(new Command("?", DoUsageCommand, 0, 0));            
            this.Commands.Add(new Command("exit", DoExitCommand, 0, 1, CommandOptions.IsTerminal));
            this.Commands.Add(new Command("query", DoQueryCommand, 1, 4, CommandOptions.IsDefault | CommandOptions.RequiresAllArgs));            
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        public void Run() {

            LoadConfiguration();
            WritePageBreak();
            if(!GetOption(NDigOptions.SuppressLogo)) {                
                DoVersionCommand(null);
                WritePageBreak();
            }
            ProcessCommands();
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Writes a formatted usage message to the output.
        /// </summary>
        /// <param name="format">The format specification.</param>
        /// <param name="args">The format arguments.</param>
        protected override void WriteUsage(string format, params object[] args) {            

            if(!string.IsNullOrEmpty(format)) {
                WriteLine(format, args);
                WriteLine();
            }
            WriteLine("[@server] [qtype] [qclass] qname");
            WriteLine("  - retrieve info about qname");
            WriteLine("      server:      query server name or forward server index");
            WriteLine("      qtype:       query type (A,CNAME,MX,NS,PTR,SOA,TXT,...)");            
            WriteLine("      qclass:      query class (IN,CH,HS,ANY)");            
            WriteLine("      qname:       query target name");
            WriteLine("set opt [value]");
            WriteLine("  - set specified option, where opt is one of:");
            WriteLine("      qtype:       default qtype (current:{0})", DnsUtility.ToString(this.DefaultQueryType));
            WriteLine("      qclass:      default qclass (current:{0})", DnsUtility.ToString(this.DefaultQueryClass));            
            WriteLine("      suffix:      appends name to each query (current:{0})", this.NameSuffix);
            WriteLine("      reverse:     reverse ips before display (state:{0})", GetOptionOnOff(NDigOptions.ReverseAddrs));
            WriteLine("      sort:        sort section before display (state:{0})", GetOptionOnOff(NDigOptions.SortRecords));
            WriteLine("      header:      header display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteHeader));
            WriteLine("      answer:      answer section display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteAnswer));
            WriteLine("      question:    question section display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteQuestion));
            WriteLine("      authority:   authority section display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteAuthority));
            WriteLine("      additional:  additional section display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteAdditional));
            WriteLine("      stats:       query stats display mode (state:{0})", GetOptionOnOff(NDigOptions.WriteStats));
            WriteLine("      noempty:     suppress empty section display (state:{0})", GetOptionOnOff(NDigOptions.SuppressEmptySections));            
            WriteLine("      nologo:      suppress program logo (state:{0})", GetOptionOnOff(NDigOptions.SuppressLogo));
            WriteLine("config");
            WriteLine("  - print resolver configuration");
            WriteLine("servers");
            WriteLine("  - print forward server list");
            WriteLine("version");
            WriteLine("  - print version and copyright header");
            WriteLine("help, ?");
            WriteLine("  - print help");
            WriteLine("exit [save]");
            WriteLine("  - exit program");
            WriteLine("      save:        save configuration (default:yes)");
        }        

        #endregion

        #region Private Impl.

        private void DoQueryCommand(string[] args) {
            
            DnsName qname;
            DnsQueryType ttype;
            DnsQueryType qtype = this.DefaultQueryType;            
            DnsQueryClass tclass;
            DnsQueryClass qclass = this.DefaultQueryClass;            
            string qnameArg = args[args.Length - 1];
            IPAddress taddr;
            IPEndPoint qserver = null;            
            
            for(int i = 0; i < args.Length - 1; ++i) {
                if(args[i][0] == '@') {
                    int serverIndex;
                    string arg = args[i].Substring(1);

                    if(int.TryParse(arg, out serverIndex)) {
                        if(serverIndex < 0 || serverIndex > this.Resolver.Servers.Count - 1) {
                            WriteUsage("@server: {0} is not a valid forward server index (0-{1})", 
                                arg, this.Resolver.Servers.Count - 1);
                            return;
                        }
                        qserver = this.Resolver.Servers[serverIndex];
                    } else if(IPAddress.TryParse(arg, out taddr) || TryGetHostAddress(arg, out taddr)) {
                        qserver = new IPEndPoint(taddr, DnsTransport.DnsPort);
                    } else {
                        WriteUsage("@server: '{0}' is not a valid server/ip/index or it could not be resolved", arg);
                        return;
                    }
                    continue;
                }
                if(TryParseEnum(args[i], out ttype)) {
                    qtype = ttype;
                    continue;
                }
                if(TryParseEnum(args[i], out tclass)) {
                    qclass = tclass;
                    continue;
                }
                WriteUsage("qtype,qclass: '{0}' is not a value qtype or qclass", args[i]);
                return;
            }           

            if(qtype == DnsQueryType.Ptr) {
                if(!IPAddress.TryParse(qnameArg, out taddr)) {
                    WriteUsage("qname: '{0}' is not a valid ip address", qnameArg);
                    return;
                }
                qname = PtrRecord.MakeName(taddr);
            } else if(DnsName.TryParse(qnameArg, out qname)) {
                if(qname.Kind == DnsNameKind.Relative && this.NameSuffix != null)
                    qname = qname.Concat(this.NameSuffix);
            } else {
                WriteUsage("qname: '{0}' is not a valid qname", qnameArg);
                return;
            }

            DoQueryCommand(new DnsQuestion(qname, qtype, qclass), qserver);
        }

        private bool TryGetHostAddress(string hostOrAddress, out IPAddress addr) {

            IPHostEntry entry;

            try {
                entry = this.Resolver.GetHostEntry(hostOrAddress);
                if(entry.AddressList.Length > 0) {
                    addr = entry.AddressList[0];
                    return true;
                }
            } catch(ArgumentException) {
            } catch(DnsException) { }

            addr = null;

            return false;
        }

        private void DoQueryCommand(DnsQuestion question, IPEndPoint server) {

            DnsReply reply;            
            List<IPEndPoint> servers = null;            
            Stopwatch sw = Stopwatch.StartNew();

            if(server != null) {
                servers = new List<IPEndPoint>(this.Resolver.Servers);
                this.Resolver.Servers.Clear();                
                this.Resolver.Servers.Add(server);
            }
            try {
                reply = this.Resolver.Resolve(question);
                sw.Stop();
            } catch(DnsException exc) {
                WriteLine("{0} - {1}", exc.GetType().Name, exc.Message);
                return;
            } finally {
                if(servers != null) {
                    this.Resolver.Servers.Clear();
                    servers.ForEach((ep) => this.Resolver.Servers.Add(ep));
                }
            }
            
            if(GetOption(NDigOptions.WriteHeader))
                WriteHeader(reply.Header);
            if(GetOption(NDigOptions.WriteQuestion))
                WriteSection("QUESTION", reply.Questions);
            if(GetOption(NDigOptions.WriteAnswer))
                WriteSection("ANSWER", reply.Answers);
            if(GetOption(NDigOptions.WriteAuthority))
                WriteSection("AUTHORITY", reply.Authorities);
            if(GetOption(NDigOptions.WriteAdditional))
                WriteSection("ADDITIONAL", reply.Additionals);
            if(GetOption(NDigOptions.WriteStats))
                WriteStats(server != null ? server : this.Resolver.Servers[0], sw.Elapsed);
        }

        private void DoUsageCommand(string[] args) {

            WriteUsage();
        }

        private void DoConfigCommand(string[] args) {
            
            WriteObject(this.Resolver, "Resolver", 0);            
        }

        private void WriteObject(object graph, string name, int depth) {

            // This is not meant to be complete.

            object value;            

            WriteLine("{0}{1}: {2}", GetIndent(depth), name, graph != null ? graph : "(null)");
            if(graph != null && Type.GetTypeCode(graph.GetType()) == TypeCode.Object) {
                foreach(PropertyInfo property in GetPublicInstanceProperties(graph)) {
                    try {
                        value = property.GetGetMethod().Invoke(graph, null);
                    } catch(TargetInvocationException) {
                        continue;
                    }
                    if(!(value is IEnumerable) || value is string) {
                        WriteObject(value, property.Name, depth + 1);
                    } else {
                        int i = 0;

                        WriteLine("{0}{1}:", GetIndent(depth + 1), property.Name);
                        foreach(object element in (IEnumerable)value)
                            WriteObject(element, string.Format("[{0}]", i++), depth + 2);
                    }
                }
            }            
        }

        private static string GetIndent(int depth) {

            return new string('.', depth * 2);
        }

        private static IEnumerable<PropertyInfo> GetPublicInstanceProperties(object graph) {

            return from property in graph.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   let getMethod = property.GetGetMethod()
                   where getMethod != null && getMethod.GetParameters().Length == 0
                   orderby property.Name
                   select property;            
        }

        private void DoServersCommand(string[] args) {

            int i = 0;

            foreach(IPEndPoint server in this.Resolver.Servers)
                WriteLine("[{0}]: {1} ({2})", i++, server, ReverseIfEnabled(server.Address));
        }

        private void DoSetCommand(string[] args) {            

            switch(args[0].ToLowerInvariant()) {
                case "suffix":
                    DnsName suffix = null;

                    if(args.Length == 2 && !DnsName.TryParse(args[1], out suffix)) {
                        WriteUsage("value: '{0}' is not a valid name", args[1]);
                        return;
                    }
                    this.NameSuffix = suffix;
                    WriteLine("suffix set: {0}", this.NameSuffix);
                    break;
                case "qclass":
                    DnsQueryClass qclass = DnsQueryClass.IN;

                    if(args.Length == 2 && !TryParseEnum(args[1], out qclass)) {
                        WriteUsage("value: '{0}' is not a valid qclass", args[1]);
                        return;
                    }
                    this.DefaultQueryClass = qclass;
                    WriteLine("qclass set: {0}", DnsUtility.ToString(this.DefaultQueryClass));
                    break;
                case "qtype":
                    DnsQueryType qtype = DnsQueryType.A;

                    if(args.Length == 2 && !TryParseEnum(args[1], out qtype)) {
                        WriteUsage("value: '{0}' is not a valid qtype", args[1]);
                        return;
                    }
                    this.DefaultQueryType = qtype;
                    WriteLine("qtype set: {0}", DnsUtility.ToString(this.DefaultQueryType));
                    break;
                case "reverse":
                    ParseBooleanOption(NDigOptions.ReverseAddrs, "reverse", args);
                    break;
                case "sort":
                    ParseBooleanOption(NDigOptions.SortRecords, "sort", args);
                    break;
                case "header":
                    ParseBooleanOption(NDigOptions.WriteHeader, "header", args);
                    break;
                case "question":
                    ParseBooleanOption(NDigOptions.WriteQuestion, "question", args);
                    break;
                case "answer":
                    ParseBooleanOption(NDigOptions.WriteAnswer, "answer", args);
                    break;
                case "authority":
                    ParseBooleanOption(NDigOptions.WriteAuthority, "authority", args);
                    break;
                case "additional":
                    ParseBooleanOption(NDigOptions.WriteAdditional, "additional", args);
                    break;
                case "stats":
                    ParseBooleanOption(NDigOptions.WriteStats, "stats", args);
                    break;                
                case "noempty":
                    ParseBooleanOption(NDigOptions.SuppressEmptySections, "noempty", args);
                    break;
                case "nologo":
                    ParseBooleanOption(NDigOptions.SuppressLogo, "nologo", args);
                    break;
                default:
                    WriteUsage("opt: '{0}' is not a valid option", args[0]);
                    return;
            }
        }        

        private static bool TryParseEnum<T>(string s, out T result) {            

            try {
                result = (T)Enum.Parse(typeof(T), s, true);
                return true;
            } catch(ArgumentException) { }

            result = default(T);

            return false;
        }

        private void ParseBooleanOption(NDigOptions option, string name, string[] args) {

            bool on = false;

            if(args.Length == 1) {
                WriteUsage("value: required argument");
                return;
            } else if(!TryParseBoolean(args[1], out on)) {
                WriteUsage("value: '{0}' is not a valid bool {1}", args[1], BOOL_OPT_DESC);
                return;
            }
            SetOption(option, on);
            WriteLine("{0} set: {1}", name, GetOptionOnOff(option));
        }

        private static bool TryParseBoolean(string s, out bool result) {            

            switch(s.ToLowerInvariant()) {
                case "1":
                case "yes":
                case "on":                
                    result = true;
                    break;
                case "0":
                case "no":
                case "off":
                    result = false;
                    break;
                default:
                    result = false;
                    return false;
            }

            return true;
        }

        private void DoVersionCommand(string[] args) {

            WriteLine(AssemblyInfo.VersionedTitle);
            WriteLine(AssemblyInfo.Copyright);
        }

        private void DoExitCommand(string[] args) {

            bool save = true;

            if(args.Length == 1)
                TryParseBoolean(args[0], out save);

            if(save)
                SaveConfiguation();
        }

        private void WriteStats(IPEndPoint responder, TimeSpan elapsed) {

            WriteLine("QUERY STATS:");
            WriteLine();
            WriteLine("Elapsed: {0:0.00}ms", elapsed.TotalMilliseconds);
            WriteLine("Server:  {0} ({1})", responder, ReverseIfEnabled(responder.Address));
            WriteLine("When:    {0:r}", DnsClock.Now());
            WriteLine();
        }

        private string ReverseIfEnabled(IPAddress addr) {
            
            return GetOption(NDigOptions.ReverseAddrs) ? Reverse(addr) : addr.ToString();
        }

        private string Reverse(IPAddress addr) {

            string host = null;
            IAsyncResult iar = this.Resolver.BeginGetHostEntry(addr, null, null);

            if(iar.AsyncWaitHandle.WaitOne(1000, false)) {
                try {
                    host = this.Resolver.EndGetHostEntry(iar).HostName;
                } catch(DnsException) { }
            }

            return !string.IsNullOrEmpty(host) ? host : addr.ToString();
        }

        private void WriteHeader(DnsHeader header) {

            bool printedWarning = false;

            WriteLine("HEADER:");
            WriteLine();
            WriteLine("Opcode: {0}, Rcode: {1}, Id: {2}, Flags: {3}",
                DnsUtility.ToString(header.OpCode), 
                header.ResponseCode.ToString().ToUpperInvariant(), header.Id,
                GetHeaderFlags(header));
            WriteLine("Question: {0}, Answer: {1}, Authority: {2}, Additional: {3}",
                header.QuestionCount, header.AnswerCount, header.AuthorityCount,
                header.AdditionalCount);
            WriteLine();
            if(header.IsTruncated) {
                WriteLine("WARNING: answer was truncated");
                printedWarning = true;
            }
            if(header.IsRecursionDesired && !header.IsRecursionAllowed && !header.IsAuthorative) {
                WriteLine("WARNING: recursion was requested but disallowed");
                printedWarning = true;
            }
            if(printedWarning)
                WriteLine();            
        }

        private static string GetHeaderFlags(DnsHeader header) {

            StringBuilder flags = new StringBuilder();

            if(!header.IsQuery)
                flags.Append("qr ");
            if(header.IsAuthorative)
                flags.Append("aa ");
            if(header.IsTruncated)
                flags.Append("at ");
            if(header.IsRecursionDesired)
                flags.Append("rd ");
            if(header.IsRecursionAllowed)
                flags.Append("ra");

            if(flags.Length > 0 && flags[flags.Length - 1] == ' ')
                --flags.Length;

            return flags.ToString();
        }

        private void WriteSection(string name, DnsQuestionCollection questions) {

            if(questions.Count == 0 && GetOption(NDigOptions.SuppressEmptySections))
                return;

            WriteLine("{0} SECTION:", name);
            WriteLine();
            if(questions.Count > 0) {
                foreach(DnsQuestion question in questions)
                    WriteLine(question.ToString());
                WriteLine();
            }
        }

        private void WriteSection<T>(string name, DnsRecordCollection<T> records) where T : DnsRecord {

            if(records.Count == 0 && GetOption(NDigOptions.SuppressEmptySections))
                return;            

            WriteLine("{0} SECTION:", name);
            WriteLine();
            if(records.Count > 0) {
                IEnumerable<T> rrset = GetOption(NDigOptions.SortRecords) ? SortRecords(records) : records;

                foreach(DnsRecord record in rrset)
                    WriteLine(record.ToString());
                WriteLine();
            }
        }

        private static IEnumerable<T> SortRecords<T>(DnsRecordCollection<T> records) where T : DnsRecord {

            T[] arr = records.ToArray();

            Array.Sort(arr, DnsRecordComparer.Instance);

            return arr;
        }

        private void LoadConfiguration() {

            // These could easily be made into configuration points but I think that users would
            // find their persistence annoying.
            this.DefaultQueryType = DnsQueryType.A;
            this.DefaultQueryClass = DnsQueryClass.IN;            
            this.Resolver = DnsStubResolver.Instance();
            try {
                this.ExeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                this.ConfigSection = (NDigSection)this.ExeConfig.Sections[NDigSection.DefaultLocation];
                if(this.ConfigSection == null) {
                    this.ConfigSection = new NDigSection();
                    this.ExeConfig.Sections.Add(NDigSection.DefaultLocation, this.ConfigSection);
                }
            } catch(ConfigurationErrorsException exc) {
                _log.Error(exc);
                this.ConfigSection = new NDigSection();
                WriteLine("WARNING: failed to load configuration: '{0}'", exc.Message);
                WritePageBreak();                
            }
        }

        private void SaveConfiguation() {

            if(this.ExeConfig == null)
                return;

            try {
                this.ExeConfig.Save();
            } catch(ConfigurationErrorsException exc) {
                _log.Error(exc);
                WriteLine("WARNING: failed to save configuration: '{0}'", exc.Message);
                WritePageBreak();
            }
        }

        private bool GetOption(NDigOptions option) {

            return (this.Options & option) == option;
        }

        private void SetOption(NDigOptions option, bool value) {

            if(value)
                this.Options |= option;
            else
                this.Options &= ~option;
        }

        private string GetOptionOnOff(NDigOptions option) {

            return GetOption(option) ? "on" : "off";
        }

        private static bool IsFatal(Exception exc) {

            return exc is OutOfMemoryException ||
                exc is StackOverflowException ||
                exc is ExecutionEngineException ||
                exc is ThreadAbortException ||
                exc is TypeInitializationException;
        }

        private NDigOptions Options {

            get { return this.ConfigSection.Options; }
            set { this.ConfigSection.Options = value; }
        }

        private DnsQueryClass DefaultQueryClass { get; set; }

        private DnsQueryType DefaultQueryType { get; set; }

        private DnsStubResolver Resolver { get; set; }

        private System.Configuration.Configuration ExeConfig { get; set; }

        private NDigSection ConfigSection { get; set; }

        private DnsName NameSuffix {

            get { return this.Resolver.NameSuffix; }
            set { this.Resolver.NameSuffix = value; }
        }

        #endregion
    }
}
