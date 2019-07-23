using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IGraphObjectRepository<T> where T : IGraphObject
    {
        T GetElementById(Guid id);
        void StoreIfNotExists(T o);
        int Count { get; }
        void DeleteIfExists(T o);
        IEnumerable<T> Values { get; }
        IEnumerator<T> GetEnumerator();
    }
}