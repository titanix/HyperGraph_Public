using System;
using System.Collections.Generic;
using System.Linq;

using Leger.IO;

namespace Leger.Extra.Trie
{
    public class Trie : ITrie
    {
        GraphObjectTypeInfo trieNode = new GraphObjectTypeInfo("682d8877-f8fd-4926-baca-2de529af6780", "Trie Node", GraphObjectType.Vertex);
        OrientedBinaryEdgeTypeInfo trieLink = new OrientedBinaryEdgeTypeInfo("e43bd63d-5aaf-42bf-a6d4-c167cd43976f", "Trie Link");
        Graph graph = new Graph();

        public Trie()
        {
            TextVertex root = new TextVertex("");
            graph.AddVertex(root);
        }

        internal Trie(Graph g)
        {
            graph = g;
        }

        // inspiré de https://en.wikipedia.org/wiki/Trie#Algorithms
        public void Insert(string str, string value)
        {
            CodePointIndexedString cpi = new CodePointIndexedString(str);
            IVertex node = graph.SearchNode("")[0];
            int i = 0;
            int stopIndex = cpi.Length - 1;

            while (i <= stopIndex)
            {
                IEdge link = GetLabeledEdge(node, cpi.AtIndex(i));
                if (link != null)
                {
                    node = link.GetLinkedObjects().Where(o => o.ObjectId != node.ObjectId).First() as IVertex;
                    if (i == stopIndex)
                    {
                        Annotation nodeValue = new Annotation() { Key = "id", Value = value };
                        graph.AddAnnotation(node, nodeValue);
                    }
                    i++;
                }
                else
                {
                    break;
                }
            }

            while (i <= stopIndex)
            {
                string currentChar = cpi.AtIndex(i);
                IVertex newNode = new TextVertex(cpi.Substring(0, i + 1));
                graph.AddVertex(newNode);
                Annotation edgeLabel = new Annotation() { Key = "label", Value = currentChar };
                IEdge edge = graph.CreateEdge(trieLink, node, newNode);
                graph.AddAnnotation(edge, edgeLabel);
                node = newNode;

                if (i == stopIndex)
                {
                    Annotation nodeValue = new Annotation() { Key = "id", Value = value };
                    graph.AddAnnotation(node, nodeValue);
                }
                i++;
            }
        }

        public List<TrieMatchResult> MatchPrefix(string search)
        {
            CodePointIndexedString cpi = new CodePointIndexedString(search);
            IVertex node = graph.SearchNode("")[0];
            int i = 0;
            int stopIndex = cpi.Length - 1;

            while (i <= stopIndex)
            {
                IEdge link = GetLabeledEdge(node, cpi.AtIndex(i));
                if (link != null)
                {
                    node = link.GetLinkedObjects().Where(o => o.ObjectId != node.ObjectId).First() as IVertex;
                    if (i == stopIndex)
                    {
                        List<TrieMatchResult> result = new List<TrieMatchResult>();
                        foreach (Annotation a in graph.GetAnnotations(node))
                        {
                            result.Add(new TrieMatchResult(node.ToString(), a.Value));
                        }
                        result.AddRange(_matchPrefix(node).ToList());
                        return result;
                    }
                    i++;
                }
                else
                {
                    return new List<TrieMatchResult>();
                }
            }
            return _matchPrefix(node).ToList();

            IEnumerable<TrieMatchResult> _matchPrefix(IVertex node_p)
            {
                foreach (IVertex tempNode in node_p.Successors(trieLink))
                {
                    IEnumerable<Annotation> annotations = graph.GetAnnotations(tempNode);
                    foreach (Annotation a in annotations)
                    {
                        yield return new TrieMatchResult(tempNode.ToString(), a.Value);
                    }
                    foreach (var v in _matchPrefix(tempNode))
                    {
                        yield return v;
                    }
                }
            }
        }

        public IEdge GetLabeledEdge(IVertex v, string s)
        {
            foreach (IEdge e in v.Links)
	        {
                Annotation a = graph.GetAnnotations(e)[0];
                if (a.Value == s)
                {
                    return e;
                }
	        }
            return null;
        }

        class TextVertex : Vertex<SerializableString>
        {
            public TextVertex(string str) : base(new GraphObjectTypeInfo("682d8877-f8fd-4926-baca-2de529af6780", "Trie Node", GraphObjectType.Vertex), new SerializableString(str)) { }
        }

        #region IGraphSerializable 
        public IEnumerator<IVertex> GetEnumerator()
        {
            return graph.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return graph.GetEnumerator();
        }

        public List<Tuple<Guid, Annotation>> GetAllAnnotations()
        {
            return graph.GetAllAnnotations();
        }

        public List<Annotation> GetAnnotations(IGraphObject @object)
        {
            return graph.GetAnnotations(@object);
        }
        #endregion
    }
}
