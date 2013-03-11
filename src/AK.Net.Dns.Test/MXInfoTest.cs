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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace AK.Net.Dns.Test
{
    [TestFixture]
    [TestsOn(typeof(MXInfo))]     
    public class MXInfoTest
    {
        private readonly DnsName DOMAIN = DnsName.Parse("test.com");
        private readonly DnsName[] EXCHANGES = {
            DnsName.Parse("mx1.test.com"),
            DnsName.Parse("mx2.test.com"),
            DnsName.Parse("mx3.test.com"),
            DnsName.Parse("mx4.test.com")
        };

        [Test]
        [ExpectedArgumentNullException]
        public void ctor_throws_if_domain_is_null() {

            MXInfo info = new MXInfo(null);            
        }

        [Test]
        public void ctor_sets_properties() {

            MXInfo info;

            info = new MXInfo(DOMAIN);
            Assert.AreSame(DOMAIN, info.Domain);
            Assert.IsNotNull(info.Exchanges);
            Assert.IsEmpty(info.Exchanges);

            info = new MXInfo(DOMAIN, EXCHANGES);
            Assert.AreSame(DOMAIN, info.Domain);            
            Assert.AreElementsEqual(EXCHANGES, info.Exchanges);            
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void exchanges_is_readonly_list() {

            MXInfo info = new MXInfo(DOMAIN, EXCHANGES);

            info.Exchanges.Add(DnsName.Parse("mx5.test.com"));
        }
    }
}
