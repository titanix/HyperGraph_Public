using System;
using System.Collections.Generic;
using Leger;
using Leger.IO;

namespace DemoCodeFiles
{
	class Tutorial3
	{
		// Use the debugguer to inspect object
		public static void Main_3(string[] args)
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

			// tutorial 3 start
			// Performing a Search
			Console.WriteLine("Default indexed string of Node 1:" + demoVertex);
			List<IVertex> results = graph.SearchNode("", "Node 1");

			Console.WriteLine("Number of results: " + results.Count);
			Console.WriteLine("Result node " + results[0]);

			// Traversing the Graph
			Console.WriteLine("Number of links from Node 1: " + demoVertex.Links.Count);

			List<IVertex> linkedNodes = new List<IVertex>();
			foreach (IEdge edge in demoVertex.Links)
			{
				foreach (IVertex v in edge.GetLinkedObjects(GraphObjectType.Vertex))
				{
					if (v.ObjectId != demoVertex.ObjectId)
					{
						linkedNodes.Add(v);
					}
				}
			}
			linkedNodes.ForEach(n => Console.WriteLine(n));

			// Get Successors (the Easy Way)
			OrientedBinaryEdgeTypeInfo relationship = new OrientedBinaryEdgeTypeInfo("45fede5f-a6bc-4560-a44d-906d5043abdf", "", "");
			linkedNodes = demoVertex.Successors(relationship);
			linkedNodes.ForEach(n => Console.WriteLine(n));
		}
	}
}