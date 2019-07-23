using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Leger.IO.Extensibility
{
    public class ExtensibleXmlGraphDeserializer
    {
        public ExtendedGraph Load(XDocument xdoc)
        {
            Graph graph = GraphXmlDeserializer.LoadFromXml(xdoc);
            ExtensionXmlNames extNames = new ExtensionXmlNames();
            List<IXmlExtension> extensions = new List<IXmlExtension>();

            if (xdoc != null)
            {
                XElement extensionList = xdoc.Root.Element(extNames.ExtensionListElement);
                foreach (XElement extensionNode in extensionList.Elements(extNames.ExtensionElement))
                {
                    string typeName = extensionNode.Attribute(extNames.ExtensionPreferredDeserializerAttribute)?.Value;
                    string assemblyName = extensionNode.Attribute(extNames.ExtensionDezerializerAssemblyAttribute)?.Value;
                    if (String.IsNullOrEmpty(typeName) || String.IsNullOrEmpty(assemblyName))
                    {
                        Debug.WriteLine($"Extension loading skipped. Parameters [{typeName}] and [{assemblyName}]");
                        continue;
                    }

                    Assembly assembly = Assembly.LoadFrom(assemblyName);
                    if (assembly == null)
                    {
                        Debug.WriteLine($"Assembly [{assemblyName}] could not be loaded.");
                        continue;
                    }
                    Type type = assembly.GetType(typeName);
                    if (type == null)
                    {
                        Debug.WriteLine($"Type [{typeName}] does not exist in assembly [{assemblyName}].");
                        continue;
                    }

                    IXmlExtension extension = Activator.CreateInstance(type) as IXmlExtension;
                    if (extension == null)
                    {
                        Debug.WriteLine($"Instance of type [{typeName}] could not be created as IXmlExtension.");
                        continue;
                    }

                    extension.ReadObject(extensionNode);
                    extensions.Add(extension);
                }
                return new ExtendedGraph(graph, extensions);
            }
            return null;
        }

        public ExtendedGraph Load(string path)
        {
            XDocument xdoc = XDocument.Load(new StreamReader(path));
            return Load(xdoc);
        }
    }
}
