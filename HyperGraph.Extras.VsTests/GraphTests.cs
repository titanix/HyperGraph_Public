using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Leger;
using Leger.IO;
using Leger.Extra;

namespace HyperGraph.VsTests
{
    [TestClass]
    public class XmlFormatV3DeserializerTests
    {
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

        public class DotNetFileProvider : IFileProvider
        {
            public StreamReader GetFileReader(string path)
            {
                return new StreamReader(path);
            }
        }
    }
}