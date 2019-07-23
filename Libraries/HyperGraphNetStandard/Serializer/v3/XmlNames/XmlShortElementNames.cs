namespace Leger.IO
{
    public class XmlShortElementNames : IXmlElementName
    {
        public string ResourceListElement { get { return "rL"; } }
        public string TypeListElement { get { return "tL"; } }
        public string NodeListElement { get { return "nL"; } }
        public string EdgeListElement { get { return "eL"; } }
        public string AnnotationListElement { get { return "aL"; } }

        public string VertexTypeElement { get { return "type"; } }
        public string EdgeTypeElement { get { return "type"; } }
        public string TypeKindAttribute { get { return "kind"; } }
        public string TypeKindAttributeValueVertex { get { return "v"; } }
        public string TypeKindAttributeValueEdge { get { return "e"; } }
        public string TypeInternalIdAttribute { get { return "iid"; } }
        public string TypeExternalIdAttribute { get { return "pid"; } }
        public string TypeIsDirectContentAttribute { get { return "direct_content"; } }
        public string TypeIsOrientedEdgeAttribute { get { return "is_oriented"; } }
        public string TypeNameElement { get { return "short_name"; } }
        public string TypeDescriptionElement { get { return "description"; } }

        public string NodeElement { get { return "node"; } }
        public string NodeTypeAttribute { get { return "type"; } }
        public string NodeInternalIdAttribute { get { return "iid"; } }
        public string NodeExternalIdAttribute { get { return "pid"; } }
        public string NodeLanguagedAttribute { get { return "lang"; } }
        public string NodeContentElement { get { return "content"; } }
        public string NodeCanonicalNameElement { get { return "canonical_name"; } }

        public string EdgeElement { get { return "e"; } }
        public string EdgeTypeAttribute { get { return "t"; } }
        public string EdgeExternalIdAttribute { get { return "pid"; } }
        public string EdgeTargetElement { get { return "t"; } }
        public string EdgeTargetTypeAttribute { get { return "type"; } }
        public string EdgeTargetIdAttribute { get { return "id"; } }
        public string EdgeTargetTypeAttributeValueInternal { get { return "i"; } }
        public string EdgeTargetTypeAttributeValueExternal { get { return "e"; } }

        public string AnnotationElement { get { return "a"; } }
        public string AnnotationNamespaceAttribute { get { return "n"; } }
        public string AnnotationKeyAttribute { get { return "k"; } }
        public string AnnotationValueAttribute { get { return "v"; } }
        public string AnnotationTargetElement { get { return "t"; } }
        public string AnnotationTargetIdAttribute { get { return "id"; } }
        public string AnnotationTargetTypeAttribute { get { return "type"; } }
        public string AnnotationTargetAttributeValueInternal { get { return "i"; } }
        public string AnnotationTargetAttributeValueExternal { get { return "e"; } }
    }
}