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
    /// A DNS Start of Zone Authority (SOA) [RFC1035] record which contains
    /// information specific to a particular zone.
    /// </summary>
    [Serializable]
    public class SoaRecord : DnsRecord
    {
        #region Private Fields.

        private DnsName _master;
        private DnsName _rMbox;
        private long _serial;
        private TimeSpan _refresh;
        private TimeSpan _retry;
        private TimeSpan _expire;
        private TimeSpan _minimum;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty array of SoaRecord records. This field is readonly.
        /// </summary>
        new public static readonly SoaRecord[] EmptyArray = { };

        /// <summary>
        /// Initialises a new instance of the SoaRecord class and specifies the owner name,
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
        public SoaRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl,
            IDnsReader reader)
            : base(owner, DnsRecordType.Soa, cls, ttl) {

            Guard.NotNull(reader, "reader");

            // Skip the RDLENGTH.
            reader.ReadUInt16();
            _master = reader.ReadName();
            _rMbox = reader.ReadName();
            _serial = reader.ReadUInt32();
            _refresh = reader.ReadTtl();
            _retry = reader.ReadTtl();
            _expire = reader.ReadTtl();
            _minimum = reader.ReadTtl();
        }

        /// <summary>
        /// Initialises a new instance of the SoaRecord class.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <param name="master">The domain name of the name server that is the master
        /// of the data.</param>
        /// <param name="rMbox">The domain name which specifies the mailbox responsible
        /// for the domain.</param>
        /// <param name="serial">The version number of the zone to which the name
        /// relates.</param>
        /// <param name="refresh">The interval at which the zone refreshed.</param>
        /// <param name="retry">The interval at which a failed domain refresh should
        /// be retried.</param>
        /// <param name="expire">The upper limit on the time interval that can elapse
        /// before the zone is no
        /// longer authoritative.</param>
        /// <param name="minimum">The minimum TTL value for any resource record exported
        /// by the zone.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/>, <paramref name="master"/> or
        /// <paramref name="rMbox"/> is <see langword="null"/>.
        /// </exception>
        public SoaRecord(DnsName owner, DnsRecordClass cls, TimeSpan ttl, DnsName master, DnsName rMbox,
            long serial, TimeSpan refresh, TimeSpan retry, TimeSpan expire, TimeSpan minimum)
            : base(owner, DnsRecordType.Soa, cls, ttl) {

            Guard.NotNull(master, "master");
            Guard.NotNull(rMbox, "rMbox");

            _master = master;
            _rMbox = rMbox;
            _serial = serial;
            _refresh = refresh;
            _retry = retry;
            _expire = expire;
            _minimum = minimum;
        }

        /// <summary>
        /// Writes the RDATA of this resource record to the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteData(IDnsWriter writer) {

            Guard.NotNull(writer, "writer");

            writer.WriteName(this.Master);
            writer.WriteName(this.RMbox);
            writer.WriteUInt32(this.Serial);
            writer.WriteTtl(this.Refresh);
            writer.WriteTtl(this.Retry);
            writer.WriteTtl(this.Expire);
            writer.WriteTtl(this.Minimum);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return DnsUtility.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                base.ToString(), this.Master, this.RMbox, this.Serial, DnsUtility.ToString(this.Refresh),
                DnsUtility.ToString(this.Retry), DnsUtility.ToString(this.Expire), DnsUtility.ToString(this.Minimum));

        }

        /// <summary>
        /// Gets or sets the domain name of the name server that is the master of the data.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName Master {

            get { return _master; }
            set {
                Guard.NotNull(value, "value");
                _master = value;
            }
        }

        /// <summary>
        /// Gets or sets the domain name which specifies the mailbox that is responsible
        /// for the zone.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public DnsName RMbox {

            get { return _rMbox; }
            set {
                Guard.NotNull(value, "value");
                _rMbox = value;
            }
        }

        /// <summary>
        /// Gets or sets the version number of the zone to which the name relates.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative or greater than
        /// <see cref="System.UInt32.MaxValue"/>.
        /// </exception>
        public long Serial {

            get { return _serial; }
            set {
                Guard.IsUInt32(value, "value");
                _serial = value;
            }
        }

        /// <summary>
        /// Gets or sets the interval at which the zone refreshed.
        /// </summary>
        public TimeSpan Refresh {

            get { return _refresh; }
            set { _refresh = value; }
        }

        /// <summary>
        /// Gets or sets the interval at which a failed domain refresh should be retried.
        /// </summary>
        public TimeSpan Retry {

            get { return _retry; }
            set { _retry = value; }
        }

        /// <summary>
        /// Gets or sets the upper limit on the time interval that can elapse before the zone
        /// is no longer authoritative
        /// </summary>
        public TimeSpan Expire {

            get { return _expire; }
            set { _expire = value; }
        }

        /// <summary>
        /// Gets or sets the minimum TTL value for any resource record exported by the zone.
        /// </summary>
        public TimeSpan Minimum {

            get { return _minimum; }
            set { _minimum = value; }
        }

        #endregion
    }
}
