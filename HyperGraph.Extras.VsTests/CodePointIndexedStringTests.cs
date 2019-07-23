namespace HyperGraph.VsTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml.Linq;
    using Leger.Extra.Trie;
    using Leger.IO;

    [TestClass]
    public class CodePointIndexedStringTests
    {
        [TestMethod]
        public void CPIInsideBMP()
        {
            CodePointIndexedString cpi = new CodePointIndexedString("abc");

            Assert.AreEqual(3, cpi.Length);
        }

        [TestMethod]
        public void CPIOutsideBMP()
        {
            CodePointIndexedString cpi = new CodePointIndexedString("𒆠𒆡");

            Assert.AreEqual(2, cpi.Length);
        }

        [TestMethod]
        public void CPIMixed()
        {
            CodePointIndexedString cpi = new CodePointIndexedString("𒆠ab𒆡c");

            Assert.AreEqual(5, cpi.Length);

            Assert.AreEqual("𒆠", cpi.AtIndex(0));
            Assert.AreEqual("a", cpi.AtIndex(1));
            Assert.AreEqual("b", cpi.AtIndex(2));
            Assert.AreEqual("𒆡", cpi.AtIndex(3));
            Assert.AreEqual("c", cpi.AtIndex(4));
        }

        [TestMethod]
        public void CPISubstringBMP()
        {
            CodePointIndexedString cpi = new CodePointIndexedString("abc");

            Assert.AreEqual("a", cpi.Substring(0, 1));
            Assert.AreEqual("ab", cpi.Substring(0, 2));
            Assert.AreEqual("abc", cpi.Substring(0, 3));

            Assert.AreEqual("bc", cpi.Substring(1, 2));
            
            Assert.AreEqual("c", cpi.Substring(2, 1));
        }

        [TestMethod]
        public void CPISubstringOutsideBMP()
        {
            CodePointIndexedString cpi = new CodePointIndexedString("𒆠ab");

            Assert.AreEqual("𒆠", cpi.Substring(0, 1));
            Assert.AreEqual("𒆠a", cpi.Substring(0, 2));
            Assert.AreEqual("𒆠ab", cpi.Substring(0, 3));

            Assert.AreEqual("ab", cpi.Substring(1, 2));

            Assert.AreEqual("b", cpi.Substring(2, 1));
        }
    }
}