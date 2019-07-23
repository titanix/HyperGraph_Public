using System;
using System.Collections.Generic;

namespace Leger.Extra.Trie
{
    interface ITrie : IGraphSerializable
    {
        void Insert(string str, string value);
        List<TrieMatchResult> MatchPrefix(string search);
    }

    public class TrieMatchResult : Tuple<string, string>
    {
        public TrieMatchResult(string match, string value) : base(match, value) { }
        public string Match { get => Item1; }
        public string Value { get => Item2; }
    }
}
