using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;

using Leger.IO;

namespace Leger.Extra.SqlBinding
{
	internal class SqlVertexStore<ConcreteCommand> : SqlGraphObjectStore<ConcreteCommand>, IGraphObjectRepository<IVertex> where ConcreteCommand : DbCommand, new()
	{
		private Dictionary<Guid, IVertex> cache = new Dictionary<Guid, IVertex>();
		private IVertexStoreRawSqlProvider rawSqlProvider;

		public SqlVertexStore(DbConnection dbConnection, DbProvider<ConcreteCommand> provider, IVertexStoreRawSqlProvider rawSqlProvider)
			: base(dbConnection, "Vertex")
		{
			base.provider = provider;
			this.rawSqlProvider = rawSqlProvider;
		}

		public int Count => throw new NotImplementedException();

        public IEnumerable<IVertex> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void DeleteIfExists(IVertex o)
        {
            throw new NotImplementedException();
        }

        public IVertex GetElementById(Guid id)
        {
            if (!cache.ContainsKey(id))
            {
				string toCmd = String.Format(rawSqlProvider.SelectVertexAndPredecessors, id);
				string fromCmd = String.Format(rawSqlProvider.SelectVertexAndSuccessors, id);

                Tuple<IVertex, List<Guid>> result = null;
                Tuple<IVertex, List<Guid>> result_2 = null;

                using (DbCommand req = (ConcreteCommand)Activator.CreateInstance(typeof(ConcreteCommand), new object[] { toCmd, connection }))
                {
                    using (DbDataReader sqlReader = req.ExecuteReader())
                    {
                        result = InstanciateVertexAndNeighbors(sqlReader);
                    }
                }

                using (DbCommand req = (ConcreteCommand)Activator.CreateInstance(typeof(ConcreteCommand), new object[] { fromCmd, connection }))
                {
                    using (DbDataReader sqlReader = req.ExecuteReader())
                    {
                        result_2 = InstanciateVertexAndNeighbors(sqlReader, permuteSource: true, augmentNeighborhood: result.Item1, previousEdges: result.Item2);
                    }
                }

                if (result_2.Item1 == null)
                {
                    return result.Item1;
                }
                return result_2.Item1;
            }

            return cache[id];
        }

        private Tuple<IVertex, List<Guid>> InstanciateVertexAndNeighbors(DbDataReader sqlReader, 
            bool permuteSource = false, 
            IVertex augmentNeighborhood = null, 
            List<Guid> previousEdges = null)
        {
            int i = 0;
            VertexData origin = new VertexData();
            List<EdgeData> edges = new List<EdgeData>();
            List<VertexData> neighbors = new List<VertexData>();

            while (sqlReader.Read())
            {
                if (i == 0)
                {
                    origin.id = Guid.Parse(sqlReader["origin_vertex_id"].ToString());
                    origin.typeInfo = sqlReader["origin_vertex_type_info"].ToString();
                    origin.content = sqlReader["origin_vertex_content"].ToString();
                }

                EdgeData edge = new EdgeData();
                edge.id = Guid.Parse(sqlReader["edge_id"].ToString());
                edge.typeInfo = sqlReader["edge_type_info"].ToString();
                edge.target = Guid.Parse(sqlReader["target_vertex_id"].ToString());
                edges.Add(edge);

                VertexData nei = new VertexData();
                nei.id = Guid.Parse(sqlReader["target_vertex_id"].ToString());
                nei.typeInfo = sqlReader["target_vertex_type_info"].ToString();
                nei.content = sqlReader["target_vertex_content"].ToString();
                neighbors.Add(nei);

                i++;
            }

            foreach (VertexData vd in neighbors)
            {
                IVertex neighbor = new Vertex<SerializableString>(
                        graphStore: provider,
                        id: vd.id,
                        type: types[vd.typeInfo],
                        content: new SerializableString(vd.content),
                        lang: "NONE");
                if (!cache.ContainsKey(neighbor.ObjectId))
                {
                    cache.Add(neighbor.ObjectId, neighbor);
                }
            }

            foreach (EdgeData ed in edges)
            {
                List<Guid> linkedElements = permuteSource ? new List<Guid> { ed.target, origin.id } : new List<Guid> { origin.id, ed.target };
                IEdge edge = new HyperEdge(ed.id, types[ed.typeInfo], provider, linkedElements);
                provider.EdgeStore.Cache(edge);
            }

            List<Guid> edgeGuidList = edges.Select(e => e.id).ToList();

            if (augmentNeighborhood != null)
            {
                edgeGuidList.AddRange(previousEdges);
            }

            IVertex originVertex = new Vertex<SerializableString>(
                graphStore: provider,
                id: origin.id,
                linkedObjectIdList: edgeGuidList,
                type: types[origin.typeInfo],
                content: new SerializableString(origin.content),
                lang: "NONE");

            if (!cache.ContainsKey(origin.id))
            {
                cache.Add(origin.id, originVertex);
            }

            if (augmentNeighborhood != null)
            {
                if (!cache.ContainsKey(origin.id))
                {
                    cache.Add(origin.id, originVertex);
                }
                else
                {
                    cache[origin.id] = originVertex;
                }
            }

            return new Tuple<IVertex, List<Guid>>(originVertex, edgeGuidList);
        }

        class VertexData
        {
            public Guid id;
            public string typeInfo;
            public string content;
        }

        class EdgeData
        {
            public Guid id;
            public string typeInfo;
            public Guid target;
        }

        public IEnumerator<IVertex> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void StoreIfNotExists(IVertex o)
        {
            throw new NotImplementedException();
        }

        public List<IVertex> SearchNode(string search)
        {
			List<IVertex> result = new List<IVertex>();
            List<Guid> guids = new List<Guid>();
			string sql = String.Format(rawSqlProvider.SelectByContent, table, search);

			using (DbCommand cmd = (ConcreteCommand)Activator.CreateInstance(typeof(ConcreteCommand)))
			{
				cmd.Connection = connection;
				cmd.CommandText = sql;

				using (DbDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
                        guids.Add(Guid.Parse(reader["id"].ToString()));
					}
				}
			}

            foreach (Guid id in guids)
            {
                result.Add(GetElementById(id));
            }

            return result;
        }
    }
}