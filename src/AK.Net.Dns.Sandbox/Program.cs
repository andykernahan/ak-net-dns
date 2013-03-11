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
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using AK.Net.Dns;
using AK.Net.Dns.Configuration;
using AK.Net.Dns.IO;
using AK.Net.Dns.Caching;

namespace AK.Net.Dns.Sandbox
{
    public static class Program
    {
        static Program()
        {

            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Main(string[] args)
        {
            var t = AKDns.GetNameServers("rcmap.co.uk.");
            var p = AKDns.GetMXInfo("rcmap.co.uk.");
            //var q = AKDns.GetHostEntry("rpc.thekernahans.net.");            

            IDnsTransport transport = new DnsUdpTransport();

            DnsQuery query = new DnsQuery();

            query.Questions.Add(new DnsQuestion("rcmap.co.uk.", DnsQueryType.A, DnsQueryClass.IN));

            DnsReply reply = transport.Send(query, new IPEndPoint(IPAddress.Parse("208.67.222.222"), DnsTransport.DnsPort));
        }
    }
}
