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

namespace AK.Net.Dns.Test
{
    public abstract class EnumSuperSetTest<TSuperSet, TSubset>
    {
        protected void EnsureSubsetDefinesSuperset() {

            HashSet<string> subset = new HashSet<string>(Enum.GetNames(typeof(TSubset)), StringComparer.Ordinal);
            HashSet<string> superset = new HashSet<string>(Enum.GetNames(typeof(TSuperSet)), StringComparer.Ordinal);            

            subset.ExceptWith(superset);
            if(subset.Count != 0) {
                StringBuilder message = new StringBuilder();

                message.AppendFormat("a {0} value should exist for the following {1} values:",
                    typeof(TSuperSet).Name, typeof(TSubset).Name).AppendLine();

                foreach(string missingMember in subset)
                    message.AppendLine(missingMember);

                Assert.Fail(message.ToString());
            }
        }

        protected void EnsureSubsetValuesAreEqualToSuperSetValues() {

            // TODO there must be a better way of executing this test, this is just ugly.
            
            TSuperSet superSetValue;
            string[] subsetNames = Enum.GetNames(typeof(TSubset));
            TSubset[] subsetValues = (TSubset[])Enum.GetValues(typeof(TSubset));

            for(int i = 0; i < subsetNames.Length; ++i) {
                try {
                    superSetValue = (TSuperSet)Enum.Parse(typeof(TSuperSet), subsetNames[i], false);
                } catch(ArgumentException) {
                    Assert.Fail("failed to parse a {0} member named '{1}'", typeof(TSuperSet).Name, subsetNames[i]);
                    return;
                }
                Assert.AreEqual(Convert.ToInt32(subsetValues[i]), Convert.ToInt32(superSetValue),
                    "{0}.{1} does not have the same value as {2}.{3}", typeof(TSubset).Name,
                    subsetNames[i], typeof(TSuperSet).Name, subsetNames[i]);
            }
        }
    }
}
