using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.ObjectModel;

namespace Leger
{
    public abstract class DataStoreBackedGraph : IGraph
    {
        protected IGraphObjectRepository<IVertex> verticesInstances;

        protected IGraphObjectRepository<IEdge> edgeInstances;

        protected HashSet<GraphObjectTypeInfo> graphTypes = new HashSet<GraphObjectTypeInfo>();

        protected ConditionalWeakTable<IGraphObject, List<Annotation>> annotationsTable = new ConditionalWeakTable<IGraphObject, List<Annotation>>();

        protected IIndexManager indexManager;

        public DataStoreBackedGraph(
            IGraphObjectRepository<IVertex> vertexStore,
            IGraphObjectRepository<IEdge> edgeStore,
            Func<IVertex, IEnumerable<IndexedString>> indexConstructionLogic = null)
        {
            verticesInstances = vertexStore;
            edgeInstances = edgeStore;
            indexManager = new IndexManager(indexConstructionLogic);
        }

        public int VerticesNumber { get => verticesInstances.Count; }

        public int EdgesNumber { get => edgeInstances.Count; }

        public IReadOnlyList<GraphObjectTypeInfo> RegisteredTypes { get => new ReadOnlyCollection<GraphObjectTypeInfo>(graphTypes.ToList()); }

        public IGraphObject GetElementById(Guid id)
        {
            IVertex v = verticesInstances.GetElementById(id);

            if (v != null)
            {
                return v;
            }

            return edgeInstances.GetElementById(id);
        }

        public void RegisterType(GraphObjectTypeInfo type)
        {
            if (!graphTypes.Contains(type))
            {
                graphTypes.Add(type);
            }
        }

        public bool AddVertex(IVertex node)
        {
            AddTypeIfNotExists(node);

            verticesInstances.StoreIfNotExists(node);
            indexManager.IndexVertex(node);

            foreach (IEdge edge in node.Links)
            {
                edgeInstances.StoreIfNotExists(edge);
                AddTypeIfNotExists(edge);
            }

            return true;
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

            foreach (IVertex v in verticesInstances)
            {
                foreach (Annotation a in GetAnnotations(v))
                {
                    result.Add(new Tuple<Guid, Annotation>(v.ObjectId, a));
                }
            }
            foreach (IEdge e in edgeInstances)
            {
                foreach (Annotation a in GetAnnotations(e))
                {
                    result.Add(new Tuple<Guid, Annotation>(e.ObjectId, a));
                }
            }

            return result;
        }

        #region IIndexManager proxy
        public IVertexIndex GetIndex(string indexName)
        {
            return indexManager.GetIndex(indexName);
        }

        public List<string> GetIndexNames()
        {
            return indexManager.GetIndexNames();
        }

        public void RebuildIndexes()
        {
            indexManager.RebuildIndexes(verticesInstances.Values, null, false);
        }
        
        public void RebuildIndexes(Func<IVertex, IEnumerable<IndexedString>> customLogic, bool overrideExistingLogic)
        {
            indexManager.RebuildIndexes(verticesInstances.Values, customLogic, overrideExistingLogic);
        }

        public void IndexVertex(IVertex node)
        {
            indexManager.IndexVertex(node);
        }
        #endregion

        public virtual List<IVertex> SearchNode(string indexName, string searchString)
        {
            if (indexManager.GetIndexNames().Contains(indexName))
            {
                IVertexIndex index = indexManager.GetIndex(indexName);
                Dictionary<string, List<IVertex>> dict = index.ToDictionary();
                if (dict.ContainsKey(searchString))
                {
                    return dict[searchString];
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

            edgeInstances.StoreIfNotExists(edge);
            AddTypeIfNotExists(edge);

            return edge;
        }

        public bool RemoveVertex(IVertex node)
        {
            foreach (IEdge edge in node.Links)
            {
                edgeInstances.DeleteIfExists(edge);
                foreach (IGraphObject target in edge.GetLinkedObjects())
                {
                    target.RemoveEdgeLink(edge);
                }
            }
            verticesInstances.DeleteIfExists(node);

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

            edgeInstances.DeleteIfExists(edge);

            foreach (IGraphObject obj in linkedObjects)
            {
                obj.RemoveEdgeLink(edge);
            }

            return true;
        }
    }
}
