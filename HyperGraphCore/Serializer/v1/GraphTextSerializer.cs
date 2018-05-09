namespace Leger.IO
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using Leger;

    [Obsolete]
    public class GraphTextSerializer
    {
        Dictionary<GraphObjectTypeInfo, int> typeAliasTable = new Dictionary<GraphObjectTypeInfo, int>();
        Dictionary<IVertex, long> nodeIdTable = new Dictionary<IVertex, long>();

        public StringBuilder Serialize(Graph g)
        {
            StringBuilder result = new StringBuilder();
            BuildTables(g);
            result.AppendFormat("H|{0},{1},nb_alias,nb_nodes", typeAliasTable.Count, nodeIdTable.Count);
            result.AppendLine();
            result.Append(GenerateTypeTable());
            result.Append(GenerateNodeList());
            result.Append(GenerateRelationList());
            result.AppendLine();
            return result;
        }

        private void BuildTables(Graph g)
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

        private StringBuilder GenerateTypeTable()
        {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<GraphObjectTypeInfo, int> alias in typeAliasTable)
            {
                result.AppendFormat("A|{0}|{1}|{2}|{3}", alias.Key.Type.ToLetter(), alias.Value, alias.Key.Name, alias.Key.Id);
                result.AppendLine();
            }
            return result;
        }

        private StringBuilder GenerateNodeList()
        {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<IVertex, long> pair in nodeIdTable)
            {
                result.AppendFormat("N|{0}|{1}|{2}", pair.Value, typeAliasTable[pair.Key.TypeIdentity], pair.Key.SerializeAsString());
                result.AppendLine();
            }
            return result;
        }

        private StringBuilder GenerateRelationList()
        {
            StringBuilder result = new StringBuilder();
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
            foreach (IEdge edge in edgeToPersiste)
            {
                result.AppendFormat("E|");
                result.AppendFormat("{0}|", typeAliasTable[edge.TypeIdentity]);
                foreach (IVertex v in edge.GetLinkedObjects(GraphObjectType.Vertex))
                {
                    result.AppendFormat("{0},", nodeIdTable[v]);
                }
                result.Remove(result.Length - 1, 1);
                result.AppendLine();
            }
            return result;
        }
    }
}