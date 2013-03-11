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
using Rhino.Mocks;

namespace AK.Net.Dns.Test
{
    [TestFixture]
    [TestsOn(typeof(DnsQuestion))]
    [VerifyEqualityContract(typeof(DnsQuestion), ImplementsOperatorOverloads = false)]
    public class DnsQuestionTest : IEquivalenceClassProvider<DnsQuestion>
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ctor_throws_if_name_is_null() {

            DnsQuestion question = new DnsQuestion(null, DnsQueryType.A, DnsQueryClass.IN);
        }

        [Test]
        public void ctor_sets_correct_properties() {

            DnsQueryClass cls = DnsQueryClass.IN;
            DnsQueryType type = DnsQueryType.A;
            DnsName name = DnsName.Parse("test.com");
            DnsQuestion question = new DnsQuestion(name, type, cls);

            Assert.AreEqual(cls, question.Class);
            Assert.AreEqual(type, question.Type);
            Assert.AreEqual(name, question.Name);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ctor_throws_if_reader_is_null() {

            DnsQuestion question = new DnsQuestion(null);
        }

        /*[Test]
        public void ctor_reads_correct_info_and_sets_correct_properties() {

            MockRepository mocks = new MockRepository();
            IDnsReader reader = mocks.StrictMock<IDnsReader>();
            DnsQueryClass cls = DnsQueryClass.In;
            DnsQueryType type = DnsQueryType.A;
            DnsName name = DnsName.Parse("test.com");
            DnsQuestion question;

            using(mocks.Ordered()) {
                Expect.On(reader).Call<DnsName>(reader.ReadDomain).Return(name);
                Expect.Call(delegate { reader.ReadQueryType(); });
                Expect.Call(delegate { reader.ReadQueryClass(); });
            }
            mocks.ReplayAll();
            question = new DnsQuestion(reader);
            mocks.VerifyAll();
            Assert.AreEqual(name, question.Name);
            Assert.AreEqual(type, question.Type);
            Assert.AreEqual(cls, question.Class);
        }*/

        [Test]
        [ExpectedArgumentNullException]
        public void write_throws_if_writer_is_null() {

            DnsQuestion question = new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.A, DnsQueryClass.IN);

            question.Write(null);
        }

        [Test]
        public void write_writes_data_in_correct_order_and_with_correct_arguments() {

            MockRepository mocks = new MockRepository();
            IDnsWriter writer = mocks.StrictMock<IDnsWriter>();
            DnsQuestion question = new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.MX, DnsQueryClass.IN);

            using(mocks.Ordered()) {
                writer.WriteName(question.Name);
                writer.WriteQueryType(question.Type);
                writer.WriteQueryClass(question.Class);
            }
            mocks.ReplayAll();
            question.Write(writer);
            mocks.VerifyAll();
        }

        public EquivalenceClassCollection<DnsQuestion> GetEquivalenceClasses() {

            return EquivalenceClassCollection<DnsQuestion>.FromDistinctInstances(
                new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.A, DnsQueryClass.Any),
                new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.A, DnsQueryClass.CH),
                new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.A, DnsQueryClass.HS),
                new DnsQuestion(DnsName.Parse("test.com"), DnsQueryType.A, DnsQueryClass.IN),
                new DnsQuestion(DnsName.Parse("test.co.uk"), DnsQueryType.A, DnsQueryClass.IN)
            );
        }
    }
}
