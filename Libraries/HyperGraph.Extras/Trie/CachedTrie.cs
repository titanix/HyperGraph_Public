using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Leger.Extra.Trie
{
    class CachedTrie : ITrie
    {
        private Trie trie = new Trie();
        private Queue<Tuple<string, List<TrieMatchResult>>> cachedResults = new Queue<Tuple<string, List<TrieMatchResult>>>();
        private uint cacheSize;

        public CachedTrie(uint cacheSize = 5)
        {
            this.cacheSize = cacheSize;
        }

        public void Insert(string str, string value)
        {
            trie.Insert(str, value);
        }

        public List<TrieMatchResult> MatchPrefix(string search)
        {
            var cachedSearch = cachedResults.Where(t => t.Item1.Equals(search));
            if (cachedResults == null)
            {
                List<TrieMatchResult> searchResult = trie.MatchPrefix(search);
                if (cachedResults.Count() == cacheSize)
                {
                    cachedResults.Dequeue();
                }
                cachedResults.Enqueue(new Tuple<string, List<TrieMatchResult>>(search, searchResult));
                return searchResult;
            }
            else
            {
                return cachedSearch.First().Item2;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return trie.GetEnumerator();
        }

        public IEnumerator<IVertex> GetEnumerator()
        {
            return trie.GetEnumerator();
        }

        public List<Tuple<Guid, Annotation>> GetAllAnnotations()
        {
            return trie.GetAllAnnotations();
        }

        public List<Annotation> GetAnnotations(IGraphObject @object)
        {
            return trie.GetAnnotations(@object);
        }
    }
}