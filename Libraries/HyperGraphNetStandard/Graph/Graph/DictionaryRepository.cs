using System;
using System.Collections.Generic;
using System.Linq;

namespace Leger
{
    public class DictionaryRepository<T> : Dictionary<Guid, T>, IGraphObjectRepository<T> where T : IGraphObject
    {
        public T GetElementById(Guid id)
        {
            if (this.ContainsKey(id))
            {
                return this[id];
            }
            return default(T);
        }

        public void StoreIfNotExists(T v)
        {
            if (!this.ContainsKey(v.ObjectId))
            {
                this.Add(v.ObjectId, v);
            }
        }

        public void DeleteIfExists(T v)
        {
            if (this.Values.Contains(v))
            {
                Guid key = this.FirstOrDefault(a => a.Value.Equals(v)).Key;
                this.Remove(key);
            }
        }

        public new IEnumerable<T> Values { get => base.Values.AsEnumerable(); }

        public new IEnumerator<T> GetEnumerator() => this.Values.GetEnumerator();
    }
}