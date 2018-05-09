using System;
using System.Collections.Generic;

namespace Leger
{
    public static class IGraphObjectExtensions
    {
        public static List<IVertex> ToVertexList(this List<IGraphObject> list)
        {
            List<IVertex> result = new List<IVertex>();
            foreach (IGraphObject obj in list)
            {
                if (obj is IVertex)
                {
                    result.Add(obj as IVertex);
                }
            }
            return result;
        }

        public static List<IEdge> ToEdgeList(this List<IGraphObject> list)
        {
            List<IEdge> result = new List<IEdge>();
            foreach (IGraphObject obj in list)
            {
                if (obj is IEdge)
                {
                    result.Add(obj as IEdge);
                }
            }
            return result;
        }
    }
}
