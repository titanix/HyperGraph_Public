using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Leger.IO;

namespace Leger
{
    public class Vertex<NodeContent> : GraphObject, IVertex where NodeContent : IStringSerializable, new()
    {
        private static GraphObjectTypeInfo defaultType =
            new GraphObjectTypeInfo(Guid.Parse("93de7187-c57d-4215-836a-eebd4cdbebc0"), "Generic Node", GraphObjectType.Vertex);
        protected string language;
        public const string DefaultLanguageValue = "none";

        protected NodeContent content;
        protected List<IndexedString> indexedStrings = new List<IndexedString>();

        protected Vertex() { }

        public Vertex(GraphObjectTypeInfo type, NodeContent content) :
            this(type, content, DefaultLanguageValue, Guid.NewGuid(), null)
        { }

        public Vertex(GraphObjectTypeInfo type, NodeContent content, string lang) :
            this(type, content, lang, Guid.NewGuid(), null)
        { }

        public Vertex(GraphObjectTypeInfo type, NodeContent content, string lang, Guid id)
            : this(type, content, lang, id, null) { }

        public Vertex(
            GraphObjectTypeInfo type,
            NodeContent content,
            string lang,
            Guid id,
            IGraphStore graphStore) : base()
        {
            base.id = id;
            base.graphStore = graphStore;
            this.content = content;
            this.type = type;
            this.language = lang;
        }

        public Vertex(
            GraphObjectTypeInfo type,
            NodeContent content,
            string lang,
            Guid id,
            IGraphStore graphStore,
            IEnumerable<Guid> linkedObjectIdList) : base()
        {
            base.id = id;
            base.linkedObjectIdList = linkedObjectIdList.ToList();
            base.graphStore = graphStore;

            this.content = content;
            this.type = type;
            this.language = lang;

        }

        public NodeContent Content { get => content; }

        public IEnumerable<IndexedString> IndexableStrings
        {
            get
            {
                if (indexedStrings.Count == 0)
                {
                    return new IndexedString[1] { new IndexedString(content.ToString()) };
                }
                else
                {
                    return indexedStrings;
                }
            }
        }

        public List<IEdge> Links
        {
            get
            {
                List<IEdge> result = new List<IEdge>();
                foreach (IGraphObject edge in base.GetLinkedEdges())
                {
                    result.Add(edge as IEdge);
                }
                return result;
            }
        }

        public override void AddEdgeLink(IEdge edge)
        {
            base.AddObject(edge);
            linkedObjectIdList.Add(edge.ObjectId);
        }

        public void Deserialize(string reader)
        {
            NodeContent content = new NodeContent();
            content.Deserialize(reader);
            this.content = content;
        }

        public void Serialize(StringBuilder writer)
        {
            StringBuilder sb = new StringBuilder();
            content.Serialize(sb);
            writer.Append(sb.ToString());
        }

        public override string ToString()
        {
            return content != null ? content.ToString() : "[Empty node]";
        }

        public string LanguageLayer
        {
            get
            {
                if (String.IsNullOrEmpty(language))
                {
                    throw new MemberAccessException("Value not set");
                }
                return language;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex<NodeContent>)
            {
                Vertex<NodeContent> casted = obj as Vertex<NodeContent>;
                return this.ObjectId == casted.ObjectId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ObjectId.GetHashCode();
        }
    }
}