namespace Leger.IO
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Leger;

    [Obsolete("Use the XML format and related classes instead.")]
    public partial class GraphTextDeserializer
    {
        Dictionary<Guid, Type> typeAssociation = new Dictionary<Guid, Type>();

        public void AssociateGraphTypeToConcreteClass<Class>(Guid guid)
        {
            if (!typeAssociation.ContainsKey(guid))
            {
                typeAssociation.Add(guid, typeof(Class));
            }
        }

        /// <summary>
        /// Permet de définir la classe concrète qui fera office d'implémentation d'un IEdge ou IVertex.
        /// </summary>
        /// <typeparam name="Class"></typeparam>
        /// <param name="type"></param>
        public void AssociateGraphTypeToConcreteClass<Class>(GraphObjectTypeInfo type)
        {
            AssociateGraphTypeToConcreteClass<Class>(type.Id);
        }

        public void AssociateGraphTypeToConcreteClass<Class>(string guid)
        {
            AssociateGraphTypeToConcreteClass<Class>(Guid.Parse(guid));
        }

        Dictionary<Guid, Type> vertexContentTypeMap = new Dictionary<Guid, Type>();
        /// <summary>
        /// Permet d'associer le type qui représente le contenu d'un noeud si celui celui-ci
        /// doit être différent de SerializableString.
        /// </summary>
        /// <typeparam name="Class"></typeparam>
        /// <param name="type"></param>
        public void AssociateVertexContentTypeToConcreteClass<Class>(GraphObjectTypeInfo type)
            where Class : IStringSerializable, new()
        {
            vertexContentTypeMap.Add(type.Id, typeof(Class));
        }

        int lineNumber;

        public Graph ReadGraph(StreamReader sr)
        {
            Graph graph = new Graph();
            GraphObjectTypeInfo[] aliasTable = new GraphObjectTypeInfo[0];
            IVertex[] nodesTable = new IVertex[0];
            lineNumber = 0;

            while (!sr.EndOfStream)
            {
                lineNumber++;
                string line = sr.ReadLine();
                string[] parts = line.Split('|');
                switch (parts[0])
                {
                    case "H":
                        HeaderInfo hInfo = ParseHeader(parts[1]);
                        aliasTable = new GraphObjectTypeInfo[hInfo.DeclaredTypes];
                        nodesTable = new IVertex[hInfo.Vertices];
                        break;
                    case "A":
                        TypeInfo typeInfo = ParseTypeDeclaration(line);
                        aliasTable[typeInfo.LocalId] = new GraphObjectTypeInfo(typeInfo.ExternalId, typeInfo.Name, typeInfo.ObjectType);            
                        break;
                    case "N":
                        List<object> constructorParams = new List<object>();
                        GraphObjectTypeInfo got = aliasTable[Int32.Parse(parts[2])];
                        string contentString = parts[3];

                        // si un type deserialisable concret est enregistré
                        // on crée une instance 'i' de IStringSerializable à partir du type enregistré
                        // on désérialise la chaîne depuis 'i'
                        // on crée une instance de StringSerializableVertex dont on fixe le contenu à 'i'
                        // sinon
                        // on crée une instance de Vertex<string> et on y la chaîne 'content'

                        Type genericType = typeof(Vertex<>);
                        IVertex v = null;
                        Type vertexContent = typeof(SerializableString);
                        if (vertexContentTypeMap.ContainsKey(got.Id))
                        {
                            vertexContent = vertexContentTypeMap[got.Id];
                        }

                        IStringSerializable content = Activator.CreateInstance(vertexContent) as IStringSerializable;
                        content.Deserialize(contentString);                                                        
                        Type specializedGenericType = genericType.MakeGenericType(vertexContent);
                        v = Activator.CreateInstance(specializedGenericType, new object[] { got, content }) as IVertex;

                        nodesTable[Int32.Parse(parts[1])] = v;
                        break;
                    case "E":
                        List<IVertex> vertices = new List<IVertex>();
                        GraphObjectTypeInfo type = aliasTable[Int32.Parse(parts[1])];
                        foreach (string v_str in parts[2].Split(','))
                        {
                            vertices.Add(nodesTable[long.Parse(v_str)]);
                        }
                        InstanciateEdge(type, typeof(HyperEdge), vertices);
                        break;
                }
            }

            foreach (IVertex node in nodesTable)
            {
                graph.AddVertex(node as IVertex);
            }
            return graph;
        }

        private void ThrowParseException(string reason)
        {
#if CS_6
            throw new TextFileParsingException($"Error parsing file at line {lineNumber}. Reason: {reason}.");
#else
            throw new TextFileParsingException(String.Format("Error parsing file at line {0}. Reason: {1}.", lineNumber, reason));
#endif
        }

        private HeaderInfo ParseHeader(string headerContent)
        {
            string[] header = headerContent.Split(',');
            HeaderInfo info = new HeaderInfo();

            try
            {
                info.DeclaredTypes = Int32.Parse(header[0]);
                info.Vertices = Int32.Parse(header[1]);
            }
            catch
            {
                ThrowParseException("Bad header format.");
            }

            return info;
        }

        private TypeInfo ParseTypeDeclaration(string declaration)
        {
            TypeInfo info = new TypeInfo();
            string[] parts = declaration.Split('|');

            try
            {
                // Parsing du type d'objet
                GraphObjectType? got = parts[1].ToGraphObjectType();
                if (got.HasValue)
                {
                    info.ObjectType = got.Value;
                }
                else
                {
                    ThrowParseException("Bad graph object type.");
                }
                // Parsing de l'id interne
                int internalTypId = 0;
                if (Int32.TryParse(parts[2], out internalTypId))
                {
                    info.LocalId = internalTypId;
                }
                else
                {
                    ThrowParseException("Integer expected for type internal id.");
                }
                // Parsing du nom
                info.Name = parts[3];
                // Parsing de l'identifiant externe
                info.ExternalId = Guid.Parse(parts[4]);
            }
            catch(IndexOutOfRangeException)
            {
                ThrowParseException("Missing data field.");
            }
            catch(FormatException)
            {
                ThrowParseException("Bad GUID format.");
            }

            return info;
        }

        private object InstanciateEdge(GraphObjectTypeInfo type, Type defaultConcreteType, IEnumerable<object> additionalParams)
        {
            List<object> constructorParams = new List<object>();
            Type concreteType = defaultConcreteType;
            constructorParams.Add(type);

            if (typeAssociation.ContainsKey(type.Id))
            {
                concreteType = typeAssociation[type.Id];
            }
            constructorParams.AddRange(additionalParams);

            return Activator.CreateInstance(concreteType, constructorParams.ToArray());
        }

        private struct HeaderInfo
        {
            internal int DeclaredTypes;
            internal int Vertices;
        }

        internal struct TypeInfo
        {
            internal GraphObjectType ObjectType;
            internal int LocalId;
            internal string Name;
            internal Guid ExternalId;
        }
    }
}