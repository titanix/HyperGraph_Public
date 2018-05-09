using System;
using System.Collections.Generic;

using Leger;

namespace Leger.IO
{
    public partial class GraphXmlDeserializer
    {
        IXmlElementName nameProvider = new XmlStandardElementNames();

        /// <summary>
        /// Nom du fichier en cours de traitement.
        /// </summary>
        string fileName;

        /// <summary>
        /// Instance d'un composant qui permet l'accès aux fichiers sur la plateforme d'exécution.
        /// </summary>
        IFileProvider localFileProvider;

        /// <summary>
        /// Association entre un type d'objet du graph et une classe concrète dérivée de IEdge ou Ivertex.
        /// </summary>
        Dictionary<Guid, Type> typeAssociation = new Dictionary<Guid, Type>();

        /// <summary>
        /// Association entre un type de noeud et le type concret qui représente son contenu.
        /// </summary>
        Dictionary<Guid, Type> vertexContentTypeMap = new Dictionary<Guid, Type>();

        /// <summary>
        /// Liste des déclarations de liens dans les fichiers lues.
        /// </summary>
        HashSet<EdgeInfo> edgesDeclarations = new HashSet<EdgeInfo>();

        /// <summary>
        /// Liste des déclarations de noeuds offrant un accès par leur identifiant.
        /// </summary>
        Dictionary<Guid, IVertex> vertexInstances = new Dictionary<Guid, IVertex>();

        /// <summary>
        /// Listes des instances de liens (obtenues après résolutions des déclaration de liens).
        /// </summary>
        Dictionary<Guid, IEdge> edgeInstances = new Dictionary<Guid, IEdge>();

        /// <summary>
        /// Listes des annotations chargées depuis les fichiers lus.
        /// </summary>
        Dictionary<Guid, List<Annotation>> annotationsTable = new Dictionary<Guid, List<Annotation>>();

        /// <summary>
        /// Définit de quelle façon sont chargés (ou non) les fichiers déclarés dans le ou les fichiers lus.
        /// </summary>
        ExternalRessourcesLoadingPolicy fileLoadingPolicy = ExternalRessourcesLoadingPolicy.None;

        HeaderInfo headerInfo = new HeaderInfo();
        List<AnnotationInfo> annotationDeclarations = new List<AnnotationInfo>();
        GraphObjectTypeInfo[] typeNumericAliasTable;
        Dictionary<int, IVertex> nodeNumericAliasTable = new Dictionary<int, IVertex>();
    }
}