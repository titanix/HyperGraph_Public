using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace Leger
{
    /// <summary>
    /// Lien qui garantie que l'ordre des objets qu'il lie n'est pas modifié en interne.
    /// </summary>
    public class HyperEdge : GraphObject, IEdge
    {
        [Obsolete("Reserved for reflection use by GraphTextDeserializer.")]
        public HyperEdge(GraphObjectTypeInfo type, params IGraphObject[] sources) :
            this(Guid.NewGuid(), type, sources)
        { }

        internal HyperEdge(Guid publicId, GraphObjectTypeInfo type, params IGraphObject[] sources)
        {
            base.id = publicId;
            base.type = type;
            foreach (IGraphObject obj in sources)
            {
                base.AddObject(obj);
            }

            bool selfReferenceSkipped = false;
            foreach (IGraphObject v in base.GetLinkedObjects())
            {
                if (sources.Length == 2 && sources[0].ObjectId == sources[1].ObjectId && !selfReferenceSkipped)
                {
                    selfReferenceSkipped = true;
                    continue;
                }
                v.AddEdgeLink(this);
            }
        }

        internal HyperEdge(Guid publicId, GraphObjectTypeInfo type, IGraphStore graphStore, IEnumerable<Guid> linkedObjectIdList) : base()
        {
            base.id = publicId;
            base.type = type;
            base.graphStore = graphStore;
            base.linkedObjectIdList = linkedObjectIdList.ToList();
        }

        public int Arity
        {
            get
            {
                if (base.linkedObjects == null)
                {
                    return base.linkedObjectIdList.Count;
                }
                else
                {
                    return base.linkedObjects.Count;
                }
            }
        }

        public override void AddEdgeLink(IEdge edge)
        {
            base.AddObject(edge);
        }
    }
}