namespace HyperGraph.VsTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;

    using Leger;
    using Leger.IO;
    using Leger.Extra;

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
    }
}