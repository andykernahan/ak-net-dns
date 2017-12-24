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
using System.Collections.ObjectModel;

namespace AK.Net.Dns
{
    /// <summary>
    /// Represents a collection of <see cref="AK.Net.Dns.DnsRecord"/>
    /// derived instances.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="AK.Net.Dns.DnsRecord"/>
    /// </typeparam>
    [Serializable]
    public class DnsRecordCollection<T> : Collection<T> where T : DnsRecord
    {
        #region Public Interface.

        /// <summary>
        /// Writes this collection of records using the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public void Write(IDnsWriter writer)
        {
            Guard.NotNull(writer, "writer");

            foreach (var record in this)
            {
                writer.WriteRecord(record);
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected override void InsertItem(int index, T item)
        {
            ValidateItem(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces the element at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The index of the element to replace.</param>
        /// <param name="item">The replacement item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected override void SetItem(int index, T item)
        {
            ValidateItem(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Validates the specified item.
        /// </summary>
        /// <param name="item">The question to validate.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void ValidateItem(T item)
        {
            Guard.NotNull(item, "item");
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of <see cref="AK.Net.Dns.DnsRecord"/>
    /// instances.
    /// </summary>
    [Serializable]
    public class DnsRecordCollection : DnsRecordCollection<DnsRecord>
    {
        #region Public Interface.

        #endregion
    }
}
