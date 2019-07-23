using System;
using System.Collections.Generic;
using System.Linq;

namespace Leger.IO.Extensibility
{
    public class ExtendedGraph
    {
        Graph graph;
        List<IXmlExtension> extensions;

        public Graph Graph => graph;
        public List<IXmlExtension> Extensions => extensions;

        public ExtendedGraph(Graph graph, IEnumerable<IXmlExtension> extensions)
        {
            this.graph = graph;
            this.extensions = extensions.ToList();
        }
    }
}
