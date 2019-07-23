using System;
using System.Collections.Generic;
using System.Text;

namespace Leger.Extra.Trie
{
    internal class CodePointIndexedString
    {
        List<string> codePoints = new List<string>();

        internal CodePointIndexedString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] > 0xD800)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(str[i]);
                    sb.Append(str[i + 1]);
                    string newString = sb.ToString();
                    codePoints.Add(newString);
                    i++;
                }
                else
                {
                    codePoints.Add(str[i].ToString());
                }
            }
        }

        internal int Length
        {
            get
            {
                return codePoints.Count;
            }
        }

        internal string AtIndex(int i)
        {
            if (i < 0 || i > codePoints.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return codePoints[i];
        }

        internal string Substring(int start, int length)
        {
            if(start < 0 || start + length > codePoints.Count)
            {
                throw new ArgumentException();
            }

            StringBuilder result = new StringBuilder();
            for (int i = start; i < start + length; i++)
            {
                result.Append(codePoints[i]);
            }
            return result.ToString();
        }
    }
}
