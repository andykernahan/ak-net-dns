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

namespace AK.Net.Dns.Records
{
    /// <summary>
    /// A DNS SRV [RFC2782] record which specifies the location of the
    /// service identified by the owner name.
    /// </summary>    
    [Serializable]
    public class SrvRecord : DnsRecord, IComparable<SrvRecord>, IComparable
    {
        #region Private Fields.

        private int _port;
        private int _weight;        
        private int _priority;        
        private DnsName _target;        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of SrvRecord records. This field is readonly.
        /// </summary>
        new public static readonly SrvRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the SrvRecord class and specifies the owner name,
        /// the resource record class, the TTL of the record and the reply reader from
        /// which the RDLENGTH and RDATA section of the resource record will be read.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="reader">The reply reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="reader"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the RDATA section of the record could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public SrvRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, IDnsReader reader)
            : base(owner, DnsRecordType.Srv, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _priority = reader.ReadUInt16();
            _weight = reader.ReadUInt16();
            _port = reader.ReadUInt16();
            _target = reader.ReadName();
        }

        /// <summary>
        /// Initialises a new instance of the SrvRecord class and specifies the owner name,
        /// the resource record class, the TTL, priority, weight, port and location of the
        /// service.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="priority">The service priority.</param>
        /// <param name="weight">The weight of the service over other services with the
        /// same priority.</param>
        /// <param name="port">The service port.</param>
        /// <param name="target">The service location.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> or <paramref name="target"/> is
        /// <see langword="null"/>.
        /// </exception>
        public SrvRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, int priority, int weight,
            int port, DnsName target)
            : base(owner, DnsRecordType.Srv, cls, ttl) {

            Guard.NotNull(target, "target");
            
            _priority = priority;
            _weight = weight;
            _port = port;
            _target = target;
        }

        /// <summary>
        /// Makes an SRV query name for the specified <paramref name="service"/>,
        /// <paramref name="protocol"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="service">The requested service.</param>
        /// <param name="protocol">The requested protocol.</param>
        /// <param name="name">The name.</param>
        /// <returns>The SRV query name for the specified <paramref name="service"/>,
        /// <paramref name="protocol"/> and <paramref name="name"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="service"/>, <paramref name="protocol"/> or
        /// <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when the format of the resultant name is invalid because of:
        /// <list type="bullet">
        /// <item><paramref name="service"/> contains invalid characters</item>
        /// <item>the length <paramref name="service"/> is greater than the maximum label
        /// length</item>
        /// <item><paramref name="protocol"/> contains invalid characters</item>
        /// <item>the length <paramref name="protocol"/> is greater than the maximum label
        /// length</item>
        /// <item>the length of the resultant name is greater than 
        /// <see cref="AK.Net.Dns.DnsName.MaxLength"/></item>
        /// </list>
        /// </exception>
        public static DnsName MakeName(string service, string protocol, DnsName name) {

            Guard.NotNull(service, "service");
            Guard.NotNull(protocol, "protocol");
            Guard.NotNull(name, "name");

            return DnsName.Parse(string.Format("_{0}._{1}.{2}", service, protocol, name.Name));
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            writer.WriteUInt16(this.Priority);
            writer.WriteUInt16(this.Weight);
            writer.WriteUInt16(this.Port);
            writer.WriteName(this.Target);
        }

        /// <summary>
        /// Returns a value indicating relative eqaulity with the 
        /// <paramref name="other"/> instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>A value indicating relative eqaulity with the 
        /// <paramref name="other"/> instance.</returns>
        public int CompareTo(SrvRecord other) {

            if(other == null)
                return 1;

            int res = this.Priority.CompareTo(other.Priority);

            return res != 0 ? res : other.Weight.CompareTo(this.Weight);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1,-2} {2,-2} {3,-2} {4,-2}", base.ToString(),
                this.Priority, this.Weight, this.Port, this.Target);
        }

        /// <summary>
        /// Gets or sets the service location.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Target {

            get { return _target; }
            set {
                Guard.NotNull(value, "value");
                _target = value;
            }
        }

        /// <summary>
        /// Gets or sets the priority of the service. Services with a lower
        /// priority value are more preferable.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid
        /// <see cref="System.UInt16"/> value.
        /// </exception>
        public int Priority {

            get { return _priority; }
            set {
                Guard.IsUInt16(value, "value");
                _priority = value;
            }
        }

        /// <summary>
        /// Gets or sets the weight of the service over other services with the
        /// same priority. Services with a higher weight value are more preferable.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid
        /// <see cref="System.UInt16"/> value.
        /// </exception>
        public int Weight {

            get { return _weight; }
            set {
                Guard.IsUInt16(value, "value");
                _weight = value;
            }
        }

        /// <summary>
        /// Gets or sets the service port.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not a valid
        /// <see cref="System.UInt16"/> value.
        /// </exception>
        public int Port {

            get { return _port; }
            set {
                Guard.IsUInt16(value, "value");
                _port = value;
            }
        }

        #endregion

        #region Explicit Interface.

        int IComparable.CompareTo(object obj) {

            return CompareTo(obj as SrvRecord);            
        }

        #endregion
    }
}
