namespace HyperGraph.VsTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Leger;
    using Leger.IO;

	/*
	 * Notes: 
	 * 1. This test suit requires VisualStudio to be run.
	 * 2. Files 'ids.xml', 'ids_light_5000.xml', and 'readings_light_5000.xml' are not distributed with this project.
	 * 3. DeploymentItem paths need to be change manually before the file can work again.
	 */
    [TestClass]
    public class XmlFormatV3DeserializerTests
    {
        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\ids.xml")]
        [TestMethod]
        public void IdsFileLoading()
        {
            DotNetFileProvider prov = new DotNetFileProvider();
            Graph g = GraphXmlDeserializer.GetGraphInstance("ids.xml", prov, ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(g);
        }

        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\ids_light_5000.xml")]
        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\readings_light_5000.xml")]
        [TestMethod]
        public void MultipleFilesLoading()
        {
            DotNetFileProvider prov = new DotNetFileProvider();
            Graph g = GraphXmlDeserializer.LoadMultipleFiles(new string[] { "ids_light_5000.xml", "readings_light_5000.xml" }, prov, ExternalRessourcesLoadingPolicy.None);
            
            Assert.IsNotNull(g);
            var search = g.SearchNode("", "侖");
            Assert.IsNotNull(search);
            Assert.AreEqual<int>(1, search.Count);

            IVertex v = search[0];
        }

        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\annotations_test.xml")]
        [TestMethod]
        public void NodeInternalAnnotationTest()
        {
            Graph g = GraphXmlDeserializer.GetGraphInstance("annotations_test.xml", new DotNetFileProvider(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(g);

            var search = g.SearchNode("", "avion");
            Assert.IsNotNull(search);
            IVertex v = search[0];

            var annotations = g.GetAnnotations(v);
            Assert.AreEqual<int>(1, annotations.Count);
            Assert.AreEqual<string>("", annotations[0].Namespace);
            Assert.AreEqual<string>("pos", annotations[0].Key);
            Assert.AreEqual<string>("nom", annotations[0].Value);
        }

        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\annotations_test.xml")]
        [TestMethod]
        public void EditableVertexLoadingTest()
        {
            Graph g = GraphXmlDeserializer.GetGraphInstance("annotations_test.xml", new DotNetFileProvider(), typeof(EditableVertex));
            Assert.IsNotNull(g);

            var search = g.SearchNode("", "avion");
            Assert.IsNotNull(search);
            IVertex v = search[0];
        }

        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\annotations_test.xml")]
        [TestMethod]
        public void NodeExternalAnnotationTest()
        {
            Graph g = GraphXmlDeserializer.GetGraphInstance("annotations_test.xml", new DotNetFileProvider(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(g);

            var search = g.SearchNode("", "porter");
            Assert.IsNotNull(search);
            IVertex v = search[0];

            var annotations = g.GetAnnotations(v);
            Assert.AreEqual<int>(1, annotations.Count);
            Assert.AreEqual<string>("", annotations[0].Namespace);
            Assert.AreEqual<string>("pos", annotations[0].Key);
            Assert.AreEqual<string>("verbe", annotations[0].Value);
        }

        [DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\abc.xml")]
        [TestMethod]
        public void EdgeAnnotationTest()
        {
            Graph g = GraphXmlDeserializer.LoadMultipleFiles(new string[] { "abc.xml" }, new DotNetFileProvider(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(g);

            IVertex a = g.SearchNode("", "A").FirstOrDefault();
            
        }

        public class DotNetFileProvider : IFileProvider
        {
            public StreamReader GetFileReader(string path)
            {
                return new StreamReader(path);
            }
        }
    }
}