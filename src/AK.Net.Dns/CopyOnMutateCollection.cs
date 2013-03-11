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

namespace AK.Net.Dns
{
    /// <summary>
    /// Provides a collection whereby the invocation of any mutative methods
    /// causes the underlying list to be copied, replaced and then the operation
    /// performed. This strategy allows the collection to be mutated without
    /// breaking concurrent enumerations. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">The collection element type.</typeparam>
    /// <remarks>    
    /// Whilst this collection provides thread safe enumeration, the remaining
    /// methods are not thread safe and appropiate locks must be taken to ensure
    /// correctness.
    /// <para>
    /// This collection should only be used if the following conditions are met:
    /// <list type="bullet">
    /// <item>
    /// modifications to the collection are rarely made as it is expensive to make
    /// a new copy of the existing list each time
    /// </item>
    /// <item>
    /// it is <b>not</b> required that existing enumerations break after mutation
    /// </item>    
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class CopyOnMutateCollection<T> : IList<T>
    {
        #region Private Fields.

        private volatile List<T> _items;        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the CopyOnMutateCollection&lt;T&gt; class.
        /// </summary>
        public CopyOnMutateCollection() {

            _items = new List<T>();            
        }

        /// <summary>
        /// Initialises a new instance of the CopyOnMutateCollection&lt;T&gt; class.
        /// </summary>
        /// <param name="collection">The collection to copy to the new collection.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="collection"/> is <see langword="null"/>.
        /// </exception>
        public CopyOnMutateCollection(IEnumerable<T> collection) {            

            _items = new List<T>(collection);            
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified
        /// <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The zero-based index of the item, otherwise; -1,</returns>
        public int IndexOf(T item) {

            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified <paramref name="item"/> into this collection at the
        /// specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index to insert the <paramref name="item"/>.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less that zero or greater than
        /// <see cref="AK.Net.Dns.CopyOnMutateCollection&lt;T&gt;.Count"/>.
        /// </exception>
        public void Insert(int index, T item) {

            NewCopy().Insert(index, item);
        }

        /// <summary>
        /// Removes an item from this collection at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less that zero or greater than or equal to
        /// <see cref="AK.Net.Dns.CopyOnMutateCollection&lt;T&gt;.Count"/>.
        /// </exception>
        public void RemoveAt(int index) {

            NewCopy().RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets an item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less that zero or greater than or equal to
        /// <see cref="AK.Net.Dns.CopyOnMutateCollection&lt;T&gt;.Count"/>.
        /// </exception>
        public T this[int index] {

            get { return _items[index]; }
            set { NewCopy()[index] = value; }
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to the end of this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item) {

            NewCopy().Add(item);
        }

        /// <summary>
        /// Clears all items from this collection.
        /// </summary>
        public void Clear() {

            _items = new List<T>();
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="item"/> is
        /// contained within this collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is contained
        /// within this collection, otherwise; <see langword="false"/>.</returns>
        public bool Contains(T item) {

            return _items.Contains(item);
        }

        /// <summary>
        /// Copies this collection to the specified <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) {

            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of items in this collection.
        /// </summary>
        public int Count {

            get { return _items.Count; }
        }

        /// <summary>
        /// Returns a value indicating if this collection is readonly.
        /// </summary>
        public bool IsReadOnly {

            get { return false; }
        }

        /// <summary>
        /// Removes the specified <paramref name="item"/> from this collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is was removed
        /// from this collection, otherwise; <see langword="false"/>.</returns>
        public bool Remove(T item) {
            
            return NewCopy().Remove(item);
        }

        /// <summary>
        /// Returns an enumerator for this collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>
        /// for the items within this collection.
        /// </returns>
        /// <remarks>The returned enumerator will <b>not</b> throw an exception
        /// if this collection is modified during enumeration.</remarks>
        public IEnumerator<T> GetEnumerator() {

            return _items.GetEnumerator();            
        }

        #endregion

        #region Explicit Interface.

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {

            return GetEnumerator();
        }

        #endregion

        #region Private Impl.
        
        private List<T> NewCopy() {

            List<T> newItems = new List<T>(_items);

            _items = newItems;

            return newItems;
        }

        #endregion
    }
}
