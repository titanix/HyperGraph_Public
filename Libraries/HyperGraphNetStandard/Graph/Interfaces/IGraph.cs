using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IGraph : IGraphSerializable
    {
        bool AddVertex(IVertex node);

        IEdge CreateEdge(GraphObjectTypeInfo type, params IGraphObject[] targets);
        IEdge CreateEdge(GraphObjectTypeInfo type, Guid publicId, params IGraphObject[] targets);

        bool RemoveVertex(IVertex node);
        void RemoveAnnotation(IGraphObject target, Annotation annotation);
        bool RemoveEdge(IEdge edge);

        void AddAnnotation(IGraphObject target, Annotation annotation);
        void AddAnnotations(IGraphObject target, List<Annotation> annotations);

        List<IVertex> SearchNode(string searchString);
        List<IVertex> SearchNode(string indexName, string searchString);

        IReadOnlyList<GraphObjectTypeInfo> RegisteredTypes { get; }
        int VerticesNumber { get; }
        int EdgesNumber { get; }

        IVertexIndex GetIndex(string indexName);
        void RebuildIndexes();
        void RebuildIndexes(Func<IVertex, IEnumerable<IndexedString>> customLogic, bool overrideExistingLogic);
    }
}
