using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Leger;

namespace Leger.IO
{
    interface IXmlGraphSerializer
    {
        XDocument Serialize(IGraphSerializable graph, bool useShortTags = false);
    }

    public class GraphXmlSerializer : IXmlGraphSerializer
    {
        Dictionary<GraphObjectTypeInfo, int> typeAliasTable = new Dictionary<GraphObjectTypeInfo, int>();
        Dictionary<IVertex, long> nodeIdTable = new Dictionary<IVertex, long>();
        IXmlElementName nameProvider = new XmlStandardElementNames();

        public XDocument Serialize(IGraphSerializable g, bool useShortTags = false)
        {
            BuildTables(g);
            XDocument doc = new XDocument();
            XElement root = new XElement(XmlHeaderNames.RootElement,
                new XElement(XmlHeaderNames.HeaderElement,
                    new XAttribute(XmlHeaderNames.VersionAttribute, "1.0"),
                    new XAttribute(XmlHeaderNames.UseShortTagsAttribute,
                        useShortTags ? XmlHeaderNames.StringBooleanValueTrue : XmlHeaderNames.StringBooleanValueFalse)));
            doc.Add(root);
            if (useShortTags)
            {
                nameProvider = new XmlShortElementNames();
            }

            root.Add(GenerateTypesElement());
            root.Add(GenerateNodesElement());
            root.Add(GenerateEdgesElement());
            root.Add(GenerateAnnotationsElement(g));

            return doc;
        }

        private XElement GenerateAnnotationsElement(IGraphSerializable g)
        {
            XElement result = new XElement(nameProvider.AnnotationListElement);

            foreach (Tuple<Guid, Annotation> t in g.GetAllAnnotations())
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

        private void BuildTables(IGraphSerializable g)
        {
            int typeAlias = 0;
            int nodeId = 0;

            foreach (IVertex v in g)
            {
                if (!typeAliasTable.ContainsKey(v.TypeIdentity))
                {
                    typeAliasTable.Add(v.TypeIdentity, typeAlias++);
                }
                if (!nodeIdTable.ContainsKey(v))
                {
                    nodeIdTable.Add(v, nodeId++);
                }
                foreach (IEdge e in v.Links)
                {
                    if (!typeAliasTable.ContainsKey(e.TypeIdentity))
                    {
                        typeAliasTable.Add(e.TypeIdentity, typeAlias++);
                    }
                }
            }
        }

        private XElement GenerateTypesElement()
        {
            XElement types = new XElement(nameProvider.TypeListElement);
            foreach (KeyValuePair<GraphObjectTypeInfo, int> alias in typeAliasTable)
            {
                var type = alias.Key;
                switch (alias.Key.Type)
                {
                    case GraphObjectType.Vertex:
                        types.Add(new XElement(nameProvider.VertexTypeElement,
                            new XAttribute(nameProvider.TypeKindAttribute, nameProvider.TypeKindAttributeValueVertex),
                            new XElement(nameProvider.TypeNameElement, type.Name),
                            new XElement(nameProvider.TypeDescriptionElement, new XCData(type.Description ?? "")),
                            new XAttribute(nameProvider.TypeInternalIdAttribute, alias.Value),
                            new XAttribute(nameProvider.TypeIsDirectContentAttribute, type.DirectContent),
                            new XAttribute(nameProvider.TypeExternalIdAttribute, type.Id)));
                        break;
                    case GraphObjectType.Edge:
                        types.Add(new XElement(nameProvider.EdgeTypeElement,
                            new XAttribute(nameProvider.TypeKindAttribute, nameProvider.TypeKindAttributeValueEdge),
                            new XElement(nameProvider.TypeNameElement, type.Name),
                            new XElement(nameProvider.TypeDescriptionElement, new XCData(type.Description ?? "")),
                            new XAttribute(nameProvider.TypeInternalIdAttribute, alias.Value),
                            new XAttribute(nameProvider.TypeIsOrientedEdgeAttribute, type.Oriented),
                            new XAttribute(nameProvider.TypeExternalIdAttribute, type.Id)));
                        break;
                }
            }
            return types;
        }

        private XElement GenerateNodesElement()
        {
            XElement nodes = new XElement(nameProvider.NodeListElement);
            foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
            {
                IVertex v = pair.Key;
                nodes.Add(new XElement(nameProvider.NodeElement,
                    new XAttribute(nameProvider.NodeTypeAttribute, typeAliasTable[v.TypeIdentity]),
                    new XAttribute(nameProvider.NodeLanguagedAttribute, v.LanguageLayer),
                    new XAttribute(nameProvider.NodeInternalIdAttribute, pair.Value),
                    new XAttribute(nameProvider.NodeExternalIdAttribute, v.ObjectId),
                    new XElement(nameProvider.NodeCanonicalNameElement, new XCData(v.ToString())),
                    new XElement(nameProvider.NodeContentElement, new XCData(v.SerializeAsString()))));
            }
            return nodes;
        }

        private XElement GenerateEdgesElement()
        {
            HashSet<IEdge> edgeToPersiste = new HashSet<IEdge>();
            foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
            {
                foreach (IEdge edge in pair.Key.Links)
                {
                    if (!edgeToPersiste.Contains(edge))
                    {
                        edgeToPersiste.Add(edge);
                    }
                }
            }

            XElement edges = new XElement(nameProvider.EdgeListElement);
            foreach (IEdge edge in edgeToPersiste)
            {
                XElement edgeElement = new XElement(nameProvider.EdgeElement,
                    new XAttribute(nameProvider.EdgeTypeAttribute, typeAliasTable[edge.TypeIdentity]),
                    new XAttribute(nameProvider.EdgeExternalIdAttribute, edge.ObjectId));

                foreach (IGraphObject obj in edge.GetLinkedObjects())
                {
                    if (obj.TypeIdentity.Type == GraphObjectType.Vertex)
                    {
                        edgeElement.Add(new XElement(nameProvider.EdgeTargetElement,
                            new XAttribute(nameProvider.EdgeTargetTypeAttribute, nameProvider.EdgeTargetTypeAttributeValueInternal),
                            new XAttribute(nameProvider.EdgeTargetIdAttribute, nodeIdTable[obj as IVertex])));
                    }
                    else
                    {
                        edgeElement.Add(new XElement(nameProvider.EdgeTargetElement,
                            new XAttribute(nameProvider.EdgeTargetTypeAttribute, nameProvider.EdgeTargetTypeAttributeValueExternal),
                            new XAttribute(nameProvider.EdgeTargetIdAttribute, "TODO")));
                    }
                }
                edges.Add(edgeElement);
            }
            return edges;
        }
    }
}
