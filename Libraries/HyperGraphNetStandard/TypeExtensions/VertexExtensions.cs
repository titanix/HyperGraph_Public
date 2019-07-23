using System;
using System.Collections.Generic;

using Leger.IO;

namespace Leger
{
    public static class VertexExtensions
    {
        /// <summary>
        /// Return the successor vertices of a vertex following a given oriented binary relationship.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edgeType"></param>
        /// <returns></returns>
        public static List<IVertex> Successors(this IVertex vertex, OrientedBinaryEdgeTypeInfo edgeType)
        {
            List<IVertex> result = new List<IVertex>();

            foreach (IEdge edge in vertex.Links)
            {
                if (edge.TypeIdentity.Equals(edgeType))
                {
                    var linkedObjects = edge.GetLinkedObjects();
                    if (linkedObjects.Count != 2)
                    {
                        throw new ArgumentException("Edge relationship (parameter 2) is not binary.");
                    }
                    IVertex v1 = linkedObjects[0] as IVertex;
                    IVertex v2 = linkedObjects[1] as IVertex;
                    if (v1.Equals(vertex))
                    {
                        result.Add(v2);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Return the predecessor vertices of a vertex following a given oriented binary relationship.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edgeType"></param>
        /// <returns></returns>
        public static List<IVertex> Predecessors(this IVertex vertex, OrientedBinaryEdgeTypeInfo edgeType)
        {
            List<IVertex> result = new List<IVertex>();

            foreach (IEdge edge in vertex.Links)
            {
                if (edge.TypeIdentity.Equals(edgeType))
                {
                    var linkedObjects = edge.GetLinkedObjects();
                    if (linkedObjects.Count != 2)
                    {
                        throw new ArgumentException("Edge relationship (parameter 2) is not binary.");
                    }
                    IVertex v1 = linkedObjects[0] as IVertex;
                    IVertex v2 = linkedObjects[1] as IVertex;
                    if (v2.Equals(vertex))
                    {
                        result.Add(v1);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Return the vertices reached from a vertex by a given non oriented relationship type.
        /// The edge can be an hyper-edge (i.e. links more than two nodes together).
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edgeType"></param>
        /// <returns></returns>
        public static List<IVertex> NeighboorVertices(this IVertex vertex, NonOrientedEdgeTypeInfo edgeType)
        {
            List<IVertex> result = new List<IVertex>();

            foreach (IEdge edge in vertex.Links)
            {
                if (edge.TypeIdentity.Equals(edgeType))
                {
                    foreach (IGraphObject obj in edge.GetLinkedObjects())
                    {
                        if (obj.TypeIdentity.Type == GraphObjectType.Vertex && !obj.Equals(vertex))
                        {
                            result.Add(obj as IVertex);
                        }
                    }
                }
            }

            return result;
        }

        public static Vertex<SerializableString> CreateTextVertex(GraphObjectTypeInfo vertexType, string vertexContent)
        {
            return new Vertex<SerializableString>(vertexType, new SerializableString(vertexContent));
        }

        public static Vertex<SerializableString> CreateTextVertex(GraphObjectTypeInfo vertexType, string vertexContent, string language)
        {
            return new Vertex<SerializableString>(vertexType, new SerializableString(vertexContent), language);
        }

        public static Vertex<SerializableString> CreateTextVertex(GraphObjectTypeInfo vertexType, string vertexContent, string language, Guid id)
        {
            return new Vertex<SerializableString>(vertexType, new SerializableString(vertexContent), language, id);
        }

        public static bool IsOfType(this IVertex vertex, string guid)
        {
            return vertex.TypeIdentity.Id.Equals(Guid.Parse(guid));
        }

        public static bool IsOfType(this IVertex vertex, Guid guid)
        {
            return vertex.TypeIdentity.Id.Equals(guid);
        }
    }
}