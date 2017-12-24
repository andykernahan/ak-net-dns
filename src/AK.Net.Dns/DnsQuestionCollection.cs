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
    /// Represents a unique collection of <see cref="AK.Net.Dns.DnsQuestion"/>
    /// instances.
    /// </summary>
    [Serializable]
    public class DnsQuestionCollection : Collection<DnsQuestion>
    {
        #region Public Interface.

        /// <summary>
        /// Reads the specified number of questions from the specified reader.
        /// </summary>
        /// <param name="reader">The query reader.</param>
        /// <param name="count">The number of questions to parse.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when one of the questions could not be read from the
        /// <paramref name="reader"/>.
        /// </exception>
        public void Read(IDnsReader reader, int count)
        {
            Guard.NotNull(reader, "reader");
            Guard.InRange(count >= 0, "count");

            ClearItems();
            while (count-- > 0)
            {
                Add(new DnsQuestion(reader));
            }
        }

        /// <summary>
        /// Writes this collection of questions using the specified
        /// <see cref="AK.Net.Dns.IDnsWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void Write(IDnsWriter writer)
        {
            Guard.NotNull(writer, "writer");

            foreach (var question in this)
            {
                question.Write(writer);
            }
        }

        /// <summary>
        /// Returns a value indicating if the two collections are considered equal.
        /// </summary>
        /// <param name="left">The first collection.</param>
        /// <param name="right">The second collection.</param>
        /// <returns><see langword="true"/> if the collections are considered equal,
        /// otherwise; <see langword="false"/>.</returns>
        public static bool Equals(DnsQuestionCollection left, DnsQuestionCollection right)
        {
            if (left == right)
            {
                return true;
            }
            if (left == null || right == null)
            {
                return false;
            }
            if (left.Count != right.Count)
            {
                return false;
            }

            for (var i = 0; i < left.Count; ++i)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Inserts an item into this collection at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        protected override void InsertItem(int index, DnsQuestion item)
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
        protected override void SetItem(int index, DnsQuestion item)
        {
            ValidateItem(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Validates the specified item
        /// </summary>
        /// <param name="item">The question to item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the specified <paramref name="item"/> already exists within
        /// this collection.
        /// </exception>
        protected virtual void ValidateItem(DnsQuestion item)
        {
            Guard.NotNull(item, "item");
            if (Contains(item))
            {
                throw Guard.DnsQuestionAlreadyAMember("item");
            }
        }

        #endregion
    }
}
