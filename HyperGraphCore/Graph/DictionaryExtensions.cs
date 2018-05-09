using System;
using System.Collections;
using System.Collections.Generic;

namespace Leger
{
    public static class DictionaryExtensions
    {
        public static void AddIfNotExists<T, U>(this Dictionary<T, U> dic, T key, U value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, value);
            }
        }

        public static void DeleteIfExists<T, U>(this Dictionary<T, U> dic, T key)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
            }
        }
    }
}