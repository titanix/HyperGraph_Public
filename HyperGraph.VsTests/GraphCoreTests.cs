using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Leger;

namespace HyperGraph.VsTests
{
    [TestClass]
    public class GraphCoreTests
    {
        [TestMethod]
        public void RemoveVertexSimpleGraph()
        {
            GraphObjectTypeInfo vertexType = new GraphObjectTypeInfo("33ca7d63-78e0-44b2-90b2-279f20f48856", "Test Vertex Type", GraphObjectType.Vertex);
            GraphObjectTypeInfo edgeType = new GraphObjectTypeInfo("9eaa1218-14d9-46cd-9678-c2a29128982f", "Test Edge Type", GraphObjectType.Edge);

            Graph g = new Graph();
            IVertex origin = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex target = VertexExtensions.CreateTextVertex(vertexType, "v2");

            g.AddVertex(origin);
            g.AddVertex(target);
            g.CreateEdge(edgeType, origin, target);

            Assert.AreEqual<int>(1, origin.Links.Count);
            Assert.AreEqual<int>(1, target.Links.Count);

            g.RemoveVertex(target);

            List<IVertex> results = g.SearchNode("", "v2");
            Assert.IsNull(results);
            Assert.AreEqual<int>(0, origin.Links.Count);
            // not in the graph anymore but should have the reference to the edge deleted
            Assert.AreEqual<int>(0, target.Links.Count);
        }

        [TestMethod]
        public void RemoveEdgeSimpleGraph()
        {
            GraphObjectTypeInfo vertexType = new GraphObjectTypeInfo("33ca7d63-78e0-44b2-90b2-279f20f48856", "Test Vertex Type", GraphObjectType.Vertex);
            GraphObjectTypeInfo edgeType = new GraphObjectTypeInfo("9eaa1218-14d9-46cd-9678-c2a29128982f", "Test Edge Type", GraphObjectType.Edge);

            Graph g = new Graph();
            IVertex origin = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex target = VertexExtensions.CreateTextVertex(vertexType, "v2");

            g.AddVertex(origin);
            g.AddVertex(target);
            IEdge edge = g.CreateEdge(edgeType, origin, target);

            Assert.AreEqual<int>(1, origin.Links.Count);
            Assert.AreEqual<int>(1, target.Links.Count);
            Assert.AreEqual<int>(1, g.EdgesNumber);

            g.RemoveEdge(edge);

            Assert.AreEqual<int>(0, origin.Links.Count);
            Assert.AreEqual<int>(0, target.Links.Count);
            Assert.AreEqual<int>(0, g.EdgesNumber);
        }
    }
}