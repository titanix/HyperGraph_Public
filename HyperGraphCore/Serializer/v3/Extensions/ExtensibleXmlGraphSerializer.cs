using System;
using System.Collections.Generic;
using System.Xml.Linq;

using System.Reflection;

namespace Leger.IO.Extensibility
{
    internal class ExtensionXmlNames
    {
        public string ExtensionListElement = "extensions";
        public string ExtensionElement = "extension";

        public string ExtensionPreferredDeserializerAttribute = "preffered_deserializer_type";
        public string ExtensionDezerializerAssemblyAttribute = "assembly_hint";
    }

    public class ExtensibleXmlGraphSerializer : IXmlGraphSerializer
    {
        List<IXmlExtension> extensions = new List<IXmlExtension>();
        GraphXmlSerializer serializer = new GraphXmlSerializer();
        ExtensionXmlNames extNames = new ExtensionXmlNames();

        public void RegisterExtension(IXmlExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                extensions.Add(extension);
            }
        }

        public XElement GenerateExtensionElement(IXmlExtension extension)
        {
            Type extType = extension.GetType();
            string type = extType.ToString();
            Assembly assembly = Assembly.GetAssembly(extType);

            XElement extNode = new XElement(extNames.ExtensionElement,
                new XAttribute(extNames.ExtensionPreferredDeserializerAttribute, type),
                new XAttribute(extNames.ExtensionDezerializerAssemblyAttribute, assembly.GetName().Name + ".dll"));
            extension.WriteObject(extNode);

            return extNode;
        }

        public XDocument Serialize(IGraphSerializable graph, bool useShortTags = false)
        {
            IXmlElementName nameProvider = new XmlStandardElementNames();
            if (useShortTags)
            {
                nameProvider = new XmlShortElementNames();
            }

            XDocument result = serializer.Serialize(graph, useShortTags);
            if (result != null)
            {
                XElement extensionNode = new XElement(extNames.ExtensionListElement);
                // pour chaque extensions on rempli le noeud extension
                foreach (IXmlExtension ext in extensions)
                {
                    extensionNode.Add(GenerateExtensionElement(ext));
                }

                result.Root.Element(nameProvider.TypeListElement).AddAfterSelf(extensionNode);
            }

            return result;
        }
    }
}