using System;
using System.Xml.Linq;

namespace Leger.IO.Extensibility
{
    public interface IXmlExtension
    {
        string ExtensionName { get; }
        void WriteObject(XElement extensionNode);
        void ReadObject(XElement extensionNode);
        object Content { get; }
    }
}
