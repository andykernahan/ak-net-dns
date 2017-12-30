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

using AK.Net.Dns;
using AK.Net.Dns.Records;

namespace AK.NDig
{    
    [Serializable]
    internal sealed class DnsRecordComparer : Comparer<DnsRecord>
    {
        #region Public Interface.

        public static readonly DnsRecordComparer Instance = new DnsRecordComparer();

        public override int Compare(DnsRecord x, DnsRecord y) {            

            if(x == y)
                return 0;
            if(x == null)
                return -1;
            if(y == null)
                return 1;

            int res = 0;

            if((res = x.Type.ToString().CompareTo(y.Type.ToString())) == 0 &&
                (res = x.Owner.CompareTo(y.Owner)) == 0) {
                switch(x.Type) {
                    case DnsRecordType.NS:
                        res = ((NSRecord)x).Domain.CompareTo(((NSRecord)y).Domain);
                        break;
                    case DnsRecordType.CName:
                        res = ((CNameRecord)x).Canonical.CompareTo(((CNameRecord)y).Canonical);
                        break;
                    case DnsRecordType.DN:
                        res = ((DNRecord)x).Target.CompareTo(((DNRecord)y).Target);
                        break;
                    case DnsRecordType.Ptr:
                        res = ((PtrRecord)x).Domain.CompareTo(((PtrRecord)y).Domain);
                        break;
                    case DnsRecordType.HInfo:
                        HInfoRecord xHInfo = (HInfoRecord)x;
                        HInfoRecord yHInfo = (HInfoRecord)y;

                        if((res = xHInfo.Cpu.CompareTo(yHInfo.Cpu)) == 0)
                            res = xHInfo.Os.CompareTo(yHInfo.Os);
                        break;
                    case DnsRecordType.MInfo:
                        MInfoRecord xMInfo = (MInfoRecord)x;
                        MInfoRecord yMInfo = (MInfoRecord)y;

                        if((res = xMInfo.RMbox.CompareTo(yMInfo.RMbox)) == 0)
                            res = xMInfo.EMbox.CompareTo(yMInfo.EMbox);
                        break;
                    case DnsRecordType.MX:
                        MXRecord xMx = (MXRecord)x;
                        MXRecord yMx = (MXRecord)y;

                        if((res = xMx.CompareTo(yMx)) == 0)
                            res = xMx.Exchange.CompareTo(yMx.Exchange);
                        break;
                    case DnsRecordType.MB:
                        res = ((MBRecord)x).Mailbox.CompareTo(((MBRecord)y).Mailbox);
                        break;
                    case DnsRecordType.MG:
                        res = ((MGRecord)x).Mailbox.CompareTo(((MGRecord)y).Mailbox);
                        break;
                    case DnsRecordType.MR:
                        res = ((MRRecord)x).NewMailbox.CompareTo(((MRRecord)y).NewMailbox);
                        break;
                    case DnsRecordType.Txt:
                        res = ((TxtRecord)x).Text.CompareTo(((TxtRecord)y).Text);
                        break;
                    case DnsRecordType.Spf:
                        res = ((SpfRecord)x).Specification.CompareTo(((SpfRecord)y).Specification);
                        break;
                    case DnsRecordType.Srv:
                        SrvRecord xSrv = (SrvRecord)x;
                        SrvRecord ySrv = (SrvRecord)y;

                        if((res = xSrv.CompareTo(ySrv)) == 0)
                            res = xSrv.Target.CompareTo(ySrv.Target);                        
                        break;
                    default:
                        res = 0;
                        break;
                }
            }

            return res;
        }

        #endregion
    }
}
