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

using System.Net;
using AK.Net.Dns.IO;
using log4net.Config;

namespace AK.Net.Dns.Sandbox
{
    public static class Program
    {
        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void Main(string[] args)
        {
            var nameServers = AKDns.GetNameServers("rcmap.co.uk.");
            var mxInfo = AKDns.GetMXInfo("rcmap.co.uk.");

            IDnsTransport transport = new DnsUdpTransport();

            var query = new DnsQuery();

            query.Questions.Add(new DnsQuestion("rcmap.co.uk.", DnsQueryType.A, DnsQueryClass.IN));

            var reply = transport.Send(query, new IPEndPoint(IPAddress.Parse("8.8.8.8"), DnsTransport.DnsPort));
        }
    }
}
