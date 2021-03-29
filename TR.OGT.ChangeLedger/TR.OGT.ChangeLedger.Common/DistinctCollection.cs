using System;
using System.Collections;
using System.Collections.Generic;

namespace TR.OGT.ChangeLedger.Common
{
    public class DistinctCollection<T> : IEnumerable<T>
    {
        private readonly object _lockObj = new object();
        private readonly HashSet<T> _hashSet = new HashSet<T>();

        /// <summary>
        /// Gets collection count.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lockObj)
                {
                    return _hashSet.Count;
                }
            }
        }

        /// <summary>
        /// Adds item to the collection.
        /// </summary>
        /// <param name="item">Item to be added to the colletion.</param>
        public void Add(T item)
        {
            lock (_lockObj)
            {
                if (!_hashSet.Contains(item))
                {
                    _hashSet.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds item range to collection.
        /// </summary>
        /// <param name="range">Item range to be added to the collection.</param>
        public void AddRange(IEnumerable<T> range)
        {
            lock (_lockObj)
            {
                foreach (var item in range)
                {
                    if (!_hashSet.Contains(item))
                    {
                        _hashSet.Add(item);
                    }
                }
            }
        }

        #region IEnumerable implementaion

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lockObj)
            {
                return _hashSet.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_lockObj)
            {
                return ((IEnumerable)_hashSet).GetEnumerator();
            }
        }

        #endregion
    }
}
