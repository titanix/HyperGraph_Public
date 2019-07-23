using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Leger.Extra.IO
{
	[Quality(Level = QualityLevel.Untested)]
	internal class UndirectedGraphDotFileSerializer
    {
		Dictionary<IVertex, long> nodeIdTable = new Dictionary<IVertex, long>();
		Dictionary<string, string> shapes = new Dictionary<string, string>();

		// TODO: name parameter
		const string FilePrologue = "graph dumped_knowledge_map {";
		const string FileEpilogue = "}";

		IGraph graph;
		NodeDisplaySettings nodeDisplaySettings;
		EdgeDisplaySettings edgeDisplaySettings;

		public StringBuilder Serialize(IGraph g, NodeDisplaySettings nodeSettings = null, EdgeDisplaySettings edgeSettings = null)
		{
            StringBuilder result = new StringBuilder();

            nodeDisplaySettings = nodeSettings;
			edgeDisplaySettings = edgeSettings;

			BuildTables(g);
			graph = g;

			result.Append(FilePrologue);
            result.Append(GenerateNodesElement());
            result.Append(GenerateEdgesElement());
            //stream.Write(GenerateAnnotationsElement(g));
            result.Append(FileEpilogue);

            return result;
		}

		public void AssociateShape(string id, string shape)
		{
			if (!shapes.ContainsKey(id))
			{
				shapes.Add(id, shape);
			}
		}

		private void BuildTables(IGraphSerializable g)
		{
			int nodeId = 0;

			foreach (IVertex v in g)
			{
				if (!nodeIdTable.ContainsKey(v))
				{
					nodeIdTable.Add(v, nodeId++);
				}
			}
		}

		private string GenerateNodesElement()
		{
			StringBuilder sb = new StringBuilder();

			foreach (IVertex v in graph)
			{
				string shape = "";
				string typeId = v.TypeIdentity.Id.ToString();
				if (shapes.ContainsKey(typeId))
				{
					shape = $"shape={shapes[typeId]},";
				}

				sb.Append(v.ObjectId.ToNodeName());
				if (nodeDisplaySettings == null || !nodeDisplaySettings.DisplayIds)
				{
					string value = v.ToString().Replace("\"", "\\\"");
					sb.Append($"[{shape}label=\"{value}\"]");
				}
				else
				{
					string value = v.ToString();
					string id = v.ObjectId.ToString();
					sb.Append($"[{shape}label=<{value}<br/><font color=\"{nodeDisplaySettings.Color}\" point-size=\"{nodeDisplaySettings.FontSize}\">{id}</font>>]");
				}
				sb.Append(";");
				sb.AppendLine();
			}

			return sb.ToString();
		}

		private string GenerateEdgesElement()
		{
			StringBuilder sb = new StringBuilder();

			HashSet<IEdge> edgeToPersiste = new HashSet<IEdge>();
			foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
			{
				foreach (IEdge edge in pair.Key.Links)
				{
					if (!edgeToPersiste.Contains(edge))
					{
						edgeToPersiste.Add(edge);
					}
				}
			}

			foreach (IEdge edge in edgeToPersiste)
			{
				/*
				XElement edgeElement = new XElement(nameProvider.EdgeElement,
					new XAttribute(nameProvider.EdgeTypeAttribute, typeAliasTable[edge.TypeIdentity]),
					new XAttribute(nameProvider.EdgeExternalIdAttribute, edge.ObjectId));
				*/
				// TODO edge label
				IGraphObject[] nodes = edge.GetLinkedObjects().ToArray();
				sb.Append(nodes[0].ObjectId.ToNodeName());
				sb.Append(" -- ");
				sb.Append(nodes[1].ObjectId.ToNodeName());
				sb.Append(";\n");
			}

			return sb.ToString();
		}
	}
}