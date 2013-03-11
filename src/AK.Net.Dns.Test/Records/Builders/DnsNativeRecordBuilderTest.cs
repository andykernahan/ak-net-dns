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
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using AK.Net.Dns.Records;
using AK.Net.Dns.Records.Builders;

namespace AK.Net.Dns.Test.Records
{
    [TestFixture]
    [TestsOn(typeof(DnsNativeRecordBuilder))]
    public class DnsNativeRecordBuilderTest
    {
        [Test]
        public void can_build_should_return_true_for_all_exported_record_types() {

            IDnsRecordBuilder builder = DnsNativeRecordBuilder.Instance;
            Type recordBaseType = typeof(DnsRecord);
            var recordTypes = from type in recordBaseType.Assembly.GetExportedTypes()
                              where type != typeof(DnsRecord) && type != typeof(XRecord) && typeof(DnsRecord).IsAssignableFrom(type)
                              select (DnsRecordType)Enum.Parse(typeof(DnsRecordType), type.Name.Substring(0, type.Name.Length - "Record".Length));

            foreach(DnsRecordType type in recordTypes) {
                Assert.IsTrue(builder.CanBuild(type), "builder should be able to build records of type {0}.{1}",
                    typeof(DnsRecordType).Name, type);
            }
        }
    }
}
