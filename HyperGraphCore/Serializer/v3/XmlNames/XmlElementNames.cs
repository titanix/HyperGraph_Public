namespace Leger.IO
{
    public class XmlStandardElementNames : IXmlElementName
    {
        public string ResourceListElement { get { return "resources"; } }
        public string TypeListElement { get { return "types"; } }
        public string NodeListElement { get { return "nodes"; } }
        public string EdgeListElement { get { return "edges"; } }
        public string AnnotationListElement { get { return "annotations"; } }

        public string VertexTypeElement { get { return "type"; } }
        public string EdgeTypeElement { get { return "type"; } }
        public string TypeKindAttribute { get { return "kind"; } }
        public string TypeKindAttributeValueVertex { get { return "vertex"; } }
        public string TypeKindAttributeValueEdge { get { return "edge"; } }
        public string TypeInternalIdAttribute { get { return "internal_id"; } }
        public string TypeExternalIdAttribute { get { return "public_id"; } }
        public string TypeIsDirectContentAttribute { get { return "direct_content"; } }
        public string TypeIsOrientedEdgeAttribute { get { return "is_oriented"; } }
        public string TypeNameElement { get { return "short_name"; } }
        public string TypeDescriptionElement { get { return "description"; } }

        public string NodeElement { get { return "node"; } }
        public string NodeTypeAttribute { get { return "type"; } }
        public string NodeInternalIdAttribute { get { return "internal_id"; } }
        public string NodeExternalIdAttribute { get { return "public_id"; } }
        public string NodeLanguagedAttribute { get { return "lang"; } }
        public string NodeContentElement { get { return "content"; } }
        public string NodeCanonicalNameElement { get { return "canonical_name"; } }

        public string EdgeElement { get { return "edge"; } }
        public string EdgeTypeAttribute { get { return "type"; } }
        public string EdgeExternalIdAttribute { get { return "public_id"; } }
        public string EdgeTargetElement { get { return "target"; } }
        public string EdgeTargetTypeAttribute { get { return "type"; } }
        public string EdgeTargetIdAttribute { get { return "id"; } }
        public string EdgeTargetTypeAttributeValueInternal { get { return "internal"; } }
        public string EdgeTargetTypeAttributeValueExternal { get { return "external"; } }

        public string AnnotationElement { get { return "annotation"; } }
        public string AnnotationNamespaceAttribute { get { return "namespace"; } }
        public string AnnotationKeyAttribute { get { return "key"; } }
        public string AnnotationValueAttribute { get { return "value"; } }
        public string AnnotationTargetElement { get { return "target"; } }
        public string AnnotationTargetIdAttribute { get { return "id"; } }
        public string AnnotationTargetTypeAttribute { get { return "type"; } }
        public string AnnotationTargetAttributeValueInternal { get { return "internal"; } }
        public string AnnotationTargetAttributeValueExternal { get { return "external"; } }
    }
}