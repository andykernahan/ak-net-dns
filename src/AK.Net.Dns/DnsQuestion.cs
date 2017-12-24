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

namespace AK.Net.Dns
{
    /// <summary>
    /// Represents a DNS question.
    /// </summary>
    [Serializable]
    public class DnsQuestion : IEquatable<DnsQuestion>
    {
        #region Private Fields.

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the DnsQuestion class and specifies
        /// the class of query, the query type and the domain being queried.
        /// </summary>
        /// <param name="name">The domain to query.</param>
        /// <param name="type">The type of query.</param>
        /// <param name="cls">The class of the query.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public DnsQuestion(DnsName name, DnsQueryType type, DnsQueryClass cls)
        {
            Guard.NotNull(name, "name");

            Class = cls;
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Initialises a new instance of the DnsQuestion class and the reply
        /// reader from which the question will be decoded.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when question data could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public DnsQuestion(IDnsReader reader)
        {
            Guard.NotNull(reader, "reader");

            try
            {
                Name = reader.ReadName();
                Type = reader.ReadQueryType();
                Class = reader.ReadQueryClass();
            }
            catch (DnsException exc)
            {
                throw Guard.InvalidDnsQuestionFormat(exc);
            }
        }

        /// <summary>
        /// Writes this question using the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Write(IDnsWriter writer)
        {
            Guard.NotNull(writer, "writer");

            writer.WriteName(Name);
            writer.WriteQueryType(Type);
            writer.WriteQueryClass(Class);
        }

        /// <summary>
        /// Returns a value indicating indicating equality with the specified
        /// question.
        /// </summary>
        /// <param name="other">The question to compare with this instance.</param>
        /// <returns><see langword="true"/> if equal, otherwise;
        /// <see langword="false"/>.</returns>
        public bool Equals(DnsQuestion other)
        {
            return other != null &&
                   other.Type == Type &&
                   other.Class == Class &&
                   other.Name.Equals(Name);
        }

        /// <summary>
        /// Returns a value indicating indicating equality with the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if equal, otherwise;
        /// <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsQuestion);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Class.GetHashCode() ^
                   Type.GetHashCode() ^
                   Name.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return DnsUtility.Format("{0,-32} {1,-6} {2,-6}", Name,
                DnsUtility.ToString(Class), DnsUtility.ToString(Type));
        }

        /// <summary>
        /// Gets the class of query.
        /// </summary>
        public DnsQueryClass Class { get; }

        /// <summary>
        /// Gets the type of query.
        /// </summary>
        public DnsQueryType Type { get; }

        /// <summary>
        /// Gets the domain name which is being queried.
        /// </summary>
        public DnsName Name { get; }

        #endregion
    }
}
