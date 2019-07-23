using System;
using System.Collections.Generic;
using System.Text;

namespace Leger.IO
{
    public class TextFileParsingException : Exception
    {
        private bool ignorable;

        public bool Ignorable
        {
            get { return ignorable; }
            set { ignorable = value; }
        }
        
        public TextFileParsingException() { }

        public TextFileParsingException(string message)
            : base(message) { }

        public TextFileParsingException(string message, Exception inner)
            : base(message, inner) { }
    }
}
