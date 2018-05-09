using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using Leger;

namespace Leger.IO
{
    public partial class GraphXmlDeserializer
    {
        /// <summary>
        /// Converti les références internes vers des noeuds présent dans les des déclarations de liens en références publiques (GUID).
        /// Comme la méthode est appelée est chaque fin de lecture de fichier, 'edgeDeclarations' ne contient normalement des liens
        /// comprenant des références internes à interpréter dans le cadre du fichier courant.
        /// </summary>
        /// <param name="nodeInternalTable"></param>
        void ConvertEdgeInternalReferencesToNodeToExternalReferences(IDictionary<int, IVertex> nodeInternalTable)
        {
            foreach (EdgeInfo info in edgesDeclarations.Where(ei => ei.LinkedObjects.Any(l => l.IsInt)))
            {
                foreach (IntOrGuid reference in info.LinkedObjects)
                {
                    if (reference.IsInt)
                    {
                        reference.IsInt = false;
                        reference.GuidValue = nodeInternalTable[reference.IntValue].ObjectId;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeTable"></param>
        /// <param name="nodeTable">Dictionnaire qui associe une référence interne à une instance de noeud.</param>
        void ResolveInternalEdges(GraphObjectTypeInfo[] typeTable, IDictionary<int, IVertex> nodeTable)
        {
            List<EdgeInfo> processed = new List<EdgeInfo>();
            foreach (EdgeInfo edgeInfo in edgesDeclarations.Where(i => i.ContainsExternal == false))
            {
                if (edgeInfo.Oriented)
                {
                    // TODO
                    throw new NotImplementedException();
                }
                else
                {
                    // normalement tous les id sont locaux et ne concernent que des noeuds car les relations ne peuvent être cible d'une relation que par leur id public
                    IEnumerable<IVertex> vertices = edgeInfo.LinkedObjects.Select(i => nodeTable[i.IntValue]);
                    IEdge edgeInstance = new HyperEdge(edgeInfo.PublicId, typeTable[edgeInfo.Type], vertices.ToArray());
                    edgeInstances.Add(edgeInfo.PublicId, edgeInstance);
                }
                processed.Add(edgeInfo);
            }
            foreach (EdgeInfo info in processed)
            {
                edgesDeclarations.Remove(info);
            }
        }

        void ResolveExternalEdges()
        {
            foreach (EdgeInfo edgeInfo in edgesDeclarations)
            {
                if (edgeInfo.Oriented)
                {
                    // TODO
                    throw new NotImplementedException();
                }
                else
                {
                    // on cherche les objets du graph dont les id correspondent aux cibles du lien
                    // ATTENTION si la cible est un lien, elle n'est pas forcément instanciée
                    IEnumerable<IGraphObject> graphObjects = edgeInfo.LinkedObjects.Select(ei => GetObjectById(ei.GuidValue));
                    IEdge edgeInstance = new HyperEdge(edgeInfo.PublicId, edgeInfo.GuidType, graphObjects.ToArray());
                    edgeInstances.Add(edgeInfo.PublicId, edgeInstance);
                }
            }
        }

        IGraphObject GetObjectById(Guid id)
        {
            if (this.vertexInstances.ContainsKey(id))
            {
                return vertexInstances[id];
            }
            else if (edgeInstances.ContainsKey(id))
            {
                return edgeInstances[id];
            }
            // si la valeur n'est pas trouvée on est probablement dans le cas d'un lien non instancié
            EdgeInfo edgeInfo = edgesDeclarations.Where(ed => ed.PublicId == id).FirstOrDefault();
            if (edgeInfo != null)
            {
                RecursivelyInstanciateEdge(edgeInfo);
                return GetObjectById(id);
            }
            return null;
        }

        /// <summary>
        /// Permet d'instancier récursivement les liens qui sont cibles d'un lien qui n'ont pas encore été instanciés.
        /// </summary>
        /// <param name="edgeInfo"></param>
        void RecursivelyInstanciateEdge(EdgeInfo edgeInfo)
        {
            throw new NotImplementedException();

            foreach (IntOrGuid targetObjectId in edgeInfo.LinkedObjects)
            {
                if (GetObjectById(targetObjectId.GuidValue) == null)
                {
                    // todo
                }
            }
            // instanciate edge
        }

        IVertex InstanciateNode(Type nodeConcreteType, VertexInfo nodeInfo)
        {
            return InstanciateNode(nodeConcreteType, nodeInfo.PublicId, nodeInfo.GuidType, nodeInfo.Content, nodeInfo.Language);
        }

        IVertex InstanciateNode(Type nodeConcreteType, Guid nodeId, GraphObjectTypeInfo nodeType, string nodeContent, string lang)
        {
            List<object> constructorParams = new List<object>();
            GraphObjectTypeInfo got = nodeType;
            string contentString = nodeContent;

            Type genericType = nodeConcreteType;
            IVertex v = null;
            Type vertexContent = typeof(SerializableString);
            if (vertexContentTypeMap.ContainsKey(got.Id))
            {
                vertexContent = vertexContentTypeMap[got.Id];
            }

            IStringSerializable content = Activator.CreateInstance(vertexContent) as IStringSerializable;
            content.Deserialize(contentString);

            Type instanceType = nodeConcreteType;
            if (nodeConcreteType.IsGenericTypeDefinition)
            {
                instanceType = genericType.MakeGenericType(vertexContent);
            }
            v = Activator.CreateInstance(instanceType, new object[] { got, content, lang, nodeId }) as IVertex;

            return v;
        }
    }
}
