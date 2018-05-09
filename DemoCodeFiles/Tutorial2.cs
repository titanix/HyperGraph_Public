using System;
using Leger;
using Leger.IO;

namespace DemoCodeFiles
{
	class Tutorial2
	{
		// Use the debugguer to inspect object
		public static void Main_2(string[] args)
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

			// tutorial 2 start
			IVertex demoVertex2 = new Vertex<SerializableString>(demoVertexType, new SerializableString("Node 2"));
			IVertex demoVertex3 = new Vertex<SerializableString>(demoVertexType, new SerializableString("Node 3"));
			graph.AddVertex(demoVertex2);
			graph.AddVertex(demoVertex3);

			GraphObjectTypeInfo demoEdgeType =
				new GraphObjectTypeInfo("45fede5f-a6bc-4560-a44d-906d5043abdf", "Demo Edge Type", GraphObjectType.Edge);

			graph.CreateEdge(demoEdgeType, demoVertex, demoVertex2);
			graph.CreateEdge(demoEdgeType, demoVertex, demoVertex3);
		}
	}
}