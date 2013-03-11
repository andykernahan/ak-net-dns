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
using System.Linq;
using System.Reflection;
using MbUnit.Framework;

using AK.Net.Dns.Records;

namespace AK.Net.Dns.Test
{
    [TestFixture]
    [TestsOn(typeof(DnsFuncs))]    
    public class DnsFuncsTest
    {
        private sealed class PredicateTestCase
        {            
            public readonly IList<DnsRecordType> Types;
            public readonly Predicate<DnsRecord> Predicate;

            public PredicateTestCase(Predicate<DnsRecord> predicate, params DnsRecordType[] types) {

                this.Predicate = predicate;
                this.Types = Array.AsReadOnly(types);
            }
        }

        private static readonly PredicateTestCase[] PREDICATE_TEST_CASES = {
            new PredicateTestCase(DnsFuncs.IsA, DnsRecordType.A),
            new PredicateTestCase(DnsFuncs.IsAaaa, DnsRecordType.Aaaa),
            new PredicateTestCase(DnsFuncs.IsAOrAaaa, DnsRecordType.A, DnsRecordType.Aaaa),
            new PredicateTestCase(DnsFuncs.IsAsfDB, DnsRecordType.AsfDB),
            new PredicateTestCase(DnsFuncs.IsCName, DnsRecordType.CName),
            new PredicateTestCase(DnsFuncs.IsDN, DnsRecordType.DN),
            new PredicateTestCase(DnsFuncs.IsHInfo, DnsRecordType.HInfo),
            new PredicateTestCase(DnsFuncs.IsIsdn, DnsRecordType.Isdn),
            new PredicateTestCase(DnsFuncs.IsLoc, DnsRecordType.Loc),
            new PredicateTestCase(DnsFuncs.IsMB, DnsRecordType.MB),
            new PredicateTestCase(DnsFuncs.IsMG, DnsRecordType.MG),
            new PredicateTestCase(DnsFuncs.IsMInfo, DnsRecordType.MInfo),
            new PredicateTestCase(DnsFuncs.IsMR, DnsRecordType.MR),
            new PredicateTestCase(DnsFuncs.IsMX, DnsRecordType.MX),
            new PredicateTestCase(DnsFuncs.IsNS, DnsRecordType.NS),
            new PredicateTestCase(DnsFuncs.IsNull, DnsRecordType.Null),
            new PredicateTestCase(DnsFuncs.IsPtr, DnsRecordType.Ptr),
            new PredicateTestCase(DnsFuncs.IsRP, DnsRecordType.RP),
            new PredicateTestCase(DnsFuncs.IsRT, DnsRecordType.RT),
            new PredicateTestCase(DnsFuncs.IsSoa, DnsRecordType.Soa),
            new PredicateTestCase(DnsFuncs.IsSrv, DnsRecordType.Srv),
            new PredicateTestCase(DnsFuncs.IsTxt, DnsRecordType.Txt),
            new PredicateTestCase(DnsFuncs.IsWks, DnsRecordType.Wks),
            new PredicateTestCase(DnsFuncs.IsX25, DnsRecordType.X25)
        };

        [Test(Order = 0)]
        public void an_is_method_should_exist_for_all_record_type_enum_members() {

            string methodName;
            string[] typeNames = Enum.GetNames(typeof(DnsRecordType));
            ILookup<string, MethodInfo> methods =
                (from method in typeof(DnsFuncs).GetMethods()
                 where method.Name.StartsWith("Is") && method.IsStatic && method.ReturnType == typeof(bool)
                 let parmeters = method.GetParameters()
                 where parmeters.Length == 1 && parmeters[0].ParameterType == typeof(DnsRecord)
                 select method).ToLookup<MethodInfo, string>((item) => item.Name);

            foreach(string typeName in typeNames) {
                // Methods should be named IsCN, IsA, IsMX etc.
                methodName = "Is" + typeName;
                Assert.IsTrue(methods.Contains(methodName), 
                    "an Is* method named {0} should exist for record type {1}.{2}", methodName, 
                    typeof(DnsRecordType).Name, typeName);
            }
        }

        [Test(Order = 1)]
        public void a_to_method_should_exist_for_all_exported_dns_record_types() {

            string methodName;
            Type recordBaseType = typeof(DnsRecord);
            var recordTypeNames = from type in recordBaseType.Assembly.GetExportedTypes()
                                  where type != recordBaseType && recordBaseType.IsAssignableFrom(type)
                                  select type.Name;
            ILookup<string, MethodInfo> methods =
                (from method in typeof(DnsFuncs).GetMethods()
                 where method.Name.StartsWith("To") && method.IsStatic && recordBaseType.IsAssignableFrom(method.ReturnType)
                 let parmeters = method.GetParameters()
                 where parmeters.Length == 1 && parmeters[0].ParameterType == typeof(DnsRecord)
                 select method).ToLookup<MethodInfo, string>((m) => m.Name);

            foreach(string recordTypeName in recordTypeNames) {
                // Methods should be named ToCn, ToA, ToMx etc.
                methodName = "To" + recordTypeName.Replace("Record", "");
                Assert.IsTrue(methods.Contains(methodName),
                    "a To* method named {0} should exist for record type {1}", methodName, recordTypeName);
            }
        }

        [Test(Order = 2)]
        public void all_predicates() {

            XRecord record;
            DnsName owner = DnsName.Parse("test.com.");
            byte[] rdata = new byte[0];
            DnsRecordType[] allRecordTypes = (DnsRecordType[])Enum.GetValues(typeof(DnsRecordType));

            foreach(PredicateTestCase @case in PREDICATE_TEST_CASES) {
                Assert.IsFalse(@case.Predicate(null));
                foreach(DnsRecordType type in @case.Types) {
                    record = new XRecord(owner, type, DnsRecordClass.IN, TimeSpan.FromHours(1), rdata);
                    Assert.IsTrue(@case.Predicate(record), "{0} expected to return true for {1}", @case.Predicate.Method, type);
                }
                foreach(DnsRecordType type in allRecordTypes) {
                    if(!@case.Types.Contains(type)) {
                        record = new XRecord(owner, type, DnsRecordClass.IN, TimeSpan.FromHours(1), rdata);
                        Assert.IsFalse(@case.Predicate(record), "{0} expected to return false for {1}", @case.Predicate.Method, type);
                    }
                }
            }
        }
    }
}
