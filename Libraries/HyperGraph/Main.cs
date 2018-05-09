using System;
using Leger;
using Leger.IO;

namespace HyperGraph
{
    public class Shell
    {
        Graph repositoryGraph = new Graph();

        public void Loop()
        {
            Console.WriteLine("Enter command:");
            string line = Console.ReadLine();
            // parse command

            repositoryGraph = LoadDataGraph();
            Command cmd = new ListObjectsCommand() { Graph = repositoryGraph };
            cmd.Execute();
        }

        #region TEST_DATA
        public Graph LoadDataGraph()
        {
            Graph DataGraph = new Graph();

            GraphObjectIdentity Fr = new GraphObjectIdentity(Guid.Parse("5688661d-ed5d-4909-b209-a43f9807943f"), "Fr", GraphObjectType.Vertex);
            GraphObjectIdentity JpK = new GraphObjectIdentity(Guid.Parse("3be991a3-f006-4c6f-af62-838c35e5276b"), "JpK", GraphObjectType.Vertex);
            GraphObjectIdentity JpR = new GraphObjectIdentity(Guid.Parse("d2ac1c0c-896d-48fd-a1e9-f61808ffcfc6"), "JpR", GraphObjectType.Vertex);
            GraphObjectIdentity Kanji = new GraphObjectIdentity(Guid.Parse("cde322c6-4f14-4735-bff0-ae59d75448e1"), "Kanji", GraphObjectType.Vertex);
            GraphObjectIdentity Example = new GraphObjectIdentity(Guid.Parse("36524d6e-a0c0-4f39-8b94-9f0e6658100c"), "Example", GraphObjectType.Vertex);
            GraphObjectIdentity Rel_JpK_Kanji = new GraphObjectIdentity(Guid.Parse("003c88a5-2c85-493b-80b1-d0869090b9b6"), "Kanjis contenus", GraphObjectType.Edge);
            GraphObjectIdentity Rel_Def3 = new GraphObjectIdentity(Guid.Parse("0eac1c2b-0b04-4f58-aec4-33118b530b74"), "Traductions", GraphObjectType.Edge);

            var ermite = new Vertex<SerializableString>(new SerializableString("ermite"), Fr);
            var sage = new Vertex<SerializableString>(new SerializableString("sage"), Fr);
            var sennin_k = new Vertex<SerializableString>(new SerializableString("仙人"), JpK);
            var sennin_r = new Vertex<SerializableString>(new SerializableString("せんにん"), JpR);
            var k1 = new Vertex<SerializableString>(new SerializableString("仙"), Kanji);
            var k2 = new Vertex<SerializableString>(new SerializableString("人"), Kanji);

            var def_a = new UnorderedEdge(sennin_k, sennin_r, ermite);
            var def_b = new UnorderedEdge(sennin_k, sennin_r, sage);
            var rel_c = new UnorderedEdge(sennin_k, k1);
            var rel_d = new UnorderedEdge(sennin_k, k2);
            def_a.SetType(Rel_Def3);
            def_b.SetType(Rel_Def3);
            rel_c.SetType(Rel_JpK_Kanji);
            rel_d.SetType(Rel_JpK_Kanji);

            DataGraph.AddVertex(ermite);
            DataGraph.AddVertex(sage);
            DataGraph.AddVertex(sennin_k);
            DataGraph.AddVertex(sennin_r);
            DataGraph.AddVertex(k1);
            DataGraph.AddVertex(k2);

            return DataGraph;
        }
        #endregion

        public static void Main()
        {
            Shell shell = new Shell();
            shell.Loop();
        }
    }

    public abstract class Command
    {
        public Graph Graph { get; set; }
        public abstract void Execute();
    }

    public class ListObjectsCommand : Command
    {
        public override void Execute()
        {
            /*
            foreach (IVertex item in Graph)
            {
                Console.WriteLine(item.ToString());
            }
            */
            GraphTextSerializer sr = new GraphTextSerializer();
            var result = sr.Serialize(Graph);
            Console.Write(result);
        }
    }
}
