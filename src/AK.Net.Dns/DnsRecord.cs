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
using AK.Net.Dns.Records.Builders;

namespace AK.Net.Dns
{
    /// <summary>
    /// Defines a base class for a DNS resource record. This class is 
    /// <see langword="abstract"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [Serializable]
    public abstract class DnsRecord
    {
        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the DnsRecord class and specifies the
        /// owner name, the resource record class and the TTL of the record.
        /// </summary>
        /// <param name="owner">The owner name.</param>
        /// <param name="type">The class of resource record.</param>
        /// <param name="cls">The class of resource record.</param>
        /// <param name="ttl">The TTL.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="owner"/> is <see langword="null"/>.
        /// </exception>
        protected DnsRecord(DnsName owner, DnsRecordType type, DnsRecordClass cls, TimeSpan ttl)
        {
            Guard.NotNull(owner, "owner");

            Owner = owner;
            Type = type;
            Class = cls;
            Ttl = ttl;
            Expires = DnsClock.Now() + ttl;
        }

        #endregion

        #region Private Fields.

        private static readonly IList<IDnsRecordBuilder> _builders;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the maximum allowable record type value. This field is constant.
        /// </summary>
        public const int MaxRecordType = ushort.MaxValue;

        /// <summary>
        /// When overriden in a dervied class; the RDATA section of the resource
        /// record is written to the specified <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public abstract void WriteData(IDnsWriter writer);

        /// <summary>
        /// Defines an empty array of <see cref="DnsRecord"/> records. This field is
        /// readonly.
        /// </summary>
        public static readonly DnsRecord[] EmptyArray = { };

        /// <summary>
        /// DnsRecord class constructor.
        /// </summary>
        static DnsRecord()
        {
            _builders = new CopyOnMutateCollection<IDnsRecordBuilder>(
                new IDnsRecordBuilder[] {DnsNativeRecordBuilder.Instance}
            );
        }

        /// <summary>
        /// Returns a <see cref="AK.Net.Dns.IDnsRecordBuilder"/> capable of building
        /// a <see cref="AK.Net.Dns.DnsRecord"/> of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of record to return a builder for.
        /// </param>
        /// <returns>
        /// The builder for the specified <paramref name="type"/>. If no exact
        /// builder exists; the default record builder is injected.
        /// </returns>
        public static IDnsRecordBuilder GetBuilder(DnsRecordType type)
        {
            return GetBuilder(type, true);
        }

        /// <summary>
        /// Returns a <see cref="AK.Net.Dns.IDnsRecordBuilder"/> capable of building
        /// a <see cref="AK.Net.Dns.DnsRecord"/> of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of record to return a builder for.
        /// </param>
        /// <param name="injectDefault">
        /// <see langword="true"/> to inject the default builder if no builder exists
        /// for the specified <paramref name="type"/>, otherwise; <see langword="false"/>.
        /// </param>
        /// <returns>
        /// The builder for the specified <paramref name="type"/>. If no builder exists
        /// and <paramref name="injectDefault"/> is <see langword="false"/>;
        /// <see langword="null"/> is returned.
        /// </returns>
        public static IDnsRecordBuilder GetBuilder(DnsRecordType type, bool injectDefault)
        {
            foreach (var builder in _builders)
            {
                if (builder.CanBuild(type))
                {
                    return builder;
                }
            }

            return injectDefault ? DnsDefaultRecordBuilder.Instance : null;
        }

        /// <summary>
        /// Adds the specified <see cref="AK.Net.Dns.IDnsRecordBuilder"/> to the
        /// internal collection of builders.
        /// </summary>
        /// <param name="builder">The builder to add.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static void AddBuilder(IDnsRecordBuilder builder)
        {
            Guard.NotNull(builder, "builder");

            lock (_builders)
            {
                if (!_builders.Contains(builder))
                {
                    _builders.Add(builder);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0,-25} {1,-6} {2,-6} {3,-6}",
                Owner, DnsUtility.ToString(Ttl), DnsUtility.ToString(Class),
                DnsUtility.ToString(Type));
        }

        /// <summary>
        /// Gets the name of the DNS node to which this resource record relates.
        /// </summary>
        public DnsName Owner { get; }

        /// <summary>
        /// Gets the type of this resource record.
        /// </summary>
        public DnsRecordType Type { get; }

        /// <summary>
        /// Gets the class of this resource record.
        /// </summary>
        public DnsRecordClass Class { get; }

        /// <summary>
        /// Gets the Time To Live (TTL) value.
        /// </summary>
        public TimeSpan Ttl { get; }

        /// <summary>
        /// Gets the date time at which this resource records expires.
        /// </summary>
        public DateTime Expires { get; }

        /// <summary>
        /// Returns a value indicating if this resource record is alive.
        /// </summary>
        public bool IsAlive()
        {
            return IsAlive(DnsClock.Now());
        }

        /// <summary>
        /// Returns a value indicating if this resource record is alive.
        /// </summary>
        /// <param name="dateTime">The current date time.</param>
        public bool IsAlive(DateTime dateTime)
        {
            return dateTime <= Expires;
        }

        #endregion
    }
}
