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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace AK.Net.Dns.Test
{
    [TestFixture]
    [TestsOn(typeof(DnsName))]
    [VerifyComparisonContract(typeof(DnsName), ImplementsOperatorOverloads = false)]
    [VerifyEqualityContract(typeof(DnsName), ImplementsOperatorOverloads = false)]
    public class DnsNameTest : IEquivalenceClassProvider<DnsName>
    {
        private sealed class TestCase
        {
            public readonly string Name;
            public readonly DnsNameKind Kind;
            public readonly IList<string> Labels;

            public TestCase(DnsNameKind kind, string name, params string[] labels) {

                this.Name = name;
                this.Kind = kind;
                this.Labels = Array.AsReadOnly(labels);
            }
        }

        private static readonly TestCase[] VALID_TEST_CASES = {
            new TestCase(DnsNameKind.Absolute, ".", new string[0]),
            new TestCase(DnsNameKind.Relative, "com", "com"),
            new TestCase(DnsNameKind.Relative, "test.com", "test", "com"),
            new TestCase(DnsNameKind.Absolute, "example.com.", "example", "com"),
            new TestCase(DnsNameKind.Relative, "test-test.com", "test-test", "com"),
            new TestCase(DnsNameKind.Absolute, "example-example.com.", "example-example", "com"),
            new TestCase(DnsNameKind.Relative, "0test", "0test"),
            new TestCase(DnsNameKind.Absolute, "0example.", "0example"),
            new TestCase(DnsNameKind.Relative, "0test0", "0test0"),
            new TestCase(DnsNameKind.Relative, "0test0.com", "0test0", "com"),
            new TestCase(DnsNameKind.Absolute, "0example0.com.", "0example0", "com"),
            new TestCase(DnsNameKind.Relative, "abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ.0123456789", "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "0123456789"),
            new TestCase(DnsNameKind.Absolute, "0123456789.ABCDEFGHIJKLMNOPQRSTUVWXYZ.abcdefghijklmnopqrstuvwxyz.", "0123456789", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz")
        };

        private static readonly string[] INVALID_NAMES = {
            "..",
            ".com",
            "test..com",
            "-",
            "-com",
            "test.-com",
            "-.com",
            "test-.com",
            "tes!.com",
            new string('a', 64), /* Max label length. */
            new string('a', 256) /* Max domain length. */
        };

        [Test]
        public void is_valid_identifies_correctly_formatted_names() {

            foreach(TestCase @case in VALID_TEST_CASES)
                Assert.IsTrue(DnsName.IsValid(@case.Name));
            foreach(string name in INVALID_NAMES)
                Assert.IsFalse(DnsName.IsValid(name));
        }        

        [Test]
        [ExpectedArgumentNullException]
        public void parse_throws_if_name_is_null() {

            DnsName name = DnsName.Parse(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void parse_throws_if_name_is_empty() {

            DnsName name = DnsName.Parse(string.Empty);
        }

        [Test]
        public void parse_throws_if_name_is_incorrectly_formatted() {

            foreach(string s in INVALID_NAMES) {
                Assert.Throws<DnsFormatException>(delegate {
                    DnsName name = DnsName.Parse(s);
                }, "name should be invalid: {0}", s);
            }
        }

        [Test]
        public void parse_parses_correctly_formatted_names() {

            DnsName name = null;

            foreach(TestCase @case in VALID_TEST_CASES) {
                name = DnsName.Parse(@case.Name);
                Assert.IsNotNull(name);
                Assert.AreEqual(@case.Name, name.Name);
                Assert.AreEqual(@case.Kind, name.Kind);
                Assert.AreElementsEqual(@case.Labels, name.Labels);
            }
        }

        [Test]
        public void try_parse_identifies_incorrectly_formatted_names() {

            DnsName name = null;

            foreach(string s in INVALID_NAMES) {
                Assert.IsFalse(DnsName.TryParse(s, out name), "name should be invalid: {0}", s);
                Assert.IsNull(name);
            }
        }

        [Test]
        public void try_parse_parses_correctly_formatted_names() {

            DnsName name = null;

            foreach(TestCase @case in VALID_TEST_CASES) {
                Assert.IsTrue(DnsName.TryParse(@case.Name, out name), "name should be valid: {0}", @case);
                Assert.IsNotNull(name);
                Assert.AreEqual(@case.Name, name.Name);
                Assert.AreEqual(@case.Kind, name.Kind);
                Assert.AreElementsEqual(@case.Labels, name.Labels);
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void classify_kind_throws_if_name_is_null() {

            DnsNameKind kind = DnsName.ClassifyKind(null);
        }

        [Test]
        public void classify_kind_returns_correct_kind() {

            foreach(TestCase @case in VALID_TEST_CASES)
                Assert.AreEqual(@case.Kind, DnsName.ClassifyKind(@case.Name));            
            // TODO Should this throw instead?
            Assert.AreEqual(DnsNameKind.Relative, DnsName.ClassifyKind(""));
        }

        [Test]
        public void to_string_should_return_name() {

            DnsName name = DnsName.Parse("test.com");

            Assert.AreEqual(name.Name, name.ToString());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void concat_throws_if_name_is_null() {

            DnsName name = DnsName.Parse("test.com");

            name = name.Concat(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void concat_throws_if_name_is_absolute() {

            DnsName name = DnsName.Parse("test.com.");

            Assert.AreEqual(DnsNameKind.Absolute, name.Kind);
            name = name.Concat(DnsName.Parse("uk"));
        }

        [Test]
        public void concat_returns_a_new_instance_and_does_not_modify_this() {

            DnsName name = DnsName.Parse("test");
            string fn = name.Name;
            DnsNameKind kind = name.Kind;
            IList<string> labels = name.Labels;
            DnsName newName = name.Concat(DnsName.Parse("co.uk"));

            Assert.AreSame(fn, name.Name);
            Assert.AreEqual(kind, name.Kind);
            Assert.AreSame(labels, name.Labels);

            Assert.AreNotSame(name, newName);
        }

        [Test]
        public void concat_returns_correctly_formatted_name() {

            DnsName name = DnsName.Parse("test");
            DnsName actualName = name.Concat(DnsName.Parse("co.uk"));
            DnsName expectedName = DnsName.Parse("test.co.uk");

            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedName.Name, actualName.Name);
            Assert.AreEqual(expectedName.Kind, actualName.Kind);
            Assert.AreElementsEqual(expectedName.Labels, actualName.Labels);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void is_child_of_of_throws_if_name_is_null() {

            DnsName name = DnsName.Parse("test.com.");

            name.IsChildOf(null);
        }

        [Test]
        public void is_child_of_general() {

            // Ensure it is not dependent on kind.
            Assert.IsTrue(DnsName.Parse("www.test.com").IsChildOf(DnsName.Parse("test.com")));
            Assert.IsTrue(DnsName.Parse("www.test.com").IsChildOf(DnsName.Parse("test.com.")));
            Assert.IsTrue(DnsName.Parse("www.test.com.").IsChildOf(DnsName.Parse("test.com")));
            Assert.IsTrue(DnsName.Parse("www.test.com.").IsChildOf(DnsName.Parse("test.com.")));            
            Assert.IsTrue(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse(".")));
            Assert.IsTrue(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("com.")));
            Assert.IsTrue(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("test.com.")));
            Assert.IsTrue(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("mail.test.com.")));
            Assert.IsTrue(DnsName.Parse("mx1.test.test.com.").IsChildOf(DnsName.Parse("test.test.com.")));
            
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("net.")));
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("example.com.")));
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("www.test.com.")));
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("in.mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse("mx1.test.test.com.").IsChildOf(DnsName.Parse("in.mx1.test.test.com.")));
            // A domain cannot be a sub-domain of itself.
            Assert.IsFalse(DnsName.Parse(".").IsChildOf(DnsName.Parse(".")));
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsChildOf(DnsName.Parse("mx1.mail.test.com.")));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void is_parent_of_throws_if_name_is_null() {

            DnsName name = DnsName.Parse("test.com.");

            name.IsParentOf(null);
        }

        [Test]
        public void is_parent_of_general() {

            // Ensure it is not dependent on kind.
            Assert.IsTrue(DnsName.Parse("test.com").IsParentOf(DnsName.Parse("www.test.com")));
            Assert.IsTrue(DnsName.Parse("test.com").IsParentOf(DnsName.Parse("www.test.com.")));
            Assert.IsTrue(DnsName.Parse("test.com.").IsParentOf(DnsName.Parse("www.test.com")));
            Assert.IsTrue(DnsName.Parse("test.com.").IsParentOf(DnsName.Parse("www.test.com.")));
            Assert.IsTrue(DnsName.Parse(".").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsTrue(DnsName.Parse("com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsTrue(DnsName.Parse("test.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsTrue(DnsName.Parse("mail.test.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsTrue(DnsName.Parse("test.test.com.").IsParentOf(DnsName.Parse("mx1.test.test.com.")));
            Assert.IsFalse(DnsName.Parse("net.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse("example.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse("www.test.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse("in.mx1.mail.test.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse("in.mx1.test.test.com.").IsParentOf(DnsName.Parse("mx1.test.test.com.")));
            // A domain cannot be the parent of itself.
            Assert.IsFalse(DnsName.Parse("mx1.mail.test.com.").IsParentOf(DnsName.Parse("mx1.mail.test.com.")));
            Assert.IsFalse(DnsName.Parse(".").IsParentOf(DnsName.Parse(".")));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void make_relative_throws_if_origin_is_null() {

            DnsName.Parse("test.com.").MakeRelative(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void make_relative_throws_if_this_is_not_relative_to_name() {

            DnsName.Parse("www.test.com.").MakeRelative(DnsName.Parse("example.com."));
        }

        [Test]
        public void make_relative_general() {

            // MakeRelative should not be dependent on kind.
            Assert.AreEqual(DnsName.Parse("www"), DnsName.Parse("www.test.com").MakeRelative(DnsName.Parse("test.com")));
            Assert.AreEqual(DnsName.Parse("www"), DnsName.Parse("www.test.com").MakeRelative(DnsName.Parse("test.com.")));
            Assert.AreEqual(DnsName.Parse("www"), DnsName.Parse("www.test.com.").MakeRelative(DnsName.Parse("test.com")));
            Assert.AreEqual(DnsName.Parse("www"), DnsName.Parse("www.test.com.").MakeRelative(DnsName.Parse("test.com.")));
            Assert.AreEqual(DnsName.Parse("mx1.mail.test.com."), DnsName.Parse("mx1.mail.test.com.").MakeRelative(DnsName.Parse(".")));
            Assert.AreEqual(DnsName.Parse("mx1.mail.test"), DnsName.Parse("mx1.mail.test.com.").MakeRelative(DnsName.Parse("com.")));
            Assert.AreEqual(DnsName.Parse("mx1.mail"), DnsName.Parse("mx1.mail.test.com.").MakeRelative(DnsName.Parse("test.com.")));
            Assert.AreEqual(DnsName.Parse("mx1"), DnsName.Parse("mx1.mail.test.com.").MakeRelative(DnsName.Parse("mail.test.com.")));
            Assert.AreEqual(DnsName.Parse("mx1.test.test"), DnsName.Parse("mx1.test.test.com.").MakeRelative(DnsName.Parse("com.")));
            Assert.AreEqual(DnsName.Parse("mx1.test"), DnsName.Parse("mx1.test.test.com.").MakeRelative(DnsName.Parse("test.com.")));
            Assert.AreEqual(DnsName.Parse("mx1"), DnsName.Parse("mx1.test.test.com.").MakeRelative(DnsName.Parse("test.test.com.")));            
        }

        [Test]
        public void root_field_is_root() {

            DnsName root = DnsName.Root;

            Assert.IsNotNull(root);
            Assert.AreEqual(".", root.Name);
            Assert.AreEqual(DnsNameKind.Absolute, root.Kind);
            Assert.IsEmpty(root.Labels);
        }

        public EquivalenceClassCollection<DnsName> GetEquivalenceClasses() {

            int i = 0;
            EquivalenceClass<DnsName>[] items = new EquivalenceClass<DnsName>[VALID_TEST_CASES.Length];

            foreach(TestCase @case in VALID_TEST_CASES) {                
                items[i++] = new EquivalenceClass<DnsName>(
                    // Ensure we are case-insensitive.
                    DnsName.Parse(@case.Name.ToLowerInvariant()),
                    DnsName.Parse(@case.Name.ToUpperInvariant()),
                    // Ensure that equality is not affected by kind.
                    DnsName.Parse(GetOppositeKind(@case))
                );
            }

            return new EquivalenceClassCollection<DnsName>(items);
        }

        private string GetOppositeKind(TestCase @case) {

            if(@case.Name.Equals("."))
                return @case.Name;

            return @case.Kind == DnsNameKind.Absolute ? @case.Name.TrimEnd('.') : @case.Name + ".";
        }
    }
}
