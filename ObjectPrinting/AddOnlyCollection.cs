using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectPrinting
{
    public class AddOnlyCollection<T> : IAddOnlyCollection<T>
    {
        public HashSet<T> HashSet { get; set; }

        public AddOnlyCollection()
        {
            HashSet = new HashSet<T>();
        }

        public void Add(T item)
        {
            if (HashSet.Contains(item))
            {
                throw new ArgumentException("Collection already has this item");
            }
            HashSet.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return HashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}