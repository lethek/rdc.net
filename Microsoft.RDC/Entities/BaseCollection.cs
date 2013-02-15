using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.RDC
{
    public class BaseCollection<T> : System.Collections.CollectionBase
    {
        public BaseCollection() : base() { }

        /// <summary>
        /// Gets or sets the value of the object at a specific position in the collection.
        /// </summary>
        public T this[int index]
        {
            get
            {
                return ((T)(this.List[index]));
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Append a Class entry to this collection.
        /// </summary>
        /// <param name="value">Class instance</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(T value)
        {
            if (!this.List.Contains(value))
            {
                return this.List.Add(value);
            }
            else
            {
                // return index of existing item; we don't support dups.
                return this.List.IndexOf(value);
            }
        }

        /// <summary>
        /// Determines whether a specified Class instance is in this collection.
        /// </summary>
        /// <param name="value">Class instance to search for.</param>
        /// <returns>True if the Class instance is in the collection; otherwise false.</returns>
        public bool Contains(T value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Retrieve the index a specified Class instance is in this collection.
        /// </summary>
        /// <param name="value">Class instance to find</param>
        /// <returns>The zero-based index of the specified Class instance. If the DataSystem is not found, the return value is -1.</returns>
        public int IndexOf(T value)
        {
            return this.List.IndexOf(value);
        }       

        /// <summary>
        /// Removes a specified Class instance from this collection.
        /// </summary>
        /// <param name="value">The Class instance to remove.</param>
        public void Remove(T value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the Class instance.
        /// </summary>
        /// <returns>An Service's enumerator.</returns>
        public new BaseCollectionEnumerator GetEnumerator()
        {
            return new BaseCollectionEnumerator(this);
        }

        /// <summary>
        /// Insert a Class instance into this collection at a specified index.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <param name="value">The Class instance to insert.</param>
        public void Insert(int index, T value)
        {
            this.List.Insert(index, value);
        }

        #region class DataSystemCollectionEnumerator
        /// <summary>
        /// Strongly typed enumerator of Service.
        /// </summary>
        public class BaseCollectionEnumerator : object, System.Collections.IEnumerator
        {
            private int index;
            private object currentElement;
            //private BaseCollection<T> collection;
            private CollectionBase collection;

            /// <summary>
            /// Default constructor for enumerator.
            /// </summary>
            /// <param name="collection">Instance of the collection to enumerate.</param>
            internal BaseCollectionEnumerator(CollectionBase collection)
            {
                index = -1;
                this.collection = collection;
            }

            /// <summary>
            /// Gets the object in the enumerated BaseCollection currently indexed by this instance.
            /// </summary>
            public object Current
            {
                get
                {
                    if (((index == -1) || (index >= collection.Count)))
                    {
                        throw new System.IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return currentElement;
                    }
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (((index == -1) || (index >= collection.Count)))
                    {
                        throw new System.IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return currentElement;
                    }
                }
            }

            /// <summary>
            /// Reset the cursor, so it points to the beginning of the enumerator.
            /// </summary>
            public void Reset()
            {
                index = -1;
                currentElement = null;
            }

            /// <summary>
            /// Advances the enumerator to the next queue of the enumeration, if one is currently available.
            /// </summary>
            /// <returns>true, if the enumerator was succesfully advanced to the next queue; false, if the enumerator has reached the end of the enumeration.</returns>
            public bool MoveNext()
            {
                if ((index < (collection.Count - 1)))
                {
                    index = (index + 1);
                    currentElement = ((BaseCollection<T>)collection)[index];
                    return true;
                }
                index = collection.Count;
                return false;
            }
        }
        #endregion

    }
}
