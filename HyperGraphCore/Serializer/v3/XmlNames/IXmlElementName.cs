namespace Leger.IO
{
    interface IXmlElementName
    {
        string ResourceListElement { get; }
        string TypeListElement { get; }
        string NodeListElement { get; }
        string EdgeListElement { get; }
        string AnnotationListElement { get; }

        string VertexTypeElement { get; }
        string EdgeTypeElement { get; }
        string TypeKindAttribute { get; }
        string TypeKindAttributeValueVertex { get; }
        string TypeKindAttributeValueEdge { get; }
        string TypeInternalIdAttribute { get; }
        string TypeExternalIdAttribute { get; }
        string TypeIsDirectContentAttribute { get; }
        string TypeIsOrientedEdgeAttribute { get; }
        string TypeNameElement { get; }
        string TypeDescriptionElement { get; }

        string NodeElement { get; }
        string NodeTypeAttribute { get; }
        string NodeInternalIdAttribute { get; }
        string NodeExternalIdAttribute { get; }
        string NodeLanguagedAttribute { get; }
        string NodeContentElement { get; }
        string NodeCanonicalNameElement { get; }

        string EdgeElement { get; }
        string EdgeTypeAttribute { get; }
        string EdgeExternalIdAttribute { get; }
        string EdgeTargetElement { get; }
        string EdgeTargetTypeAttribute { get; }
        string EdgeTargetIdAttribute { get; }
        string EdgeTargetTypeAttributeValueInternal { get; }
        string EdgeTargetTypeAttributeValueExternal { get; }

        string AnnotationElement { get; }
        string AnnotationNamespaceAttribute { get; }
        string AnnotationKeyAttribute { get; }
        string AnnotationValueAttribute { get; }
        string AnnotationTargetElement { get; }
        string AnnotationTargetIdAttribute { get; }
        string AnnotationTargetTypeAttribute { get; }
        string AnnotationTargetAttributeValueInternal { get; }
        string AnnotationTargetAttributeValueExternal { get; }
    }
}
