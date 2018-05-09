using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using Leger;

namespace HyperGraph.VsTests
{
    [TestClass]
    public class VertexExtensionsTests
    {
        GraphObjectTypeInfo vertexType = new GraphObjectTypeInfo("33ca7d63-78e0-44b2-90b2-279f20f48856", "Test Vertex Type", GraphObjectType.Vertex);
        OrientedBinaryEdgeTypeInfo edgeType = new OrientedBinaryEdgeTypeInfo("9eaa1218-14d9-46cd-9678-c2a29128982f", "Test Binary Oriented Edge Type");

        [TestMethod]
        public void VertexExtensionsOne()
        {
            IVertex origin = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex target = VertexExtensions.CreateTextVertex(vertexType, "v2");
            IEdge edge = new HyperEdge(edgeType, origin, target);

            var neighboors = origin.Successors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(1, neighboors.Count);
            Assert.AreSame(target, neighboors[0]);

            neighboors.Clear();
            neighboors = target.Successors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(0, neighboors.Count);
        }

        [TestMethod]
        public void GetSuccessorsTests_1()
        {
            // v1 -> v2 -> v3
            IVertex first = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex second = VertexExtensions.CreateTextVertex(vertexType, "v2");
            IVertex third = VertexExtensions.CreateTextVertex(vertexType, "v3");

            IEdge first_edge = new HyperEdge(edgeType, first, second);
            IEdge second_edge = new HyperEdge(edgeType, second, third);

            // à partir de v1 on doit obtenir seulement v2
            var neighboors = first.Successors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(1, neighboors.Count);
            Assert.AreSame(second, neighboors[0]);

            // à partir de v2 on doit obtenir seulement v3
            neighboors.Clear();
            neighboors = second.Successors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(1, neighboors.Count);
            Assert.AreSame(third, neighboors[0]);

            // à partir de v3 on doit ne doit rien obtenir
            neighboors.Clear();
            neighboors = third.Successors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(0, neighboors.Count);
        }

        [TestMethod]
        public void GetPredecessorsTests_1()
        {
            // v1 -> v2 -> v3
            IVertex first = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex second = VertexExtensions.CreateTextVertex(vertexType, "v2");
            IVertex third = VertexExtensions.CreateTextVertex(vertexType, "v3");

            IEdge first_edge = new HyperEdge(edgeType, first, second);
            IEdge second_edge = new HyperEdge(edgeType, second, third);

            var neighboors = first.Predecessors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(0, neighboors.Count);

            neighboors.Clear();
            neighboors = second.Predecessors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(1, neighboors.Count);
            Assert.AreSame(first, neighboors[0]);

            neighboors.Clear();
            neighboors = third.Predecessors(edgeType);
            Assert.IsNotNull(neighboors);
            Assert.AreEqual<int>(1, neighboors.Count);
            Assert.AreSame(second, neighboors[0]);
        }

        [TestMethod]
        public void RelationshipToSelf()
        {
            GraphObjectTypeInfo vertexType = new GraphObjectTypeInfo("33ca7d63-78e0-44b2-90b2-279f20f48856", "Test Vertex Type", GraphObjectType.Vertex);
            OrientedBinaryEdgeTypeInfo edgeType = new OrientedBinaryEdgeTypeInfo("9eaa1218-14d9-46cd-9678-c2a29128982f", "Test Binary Oriented Edge Type");

            Graph g = new Graph();
            IVertex first = VertexExtensions.CreateTextVertex(vertexType, "v1");
            g.AddVertex(first);

            g.CreateEdge(edgeType, first, first);

            IEnumerable<IVertex> list = first.Successors(edgeType);
            Assert.AreEqual(1, list.Count());
        }
    }
}