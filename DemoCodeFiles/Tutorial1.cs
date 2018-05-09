using System;
using Leger;
using Leger.IO;

namespace DemoCodeFiles
{
	class Tutorial1
	{
		// Use the debugguer to inspect object
		public static void Main(string[] args)
		{
			GraphObjectTypeInfo demoVertexType =
				new GraphObjectTypeInfo(
					id: "0ceb13cc-b23f-4918-9049-09a81807f0cd",
					name: "Vertex Example Type",
					type: GraphObjectType.Vertex,
					direct_content: true,
					oriented_edge: false);

			demoVertexType.Description = "This type is the first type of the tutorial.";

			IVertex demoVertex = new Vertex<SerializableString>(demoVertexType, new SerializableString("Node 1"));

			IGraph graph = new Graph();
			graph.AddVertex(demoVertex);
		}
	}
}