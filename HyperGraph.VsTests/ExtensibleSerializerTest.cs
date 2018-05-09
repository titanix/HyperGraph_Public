namespace HyperGraph.VsTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Leger;
    using Leger.IO;
    using Leger.IO.Extensibility;
    using System.Xml.Linq;

    public class TestExtension : IXmlExtension
    {
        private object content;

        public string ExtensionName => "Test Extension";

        public void WriteObject(XElement extensionNode)
        {
            extensionNode.Add(new XElement("test-element", "[text]"));
        }

        public void ReadObject(XElement extension)
        {
            content = extension.Element("test-element").Value;
        }

        public object Content { get { return content; } }
    }

    [TestClass]
    public class ExtensibleSerializationTests
    {
        private Graph GetTestGraph()
        {
            GraphObjectTypeInfo vertexType = new GraphObjectTypeInfo("33ca7d63-78e0-44b2-90b2-279f20f48856", "Test Vertex Type", GraphObjectType.Vertex);
            GraphObjectTypeInfo edgeType = new GraphObjectTypeInfo("9eaa1218-14d9-46cd-9678-c2a29128982f", "Test Edge Type", GraphObjectType.Edge);

            Graph g = new Graph();
            IVertex origin = VertexExtensions.CreateTextVertex(vertexType, "v1");
            IVertex target = VertexExtensions.CreateTextVertex(vertexType, "v2");

            g.AddVertex(origin);
            g.AddVertex(target);
            g.CreateEdge(edgeType, origin, target);

            return g;
        }

        [TestMethod]
        public void ExtensibleSerializationWithoutExtension()
        {
            ExtensibleXmlGraphSerializer ser = new ExtensibleXmlGraphSerializer();
            Graph g = GetTestGraph();

            XDocument doc = ser.Serialize(g);
            Assert.IsNotNull(doc);

            ExtensionXmlNames extNames = new ExtensionXmlNames();
            XElement extensions = doc.Root.Element(extNames.ExtensionListElement);
            Assert.IsNotNull(extensions);
        }

        [TestMethod]
        public void ExtensibleSerializationDeserializationAsGraph()
        {
            ExtensibleXmlGraphSerializer ser = new ExtensibleXmlGraphSerializer();
            Graph g = GetTestGraph();

            XDocument doc = ser.Serialize(g);
            string path = Path.GetTempFileName();
            doc.Save(path);

            Assert.IsTrue(File.Exists(path));

            g = GraphXmlDeserializer.GetGraphInstance(path, new FileLoader());
            Assert.IsNotNull(g);
        }

        [TestMethod]
        public void ExtensibleSerializationWithOneExtension()
        {
            ExtensibleXmlGraphSerializer ser = new ExtensibleXmlGraphSerializer();
            Graph g = GetTestGraph();

            IXmlExtension extension = new TestExtension();
            ser.RegisterExtension(extension);

            XDocument doc = ser.Serialize(g);
            Assert.IsNotNull(doc);

            ExtensionXmlNames extNames = new ExtensionXmlNames();
            XElement extensions = doc.Root.Element(extNames.ExtensionListElement);
            Assert.IsNotNull(extensions);
        }

        [TestMethod]
        public void ExtensibleDeserialization()
        {
            ExtensibleXmlGraphSerializer ser = new ExtensibleXmlGraphSerializer();
            Graph g = GetTestGraph();

            IXmlExtension extension = new TestExtension();
            ser.RegisterExtension(extension);

            XDocument doc = ser.Serialize(g);
            string path = Path.GetTempFileName();
            doc.Save(path);

            Assert.IsTrue(File.Exists(path));

            ExtensibleXmlGraphDeserializer extDeser = new ExtensibleXmlGraphDeserializer();
            var result = extDeser.Load(path);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Graph);
            Assert.AreEqual(1, result.Extensions.Count);
        }
    }
}