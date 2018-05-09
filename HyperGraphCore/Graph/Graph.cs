using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Leger
{
    public class Graph : IGraph
    {
        Dictionary<Guid, IVertex> verticesInstances = new Dictionary<Guid, IVertex>();

        Dictionary<Guid, IEdge> edgeInstances = new Dictionary<Guid, IEdge>();

        HashSet<GraphObjectTypeInfo> graphTypes = new HashSet<GraphObjectTypeInfo>();

        ConditionalWeakTable<IGraphObject, List<Annotation>> annotationsTable = new ConditionalWeakTable<IGraphObject, List<Annotation>>();

        bool indexDirty = true;

        Dictionary<string, Dictionary<string, List<IVertex>>> globalIndexes = new Dictionary<string, Dictionary<string, List<IVertex>>>();

        Func<IVertex, IEnumerable<IndexedString>> indexBuildingLogic;

        public Graph(Func<IVertex, IEnumerable<IndexedString>> indexConstructionLogic = null)
        {
            if (indexConstructionLogic == null)
            {
                this.indexBuildingLogic = v => { return v.IndexableStrings; };
            }
            else
            {
                this.indexBuildingLogic = indexConstructionLogic;
            }
        }

        public int VerticesNumber { get { return verticesInstances.Count; } }

        public int EdgesNumber { get { return edgeInstances.Count; } }

        public List<GraphObjectTypeInfo> RegisteredTypes
        {
            get
            {
                return graphTypes.ToList(); // on ne veut pas renvoyer la liste réélement utilisée par la classe
            }
        }

        public IGraphObject GetElementById(Guid id)
        {
            if(verticesInstances.ContainsKey(id))
            {
                return verticesInstances[id];
            }
            else if(edgeInstances.ContainsKey(id))
            {
                return edgeInstances[id];
            }
            return null;
        }

        public bool AddVertex(IVertex node)
        {
            AddTypeIfNotExists(node);
            if (!verticesInstances.ContainsKey(node.ObjectId))
            {
                verticesInstances.Add(node.ObjectId, node);
                IndexVertex(node);

                foreach (IEdge edge in node.Links)
                {
                    edgeInstances.AddIfNotExists(edge.ObjectId, edge);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddVertexWithoutDirtyFlag(IVertex node)
        {
            return AddVertex(node);
        }

        private void AddTypeIfNotExists(IGraphObject graphObject)
        {
            if (!graphTypes.Contains(graphObject.TypeIdentity))
            {
                graphTypes.Add(graphObject.TypeIdentity);
            }
        }

        public void AddAnnotation(IGraphObject target, Annotation annotation)
        {
            AddAnnotations(target, new List<Annotation>() { annotation });
        }

        public void AddAnnotations(IGraphObject target, List<Annotation> annotations)
        {
            List<Annotation> existingAnnotationList;
            annotationsTable.TryGetValue(target, out existingAnnotationList);
            if (existingAnnotationList != null)
            {
                annotationsTable.Remove(target);
                annotations.AddRange(existingAnnotationList);
            }
            annotationsTable.Add(target, annotations);
        }

        public List<Annotation> GetAnnotations(IGraphObject @object)
        {
            List<Annotation> annotationList = null;
            if (annotationsTable.TryGetValue(@object, out annotationList))
            {
                return annotationList;
            }
            else
            {
                annotationList = new List<Annotation>();
                return annotationList;
            }
        }

        public List<Tuple<Guid, Annotation>> GetAllAnnotations()
        {
            List<Tuple<Guid, Annotation>> result = new List<Tuple<Guid, Annotation>>();

            foreach (KeyValuePair<Guid, IVertex> pair in verticesInstances)
            {
                foreach (Annotation a in GetAnnotations(pair.Value))
                {
                    result.Add(new Tuple<Guid, Annotation>(pair.Key, a));
                }
            }
            foreach (KeyValuePair<Guid, IEdge> pair in edgeInstances)
            {
                foreach (Annotation a in GetAnnotations(pair.Value))
                {
                    result.Add(new Tuple<Guid, Annotation>(pair.Key, a));
                }
            }

            return result;
        }

        public List<string> GetStringIndex(string indexName)
        {
            RebuildIndexes();
            if (globalIndexes.ContainsKey(indexName))
            {
                return new List<string>(globalIndexes[indexName].Keys);
            }
            else
            {
                return new List<string>();
            }
        }

        public Dictionary<string, List<IVertex>> GetFullIndex(string indexName)
        {
            RebuildIndexes();
            if (globalIndexes.ContainsKey(indexName))
            {
                return globalIndexes[indexName];
            }
            else
            {
                return new Dictionary<string, List<IVertex>>();
            }
        }

        public void RebuildIndexes()
        {
            RebuildIndexes(indexBuildingLogic);
        }

        public void RebuildIndexes(Func<IVertex, IEnumerable<IndexedString>> customLogic)
        {
            RebuildIndexes(customLogic, false);
        }

        public void RebuildIndexes(Func<IVertex, IEnumerable<IndexedString>> customLogic, bool overrideExistingLogic = false)
        {
            if (overrideExistingLogic)
            {
                indexBuildingLogic = customLogic;
            }
            if (indexDirty)
            {
                globalIndexes.Clear();
                foreach (IVertex node in verticesInstances.Values)
                {
                    IndexVertex(node);
                }
                indexDirty = false;
            }
        }

        private void IndexVertex(IVertex node)
        {
            IEnumerable<IndexedString> indexedStrings = indexBuildingLogic(node);

            foreach (IndexedString iString in indexedStrings)
            {
                // on crée l'index s'il n'existe pas
                if (!globalIndexes.ContainsKey(iString.IndexName))
                {
                    globalIndexes.Add(iString.IndexName, new Dictionary<string, List<IVertex>>());
                }
                // on récupère une référence à l'index à remplir
                Dictionary<string, List<IVertex>> index = globalIndexes[iString.IndexName];
                // on ajoute le noeud à la liste des noeuds indexé pour une chaîne donnée
                if (index.ContainsKey(iString.Value))
                {
                    index[iString.Value].Add(node);
                }
                else
                {
                    List<IVertex> nodeList = new List<IVertex>();
                    nodeList.Add(node);
                    index.Add(iString.Value, nodeList);
                }
            }
        }

        public List<IVertex> SearchNode(string indexName, string searchString)
        {
            if (indexDirty)
            {
                RebuildIndexes();
            }
            if (globalIndexes.ContainsKey(indexName))
            {
                Dictionary<string, List<IVertex>> index = globalIndexes[indexName];
                if (index.ContainsKey(searchString))
                {
                    return index[searchString];
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public List<IVertex> SearchNode(string searchString)
        {
            return SearchNode("", searchString);
        }

        public IEnumerator<IVertex> GetEnumerator()
        {
            return verticesInstances.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEdge CreateEdge(GraphObjectTypeInfo type, params IGraphObject[] targets)
        {
            return CreateEdge(type, Guid.NewGuid(), targets);
        }

        public IEdge CreateEdge(GraphObjectTypeInfo type, Guid publicId, params IGraphObject[] targets)
        {
            HyperEdge edge = new HyperEdge(publicId, type, targets);

            edgeInstances.AddIfNotExists(edge.ObjectId, edge);
            AddTypeIfNotExists(edge);

            return edge;
        }

        public bool RemoveVertex(IVertex node)
        {
            foreach (IEdge edge in node.Links)
            {
                edgeInstances.DeleteIfExists(edge.ObjectId);
                foreach (IGraphObject target in edge.GetLinkedObjects())
                {
                    //if (target != node)
                    //{
                        target.RemoveEdgeLink(edge);
                    //}
                }
            }
            verticesInstances.DeleteIfExists(node.ObjectId);

            return true; // TODO
        }

        public void RemoveAnnotation(IGraphObject target, Annotation annotation)
        {
            List<Annotation> annotationList;
            annotationsTable.TryGetValue(target, out annotationList);

            if (annotationList != null)
            {
                if (annotationList.Contains(annotation))
                {
                    annotationList.Remove(annotation);
                }
            }
        }

        public bool RemoveEdge(IEdge edge)
        {
            List<IGraphObject> linkedObjects = edge.GetLinkedObjects();

            if (edgeInstances.ContainsKey(edge.ObjectId))
            {
                edgeInstances.Remove(edge.ObjectId);
            }

            foreach (IGraphObject obj in linkedObjects)
            {
                obj.RemoveEdgeLink(edge);
            }

            return true;
        }
    }
}
