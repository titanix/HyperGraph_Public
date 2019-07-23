using System.Text;

namespace Leger.Extra.IO
{
    public class DotFileSerializer
    {
        public DotFileSerializer() { }

        public StringBuilder SerializeToDotFile(IGraph graph, NodeDisplaySettings nodeSettings = null, EdgeDisplaySettings edgeSettings = null)
        {
            return graph.SerializeToDotFile();
        }
    }

    public static class DotFileSerializerExtensions
    {
        public static StringBuilder SerializeToDotFile(this IGraph graph, NodeDisplaySettings nodeSettings = null, EdgeDisplaySettings edgeSettings = null)
        {
            StringBuilder result = null;

            if (IsDirectedGraph(graph))
            {
                DirectedGraphDotFileSerializer sr = new DirectedGraphDotFileSerializer(graph);
                result = sr.Serialize();
                // TODO node & edge settings
            }
            else
            {
                UndirectedGraphDotFileSerializer sr = new UndirectedGraphDotFileSerializer();
                result = sr.Serialize(graph, nodeSettings, edgeSettings);
            }

            return result;
        }

        private static bool IsDirectedGraph(IGraph graph)
        {
            foreach (GraphObjectTypeInfo type in graph.RegisteredTypes)
            {
                if (type.Oriented)
                    return true;
            }
            return false;
        }
    }
}
