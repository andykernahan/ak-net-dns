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
    [TestsOn(typeof(DnsResolutionException))]
    [VerifyExceptionContract(typeof(DnsResolutionException))]
    public class DnsResolutionExceptionTest
    {
        [Test]
        public void response_code_is_null_unless_specified() {

            DnsResolutionException exc;

            exc = new DnsResolutionException();
            Assert.IsNull(exc.ResponseCode);
            exc = new DnsResolutionException("message");
            Assert.IsNull(exc.ResponseCode);
            exc = new DnsResolutionException("message", new Exception());
            Assert.IsNull(exc.ResponseCode);
        }

        [Test]
        public void ctor_should_set_message_and_response_code() {

            string message = "message";
            DnsResponseCode responseCode = DnsResponseCode.NameError;
            DnsResolutionException exc = new DnsResolutionException(message, responseCode);

            Assert.AreSame(exc.Message, message);            
            Assert.AreEqual(exc.ResponseCode, responseCode);
        }
    }
}
