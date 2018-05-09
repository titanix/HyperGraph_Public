namespace Leger
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using System.Collections.ObjectModel;

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

#if CS_6
        public GraphObjectTypeInfo TypeIdentity => type;
        public Guid ObjectId => id;
#else
        public GraphObjectTypeInfo TypeIdentity { get { return type; } }
        public Guid ObjectId { get { return id; } }
#endif

        public abstract void AddEdgeLink(IEdge edge);

        protected virtual List<IGraphObject> LinkedObjects()
        {
            GetLinkedObjects();
            return linkedObjects;
        }

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
                // throw new NullReferenceException("Missing graph store reference.");
            }

            if (linkedObjects == null)
            {
                linkedObjects = new List<IGraphObject>();
                foreach (Guid id in linkedObjectIdList)
                {
                    linkedObjects.Add(graphStore.GetObjectById(id));
                }
            }

            return linkedObjects;
        }

        public virtual List<IGraphObject> GetLinkedObjects(GraphObjectType type)
        {
            return GetLinkedObjects().Where(o => o.TypeIdentity.Type == type).ToList();
        }

        public virtual void SetLinkedObjects(IEnumerable<IGraphObject> objects)
        {
            linkedObjects = objects.ToList();
        }
    }
}