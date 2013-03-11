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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Ak.Net.Dns.Test
{
    [TestFixture]
    [TestsOn(typeof(DnsQueryType))]
    public class DnsQueryTypeTest
    {
        [Test(Order = 0)]
        public void a_query_type_should_exist_for_all_record_type_enum_members() {            

            HashSet<string> recordTypes = new HashSet<string>(Enum.GetNames(typeof(DnsRecordType)));

            recordTypes.ExceptWith(Enum.GetNames(typeof(DnsQueryType)));
            if(recordTypes.Count != 0) {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("a {0} member should exist for the following {1} members:",
                    typeof(DnsQueryType).Name, typeof(DnsRecordType).Name).AppendLine();

                foreach(string missingMember in recordTypes)
                    sb.AppendLine(missingMember);

                Assert.Fail(sb.ToString());
            }
        }

        [Test(Order = 1)]
        public void query_type_values_must_equal_record_type_values() {

            //string[] queryTypeNames = Enum.GetNames(typeof(DnsQueryType));

            //foreach(string queryTypeName in queryTypeNames) {
            //    Assert.AreEqual((int)Enum.getv
            //}
        }
    }
}
