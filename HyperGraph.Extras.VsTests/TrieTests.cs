namespace HyperGraph.VsTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml.Linq;
    using Leger.Extra.Trie;
    using Leger.IO;

    [TestClass]
    public class TriTests
    {
        [TestMethod]
        public void BasicTrieConfiguration()
        {
            Trie trie = new Trie();
            trie.Insert("bar", "11");
            trie.Insert("bag", "42");
            trie.Insert("car", "56");
            trie.Insert("care", "23");
            trie.Insert("bar", "15");

            var results = trie.MatchPrefix("");
            Assert.AreEqual(5, results.Count);

            results = trie.MatchPrefix("b");
            Assert.AreEqual(3, results.Count);

            results = trie.MatchPrefix("c");
            Assert.AreEqual(2, results.Count);

            results = trie.MatchPrefix("d");
            Assert.AreEqual(0, results.Count);

            results = trie.MatchPrefix("ba");
            Assert.AreEqual(3, results.Count);

            results = trie.MatchPrefix("bar");
            Assert.AreEqual(2, results.Count);

            results = trie.MatchPrefix("bag");
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("bag", results[0].Match);
            Assert.AreEqual("42", results[0].Value);

            results = trie.MatchPrefix("ca");
            Assert.AreEqual(2, results.Count);

            results = trie.MatchPrefix("car");
            Assert.AreEqual(2, results.Count);

            results = trie.MatchPrefix("care");
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void TrieUtf16OutsideBMPOneInsertion()
        {
            Trie trie = new Trie();
            trie.Insert("𒆠", "ki");

            var result = trie.MatchPrefix("𒆠");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TrieUtf16OutsideBMPTwoInsertions()
        {
            Trie trie = new Trie();
            trie.Insert("𒆠", "ki");

            var result = trie.MatchPrefix("𒆠");
            Assert.AreEqual(1, result.Count);

            trie.Insert("𒆡", "_");

            result = trie.MatchPrefix("𒆠");
            Assert.AreEqual(1, result.Count);
            result = trie.MatchPrefix("𒆡");
            Assert.AreEqual(1, result.Count);
        }


        [TestMethod]
        public void TrieUtf16OutsideBMPSerialization()
        {
            Trie trie = new Trie();
            trie.Insert("𒆠", "ki");
            //trie.Insert("a", "ki");

            GraphXmlSerializer ser = new GraphXmlSerializer();
            ser.Serialize(trie).Save(@"C:\Users\Louis Lecailliez\Desktop\test_outside_bmp_trie3.xml");
        }
    }
}