using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Leger.IO
{
    public partial class GraphXmlDeserializer
    {
        public ExternalRessourcesLoadingPolicy FileLoadingPolicy
        {
            get { return fileLoadingPolicy; }
            set { fileLoadingPolicy = value; }
        }

        public static Graph GetGraphInstance(string resourcePath, IFileProvider fileProvider)
        {
            return GetGraphInstance(resourcePath, fileProvider, ExternalRessourcesLoadingPolicy.None);
        }

        public static Graph GetGraphInstance(string resourcePath)
        {
            return GetGraphInstance(resourcePath, new FileLoader(), ExternalRessourcesLoadingPolicy.None);
        }

        public static Graph LoadFromXml(XDocument xml)
        {
            GraphXmlDeserializer deserializer = new GraphXmlDeserializer();
            return deserializer.LoadGraph(xml);
        }

        public static AnnotationList LoadAnnotationListFromXml(XDocument xml)
        {
            GraphXmlDeserializer deserializer = new GraphXmlDeserializer();
            return deserializer.LoadAnnotationListFromXmlPrivate(xml);
        }

        private AnnotationList LoadAnnotationListFromXmlPrivate(XDocument xml)
        {
            AnnotationList result = new AnnotationList();

            LoadAnnotationDeclarations(xml);
            ResolveAnnotationDeclarations();

            foreach (KeyValuePair<Guid, List<Annotation>> entry in annotationsTable)
            {
                foreach (Annotation annotation in entry.Value)
                {
                    result.Add(new Tuple<Guid, Annotation>(entry.Key, annotation));
                }
            }

            return result;
        }

        /// <summary>
        /// Load a graph from a file in the v3 XML format.
        /// </summary>
        /// <param name="ressourcePath">Path to the XML file.</param>
        /// <param name="fileProvider">Class instance to get a Stream from the path.</param>
        /// <param name="loadingPolicy"></param>
        /// <returns></returns>
        [Obsolete]
        public static Graph GetGraphInstance(string resourcePath, IFileProvider fileProvider, ExternalRessourcesLoadingPolicy loadingPolicy)
        {
            GraphXmlDeserializer parser = new GraphXmlDeserializer() { localFileProvider = fileProvider, FileLoadingPolicy = loadingPolicy };
            return parser.LoadFile(resourcePath);
        }

        public static Graph GetGraphInstance(string resourcePath, IFileProvider fileProvider, Type vertexImplementation)
        {
            GraphXmlDeserializer parser = new GraphXmlDeserializer() { localFileProvider = fileProvider, FileLoadingPolicy = ExternalRessourcesLoadingPolicy.None };
            parser.OverrideConcreteVertexype(vertexImplementation);
            return parser.LoadFile(resourcePath);
        }

        public static Graph LoadMultipleFiles(IEnumerable<string> resourcePaths, IFileProvider fileProvider, ExternalRessourcesLoadingPolicy loadingPolicy)
        {
            if (fileProvider == null)
            {
                throw new NullReferenceException("FileProvider implementation is not specified.");
            }
            IEnumerable<StreamReader> streams = new List<StreamReader>();
            streams = resourcePaths.Select(p => fileProvider.GetFileReader(p));

            List<GraphXmlDeserializer> deserializers = new List<GraphXmlDeserializer>();

            foreach (StreamReader stream in streams)
            {
                XDocument xdoc = XDocument.Load(stream);
                stream.Close();
                GraphXmlDeserializer gds = new GraphXmlDeserializer();
                gds.ParseFile(xdoc);
                deserializers.Add(gds);
            }

            GraphXmlDeserializer main = deserializers[0];
            for (int i = 1; i < deserializers.Count; i++)
            {
                main.Merge(deserializers[i]);
            }
            main.ResolveExternalEdges();
            main.ResolveAnnotationDeclarations();

            Graph g = new Graph();
            foreach (IVertex v in main.vertexInstances.Values)
            {
                g.AddVertex(v);
            }
            foreach (var annotationPair in main.annotationsTable)
            {
                IGraphObject target = g.GetElementById(annotationPair.Key);
                g.AddAnnotations(target, annotationPair.Value);
            }

            return g;
        }

        public void AssociateGraphTypeToConcreteClass<Class>(Guid guid)
        {
            if (!typeAssociation.ContainsKey(guid))
            {
                typeAssociation.Add(guid, typeof(Class));
            }
        }

        /// <summary>
        /// Permet d'associer le type qui représente le contenu d'un noeud si celui celui-ci
        /// doit être différent de SerializableString.
        /// </summary>
        /// <typeparam name="Class"></typeparam>
        /// <param name="type"></param>
        public void AssociateVertexContentTypeToDeserializer<Class>(GraphObjectTypeInfo type)
            where Class : IStringSerializable, new()
        {
            vertexContentTypeMap.Add(type.Id, typeof(Class));
        }

        Type concreteVertexType = typeof(Vertex<>);

        /// <summary>
        /// Modifie le type concret à utiliser pour désérialiser les noeuds du graph.
        /// Ce type doit avoir un constructeur de signature :
        /// (GraphObjectTypeInfo type, NodeContent content, string lang, Guid id)
        /// </summary>
        /// <param name="type"></param>
        public void OverrideConcreteVertexype(Type type)
        {
            concreteVertexType = type;
        }
    }
}