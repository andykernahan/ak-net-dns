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

namespace AK.Net.Dns.Test
{
    /// <summary>
    /// The tests contained within this fixture assert that DnsQueryClass enum is
    /// a superset of the DnsRecordClass enum.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(DnsQueryClass))]
    public class DnsQueryClassTest : EnumSuperSetTest<DnsQueryClass, DnsRecordClass>
    {
        [Test(Order = 0)]
        public void a_query_class_should_exist_for_all_record_classs() {

            EnsureSubsetDefinesSuperset();
        }

        [Test(Order = 1)]
        public void query_class_values_must_equal_record_class_values() {

            EnsureSubsetValuesAreEqualToSuperSetValues();
        }
    }
}
