namespace Leger
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public abstract class GraphObject : IGraphObject
    {
        protected IGraphStore graphStore;
        protected List<Guid> linkedObjectIdList = new List<Guid>();
        protected List<IGraphObject> linkedObjects;
        protected GraphObjectTypeInfo type;
        protected Guid id;

        public IGraphStore GraphStore
        {
            get { return graphStore; }
            set { graphStore = value; }
        }

        public GraphObjectTypeInfo TypeIdentity { get => type; }
        public Guid ObjectId { get => id; }

        public abstract void AddEdgeLink(IEdge edge);

        protected virtual void AddObject(IGraphObject obj)
        {
            GetLinkedObjects();
            linkedObjectIdList.Add(obj.ObjectId);
            linkedObjects.Add(obj);
        }

        public virtual void RemoveEdgeLink(IEdge source)
        {
            if (linkedObjects.Contains(source))
            {
                linkedObjects.Remove(source);
                linkedObjectIdList.Remove(source.ObjectId); // TODO null check
            }
        }

        public virtual List<IGraphObject> GetLinkedObjects()
        {
            if (graphStore == null)
            {
                //throw new NullReferenceException("Missing graph store reference.");
            }

            if (linkedObjects == null)
            {
                linkedObjects = new List<IGraphObject>();
                foreach (Guid id in linkedObjectIdList)
                {
                    // TODO: une méthode sur le graph store qui permet de récupérer des objets en batch
                    // GetObjectsByIds(IEnumerable<Guid>)
                    linkedObjects.Add(graphStore.GetObjectById(id));
                }
            }

            return linkedObjects;
        }

        [Obsolete("This method exists only for compatibility with the Android application.")]
        public virtual List<IGraphObject> GetLinkedObjects(GraphObjectType type)
        {
            return GetLinkedObjects().Where(o => o.TypeIdentity.Type == type).ToList();
        }

        public virtual List<IVertex> GetLinkedNodes()
        {
            IList<IGraphObject> temp = GetLinkedObjects().Where(o => o.TypeIdentity.Type == GraphObjectType.Vertex).ToList();

            List<IVertex> result = new List<IVertex>();
            foreach (IGraphObject graphObj in temp)
            {
                IVertex vertex = graphObj as IVertex;
                if (graphObj != null)
                {
                    result.Add(vertex);
                }
            }

            return result;
        }

        public virtual List<IEdge> GetLinkedEdges()
        {
            IList<IGraphObject> temp = GetLinkedObjects().Where(o => o.TypeIdentity.Type == GraphObjectType.Edge).ToList();
            var debug = GetLinkedObjects();

            List<IEdge> result = new List<IEdge>();
            foreach (IGraphObject graphObj in temp)
            {
                IEdge edge = graphObj as IEdge;
                if (graphObj != null)
                {
                    result.Add(edge);
                }
            }

            return result;
        }

        public virtual void SetLinkedObjects(IEnumerable<IGraphObject> objects)
        {
            if (objects == null)
            {
                linkedObjects = null;
            }
            else
            {
                linkedObjects = objects.ToList();
            }
        }
    }
}