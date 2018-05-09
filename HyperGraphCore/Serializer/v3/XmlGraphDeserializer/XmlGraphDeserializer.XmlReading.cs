using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Leger;

namespace Leger.IO
{
    public partial class GraphXmlDeserializer
    {
        private void Merge(GraphXmlDeserializer otherDeserializer)
        {
            vertexInstances = vertexInstances.Union(otherDeserializer.vertexInstances).ToDictionary(k => k.Key, v => v.Value);
            edgesDeclarations = new HashSet<EdgeInfo>(edgesDeclarations.Union(otherDeserializer.edgesDeclarations));
            annotationDeclarations = annotationDeclarations.Union(otherDeserializer.annotationDeclarations).ToList();
        }

        private GraphXmlDeserializer() { }

        private Graph LoadFile(string path)
        {
            if (localFileProvider == null)
            {
                throw new NullReferenceException("FileProvider implementation is not specified.");
            }
            var stream = localFileProvider.GetFileReader(path);
            if (stream != null)
            {
                XDocument xdoc = XDocument.Load(stream);
                stream.Close();
                return LoadGraph(xdoc);
            }
            return null;
        }

        private Graph LoadGraph(XDocument xdoc)
        {
            ParseFile(xdoc);

            ResolveExternalEdges();
            ResolveAnnotationDeclarations();

            Graph g = new Graph();
            foreach (IVertex v in vertexInstances.Values)
            {
                g.AddVertex(v);
            }

            foreach (KeyValuePair<Guid, List<Annotation>> pair in annotationsTable)
            {
                if (vertexInstances.ContainsKey(pair.Key))
                {
                    g.AddAnnotations(vertexInstances[pair.Key], pair.Value);
                }
                if (edgeInstances.ContainsKey(pair.Key))
                {
                    g.AddAnnotations(edgeInstances[pair.Key], pair.Value);
                }
            }

            return g;
        }

        private void ParseFile(XDocument xdoc)
        {
            ParseHeader(xdoc);
            LoadTypeDeclarations(xdoc);
            LoadNodeDeclarations(xdoc);
            LoadEdgeDeclarations(xdoc);
            LoadAnnotationDeclarations(xdoc);

            ResolveInternalEdges(typeNumericAliasTable, nodeNumericAliasTable);
            ConvertEdgeInternalReferencesToNodeToExternalReferences(nodeNumericAliasTable);
            //graphObjectTypeDeclarations.AddRange(typeNumericAliasTable);
        }

        private void ParseHeader(XDocument xdoc)
        {
            XElement header = xdoc.Root.Element(XmlHeaderNames.HeaderElement);
            ThrowErrorIfNull(header, "Missing or invalid header information.");

            string version = header?.Attribute(XmlHeaderNames.VersionAttribute)?.Value;

            string useShortNames = header?.Attribute(XmlHeaderNames.UseShortTagsAttribute)?.Value;
            if(useShortNames != null & useShortNames == XmlHeaderNames.StringBooleanValueTrue)
            {
                this.nameProvider = new XmlShortElementNames();
            }
        }

        private void ResolveAnnotationDeclarations()
        {
            ConvertAnnotationInternalReferences(nodeNumericAliasTable);
            foreach (AnnotationInfo decl in annotationDeclarations)
	        {
                Annotation annotation = decl.ToAnnotation();
                Guid id = decl.Target.GuidValue;
		        if (annotationsTable.ContainsKey(id))
                {
                    annotationsTable[id].Add(annotation);
                }
                else
                {
                    annotationsTable.Add(id, new List<Annotation>() { annotation });
                }
	        }
        }

        private void ConvertAnnotationInternalReferences(IDictionary<int, IVertex> nodeInternalTable)
        {
            foreach (AnnotationInfo info in annotationDeclarations.Where(ai => ai.Target.IsInt))
            {
                info.Target.IsInt = false;
                info.Target.GuidValue = nodeInternalTable[info.Target.IntValue].ObjectId;
            }
        }

        private void LoadTypeDeclarations(XDocument xdoc)
        {
            XElement typesNode = xdoc.Root.Element(nameProvider.TypeListElement);
            ThrowErrorIfNull(typesNode, $"Missing {nameProvider.TypeListElement} node.");

            headerInfo.Types = typesNode.Elements().Count();
            typeNumericAliasTable = new GraphObjectTypeInfo[headerInfo.Types];

            LoadTypes(typesNode, GraphObjectType.Vertex);
            LoadTypes(typesNode, GraphObjectType.Edge);

        }

        private void LoadNodeDeclarations(XDocument xdoc)
        {
            XElement nodeList = xdoc.Root.Element(nameProvider.NodeListElement);

            if(nodeList != null)
            {
                foreach (XElement node in nodeList.Elements(nameProvider.NodeElement))
                {
                    string type = node.Attribute(nameProvider.NodeTypeAttribute)?.Value;
                    string localId = node.Attribute(nameProvider.NodeInternalIdAttribute)?.Value;
                    string id = node.Attribute(nameProvider.NodeExternalIdAttribute)?.Value;
                    string lang = node.Attribute(nameProvider.NodeLanguagedAttribute)?.Value;
                    ThrowErrorIfNull(type, "Missing node type attribute.");
                    ThrowErrorIfNull(localId, "Missing node internal_id attribute.");
                    ThrowErrorIfNull(id, "Missing node public_id attribute.");
                    ThrowErrorIfNull(lang, "Missing node language attribute.");
                    string content = node.Element(nameProvider.NodeContentElement)?.Value;
                    string canonicalName = node.Element(nameProvider.NodeCanonicalNameElement)?.Value;
                    ThrowErrorIfNull(lang, "Missing node content element.");
                    ThrowErrorIfNull(lang, "Missing node canonical_name element.");

                    int internalId = 0;
                    if(!Int32.TryParse(localId, out internalId))
                    {
                        ThrowParseException("Invalid node internal id format.");
                    }
                    Guid externalId;
                    if(!Guid.TryParse(id, out externalId))
                    {
                        ThrowParseException("Invalid node public id format.");
                    }

                    VertexInfo vertexInfo = new VertexInfo();
                    if(!Int32.TryParse(type, out vertexInfo.Type))
                    {
                        ThrowParseException("Invalid type format.");
                    }
                    vertexInfo.CanonicalName = canonicalName;
                    vertexInfo.Content = content;
                    vertexInfo.InternalId = internalId;
                    vertexInfo.Language = lang;
                    vertexInfo.PublicId = externalId;
                    vertexInfo.GuidType = typeNumericAliasTable[vertexInfo.Type];

                    IVertex instance = InstanciateNode(concreteVertexType, vertexInfo);
                    vertexInstances.Add(vertexInfo.PublicId, instance);
                    nodeNumericAliasTable.Add(vertexInfo.InternalId, instance);
                }
            }
        }

        private void LoadEdgeDeclarations(XDocument xdoc)
        {
            XElement edgeList = xdoc.Root.Element(nameProvider.EdgeListElement);

            if (edgeList != null)
            {
                foreach (XElement edge in edgeList.Elements(nameProvider.EdgeElement))
                {
                    EdgeInfo edgeInfo = new EdgeInfo();

                    string type = edge.Attribute(nameProvider.EdgeTypeAttribute)?.Value;
                    int typeAlias;
                    if(!Int32.TryParse(type, out typeAlias))
                    {
                        ThrowParseException("Invalid edge type attribute value.");
                    }
                    edgeInfo.Type = typeAlias; // au cas où
                    edgeInfo.GuidType = typeNumericAliasTable[typeAlias];

                    string strId = edge.Attribute(nameProvider.EdgeExternalIdAttribute)?.Value;
                    if(!Guid.TryParse(strId, out edgeInfo.PublicId))
                    {
                        ThrowParseException("Invalid edge public id.");
                    }

                    foreach (XElement target in edge.Elements(nameProvider.EdgeTargetElement))
                    {
                        IntOrGuid id = new IntOrGuid();
                        string targetType = target.Attribute(nameProvider.EdgeTargetTypeAttribute)?.Value; // TODO :check
                        string strValue = target.Attribute(nameProvider.EdgeTargetIdAttribute)?.Value;
                        if (targetType == nameProvider.EdgeTargetTypeAttributeValueInternal)
                        {
                            id.IsInt = true;
                            if(!Int32.TryParse(strValue, out id.IntValue))
                            {
                                ThrowParseException("Invalid edge target internal value.");
                            }
                        }
                        else if (targetType == nameProvider.EdgeTargetTypeAttributeValueExternal)
                        {
                            id.IsInt = false;
                            if(!Guid.TryParse(strValue, out id.GuidValue))
                            {
                                ThrowParseException("Invalid edge target external value.");
                            }

                            edgeInfo.ContainsExternal = true;
                        }
                        edgeInfo.LinkedObjects.Add(id);
                    }

                    edgesDeclarations.Add(edgeInfo);
                }
            }
        }

        private void LoadAnnotationDeclarations(XDocument xdoc)
        {
            XElement annotationsNode = xdoc.Root.Element(nameProvider.AnnotationListElement);
            if(annotationsNode == null)
            {
                return;
            }

            foreach (XElement annotationNode in annotationsNode.Elements(nameProvider.AnnotationElement))
	        {
		        string @namespace = annotationNode.Attribute(nameProvider.AnnotationNamespaceAttribute)?.Value ?? "";
                string key = annotationNode.Attribute(nameProvider.AnnotationKeyAttribute)?.Value;
                string value = annotationNode.Attribute(nameProvider.AnnotationValueAttribute)?.Value;
                ThrowErrorIfNullOrEmpty(key, "Missing annotation key attribute.");
                //ThrowErrorIfNullOrEmpty(value, "Missing annotation value attribute.");

                XElement targetNode = annotationNode.Element(nameProvider.AnnotationTargetElement);
                ThrowErrorIfNull(targetNode, "Missing annotation target element.");
                string targetType = targetNode.Attribute(nameProvider.AnnotationTargetTypeAttribute)?.Value;
                ThrowErrorIfNull(targetType, "Missing annotation target type.");

                string id = targetNode.Attribute(nameProvider.AnnotationTargetIdAttribute)?.Value;
                ThrowErrorIfNullOrEmpty(id, "Annotation target identifier bad format.");
                AnnotationInfo info = new AnnotationInfo()
                {
                    Namespace = @namespace,
                    Key = key,
                    Value = value,
                    Target = new IntOrGuid()
                };

                if (targetType == nameProvider.AnnotationTargetAttributeValueInternal)
                {
                    info.Target.IsInt = true;
                    int result;
                    if(Int32.TryParse(id, out result))
                    {
                        info.Target.IntValue = result;
                    }
                    else
                    {
                        ThrowParseException("Invalid annotation id.");
                    }
                }
                else if (targetType == nameProvider.AnnotationTargetAttributeValueExternal)
                {
                    info.Target.IsInt = false;
                    Guid result;
                    if(Guid.TryParse(id, out result))
                    {
                        info.Target.GuidValue = result;
                    }
                    else
                    {
                        ThrowParseException("Invalid annotation id.");
                    }
                }
                else
                {
                    ThrowParseException("Invalid annotation type.");
                }
                annotationDeclarations.Add(info);
	        }
        }

        private void LoadTypes(XElement parentNode, GraphObjectType kind)
        {
            IEnumerable<XElement> types = kind == GraphObjectType.Vertex ?
                parentNode.Elements(nameProvider.VertexTypeElement).Where(e => e.Attribute(nameProvider.TypeKindAttribute).Value == nameProvider.TypeKindAttributeValueVertex) :
                parentNode.Elements(nameProvider.EdgeTypeElement).Where(e => e.Attribute(nameProvider.TypeKindAttribute).Value == nameProvider.TypeKindAttributeValueEdge);

            if (types != null)
            {
                foreach (XElement typeElement in types)
                {
                    string id = typeElement.Attribute(nameProvider.TypeExternalIdAttribute)?.Value;
                    ThrowErrorIfNull(id, "Missing type external id.");

                    string internalId = typeElement.Attribute(nameProvider.TypeInternalIdAttribute)?.Value;
                    ThrowErrorIfNull(id, "Missing type internal id.");
                    int internalTypeId = 0;
                    if (!Int32.TryParse(internalId, out internalTypeId))
                    {
                        ThrowParseException("Internal id not convertible to integer.");
                    }

                    string name = typeElement.Element(nameProvider.TypeNameElement)?.Value;
                    ThrowErrorIfNull(id, "Missing type name.");

                    string description = typeElement.Element(nameProvider.TypeDescriptionElement)?.Value;
                    ThrowErrorIfNull(id, "Missing type description.");

					string orientedString = typeElement.Attribute(nameProvider.TypeIsOrientedEdgeAttribute)?.Value;
					bool oriented = false;
					if (orientedString != null)
					{
						oriented = Boolean.Parse(orientedString);
					}

					string directContentString = typeElement.Attribute(nameProvider.TypeIsDirectContentAttribute)?.Value;
					bool directContent = false;
					if (directContentString != null)
					{
						directContent = Boolean.Parse(directContentString);
					}

					Guid guid = Guid.Parse(id); // TODO checker les erreurs
                    
					GraphObjectTypeInfo typeInfo = new GraphObjectTypeInfo(guid, name, kind, oriented, directContent); // TODO fixer correctement direct_content
                    typeInfo.Description = description;

                    typeNumericAliasTable[internalTypeId] = typeInfo;
                }
            }
        }

        private void FillVertexSpecificInformation(XElement node, GraphObjectTypeInfo typeInfo)
        {
            // TODO
        }

        private void FillEdgeSpecificInformation(XElement node, GraphObjectTypeInfo typeInfo)
        {
            string orientationStr = node.Attribute(nameProvider.TypeIsOrientedEdgeAttribute)?.Value;
            ThrowErrorIfNull(orientationStr, "Missing edge orientation.");
            typeInfo.Oriented = orientationStr.Equals(XmlHeaderNames.StringBooleanValueTrue);
            // TODO
        }

        private void ThrowErrorIfNull(object obj, string reason)
        {
            if(obj == null)
            {
                ThrowParseException(reason);
            }
        }

        private void ThrowErrorIfNullOrEmpty(object obj, string reason)
        {
            if(obj == null)
            {
                ThrowParseException(reason);
            }
            if(obj is string)
            {
                if(String.IsNullOrEmpty(obj as string))
                {
                    ThrowParseException(reason);
                }
            }
        }
        
        private void ThrowParseException(string reason, bool ignorable = false)
        {
            throw new XmlFileParsingException(String.Format("Error parsing file {0}. Reason: {1}.", fileName, reason)) { Ignorable = ignorable };
        }

    }
}
