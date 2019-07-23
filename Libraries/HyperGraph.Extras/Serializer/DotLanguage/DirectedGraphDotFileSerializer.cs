using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Leger.Extra.IO
{
	[Quality(Level = QualityLevel.Untested)]
	internal class DirectedGraphDotFileSerializer
    {
		Dictionary<IVertex, long> nodeIdTable = new Dictionary<IVertex, long>();
		StringBuilder dotFile = new StringBuilder();
		IGraph graph;
		bool mergeTransitions;

		public bool MergeTransitions
		{
			get { return mergeTransitions; }
			set { mergeTransitions = value; }
		}

		public DirectedGraphDotFileSerializer(IGraph g)
		{
			graph = g;
		}

        public void WriteFile(string path)
        {
            GenerateFile();
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(dotFile.ToString());
            }
        }

        public StringBuilder Serialize()
        {
            GenerateFile();
            return dotFile;
        }

        private void GenerateFile()
		{
			BuildTables(graph);

			AppendProlog();
			AppendNodes();
			AppendEdges();
			AppendEnding();
		}

		private void AppendProlog()
		{
			dotFile.AppendLine("digraph {");
		}

		private void AppendEnding()
		{
			dotFile.AppendLine("}");
		}


		private void AppendNodes()
		{
			StringBuilder states = new StringBuilder();

			foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
			{
				IVertex v = pair.Key;
				dotFile.AppendLine(String.Format("{0} [label=\"{1}\"];", GenerateNodeName(v), v.ToString()));
			}
		}

		private string GenerateNodeName(IVertex v)
		{
			return "_" + v.ObjectId.ToString().Replace('-', '_');
		}

		private void AppendEdges()
		{
			if (MergeTransitions)
			{
				AppendEdgesMerged();
			}
			else
			{
				AppendEdgesNonMerged();
			}
		}

		#region Private Classes
		private class OriginTarget
		{
			public IVertex Origin { get; set; }
			public IVertex Target { get; set; }

			public override bool Equals(object obj)
			{
				OriginTarget ot = obj as OriginTarget;
				return this.GetHashCode() == ot.GetHashCode();
			}

			// required so that two different instances are recognized as the same dictionary key
			// https://github.com/dotnet/corefx/blob
			public override int GetHashCode()
			{
				return Origin.GetHashCode() ^ Target.GetHashCode();
			}
		}

		private class OriginTargetAnnotation
		{
			public IVertex Origin { get; set; }
			public IVertex Target { get; set; }
			public ComparableAnnotation Label { get; set; }

			public override bool Equals(object obj)
			{
				OriginTargetAnnotation ota = obj as OriginTargetAnnotation;
				return this.GetHashCode() == ota.GetHashCode();
			}

			public override int GetHashCode()
			{
				return Origin.GetHashCode() ^ Target.GetHashCode() ^ Label.GetHashCode();
			}
		}

		internal class ComparableAnnotation : Annotation
		{
			public override int GetHashCode()
			{
				return (base.Namespace + base.Key + base.Value).GetHashCode();
			}
		}
		#endregion

		private void AppendEdgesMerged()
		{
			// keep track of the transitions that are already processed
			// this is needed because duplicate are created when looking for a transition from both its origin and target node
			//HashSet<OriginTargetAnnotation> processedTransitions = new HashSet<OriginTargetAnnotation>();
			Dictionary<OriginTarget, StringBuilder> transitionLabels = new Dictionary<OriginTarget, StringBuilder>();

			foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
			{
				foreach (IEdge edge in pair.Key.Links.Where(l => l.GetLinkedObjects()[0].Equals(pair.Key)))
				{
					Annotation annotation = graph.GetAnnotations(edge).First();

					OriginTarget ot = new OriginTarget()
					{
						Origin = edge.GetLinkedObjects()[0] as IVertex,
						Target = edge.GetLinkedObjects()[1] as IVertex,
					};
					//OriginTargetAnnotation ota = new OriginTargetAnnotation()
					//{
					//    Origin = edge.GetLinkedObjects()[0] as IVertex,
					//    Target = edge.GetLinkedObjects()[1] as IVertex,
					//    Label = new ComparableAnnotation() { Namespace = annotation.Namespace, Key = annotation.Key, Value = annotation.Value }
					//};

					//if (processedTransitions.Contains(ota))
					//    continue;

					string label = $"{annotation.Key}:{annotation.Value}";
					if (transitionLabels.ContainsKey(ot))
					{
						transitionLabels[ot].Append($",{label}");
					}
					else
					{
						transitionLabels.Add(ot, new StringBuilder(label));
					}

					//processedTransitions.Add(ota);
				}
			}
			foreach (KeyValuePair<OriginTarget, StringBuilder> entry in transitionLabels)
			{
				var ot = entry.Key;
				dotFile.AppendLine($"{GenerateNodeName(ot.Origin)} ->  {GenerateNodeName(ot.Target)}[label=\"{entry.Value.ToString()}\"];");
			}
		}

		private void AppendEdgesNonMerged()
		{
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
				// ATTENTION ce n'est adapté qu'aux digraph stricts
				IVertex source = edge.GetLinkedObjects()[0] as IVertex;
				IVertex target = edge.GetLinkedObjects()[1] as IVertex;
				Annotation label = graph.GetAnnotations(edge).First();

				dotFile.AppendLine(String.Format("{0} -> {1}[label=\"{2}:{3}\"];", GenerateNodeName(source), GenerateNodeName(target), label.Key, label.Value));
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
	}

	public static class SinoLexVertexExtensions
	{
		public static List<IVertex> Successors(this IVertex vertex)
		{
			List<IVertex> result = new List<IVertex>();

			foreach (IEdge edge in vertex.Links)
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

			return result;
		}
	}
}
