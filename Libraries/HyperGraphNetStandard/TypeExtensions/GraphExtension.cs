using System;
using System.Collections.Generic;
using System.Linq;

using Leger.IO;

namespace Leger
{
    public static class GraphExtensions
    {
        public static List<IVertex> SearchNode(this IGraph graph, Guid type, string searchString)
        {
            return graph.SearchNode(searchString).Where(n => n.TypeIdentity.Id.ToString().Equals(type.ToString())).ToList();
        }

        public static List<IVertex> SearchNode(this IGraph graph, GraphObjectTypeInfo type, string searchString)
        {
            return graph.SearchNode(type.Id, searchString);
        }
    }
}