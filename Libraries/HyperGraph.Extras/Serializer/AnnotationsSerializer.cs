using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;

using Leger;
using Leger.IO;

namespace Leger.Extra.IO
{
    public class AnnotationsSerializer
    {
        IXmlElementName nameProvider = new XmlStandardElementNames();

        public XDocument Serialize(List<Tuple<Guid, Annotation>> annotations)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement(XmlHeaderNames.RootElement,
                new XElement(XmlHeaderNames.HeaderElement,
                    new XAttribute(XmlHeaderNames.VersionAttribute, "1.0")));
            doc.Add(root);

            root.Add(new XElement(nameProvider.TypeListElement));
            root.Add(new XElement(nameProvider.NodeListElement));
            root.Add(new XElement(nameProvider.EdgeListElement));
            root.Add(GenerateAnnotationsElement(annotations));

            return doc;
        }

        private XElement GenerateAnnotationsElement(List<Tuple<Guid, Annotation>> annotations)
        {
            XElement result = new XElement(nameProvider.AnnotationListElement);

            foreach (Tuple<Guid, Annotation> t in annotations)
            {
                result.Add(new XElement(nameProvider.AnnotationElement,
                    new XAttribute(nameProvider.AnnotationNamespaceAttribute, t.Item2.Namespace ?? ""),
                    new XAttribute(nameProvider.AnnotationKeyAttribute, t.Item2.Key),
                    new XAttribute(nameProvider.AnnotationValueAttribute, t.Item2.Value),
                    new XElement(nameProvider.AnnotationTargetElement,
                        new XAttribute(nameProvider.AnnotationTargetTypeAttribute, nameProvider.AnnotationTargetAttributeValueExternal),
                        new XAttribute(nameProvider.AnnotationTargetIdAttribute, t.Item1))));
            }

            return result;
        }
    }
}
