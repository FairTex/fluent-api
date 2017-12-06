using System.Collections.Generic;

namespace ObjectPrinting
{
    public interface IAddOnlyCollection<T> : IEnumerable<T>
    {
        void Add(T item);
    }
}