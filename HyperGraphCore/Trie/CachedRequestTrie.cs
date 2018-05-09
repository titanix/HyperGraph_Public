using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Leger
{
    class CachedRequestTrie : ITrie
    {
        Trie trie = new Trie();
        Queue<Tuple<string, List<TrieMatchResult>>> cachedResults = new Queue<Tuple<string, List<TrieMatchResult>>>();
        uint cacheSize;

        public CachedRequestTrie(uint cacheSize = 5)
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

        #region Graph Interface
        public bool AddVertex(IVertex node)
        {
            return trie.AddVertex(node);
        }

        public IEdge CreateEdge(GraphObjectTypeInfo type, params IGraphObject[] targets)
        {
            return trie.CreateEdge(type, targets);
        }

        public IEdge CreateEdge(GraphObjectTypeInfo type, Guid publicId, params IGraphObject[] targets)
        {
            return trie.CreateEdge(type, publicId, targets);
        }

        public bool RemoveVertex(IVertex node)
        {
            return trie.RemoveVertex(node);
        }

        public void RemoveAnnotation(IGraphObject target, Annotation annotation)
        {
            trie.RemoveAnnotation(target, annotation);
        }

        public bool RemoveEdge(IEdge edge)
        {
            return trie.RemoveEdge(edge);
        }

        public void AddAnnotation(IGraphObject target, Annotation annotation)
        {
            trie.AddAnnotation(target, annotation);
        }

        public void AddAnnotations(IGraphObject target, List<Annotation> annotations)
        {
            trie.AddAnnotations(target, annotations);
        }

        public List<Tuple<Guid, Annotation>> GetAllAnnotations()
        {
            return trie.GetAllAnnotations();
        }

        public System.Collections.Immutable.IImmutableList<Annotation> GetAnnotations(IGraphObject @object)
        {
            return trie.GetAnnotations(@object);
        }

        public List<IVertex> SearchNode(string indexName, string searchString)
        {
            return trie.SearchNode(indexName, searchString);
        }

        public List<string> GetStringIndex(string indexName)
        {
            return trie.GetStringIndex(indexName);
        }

        public void RebuildIndexes()
        {
            trie.RebuildIndexes();
        }

        public void RebuildIndexes(Func<IVertex, IEnumerable<IndexedString>> customLogic)
        {
            trie.RebuildIndexes(customLogic);
        }

        public IEnumerator<IVertex> GetEnumerator()
        {
            return trie.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return trie.GetEnumerator();
        }
        #endregion
    }
}
