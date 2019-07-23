using System;
using System.Collections.Generic;
using System.Text;

namespace Leger.IO
{
    public class XmlFileParsingException : Exception
    {
        private bool ignorable;

        public bool Ignorable
        {
            get { return ignorable; }
            set { ignorable = value; }
        }

        public XmlFileParsingException() { }

        public XmlFileParsingException(string message)
            : base(message) { }

        public XmlFileParsingException(string message, Exception inner)
            : base(message, inner) { }
    }
}
