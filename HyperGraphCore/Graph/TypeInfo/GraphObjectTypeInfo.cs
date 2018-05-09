namespace Leger
{
    using System;

    public class GraphObjectTypeInfo
    {
        /// <summary>
        /// Identifiant unique (GUID) du type de noeud.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Nom lisible par un humain du type de noeud.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description à destination d'un humain du type et de ses données internes.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indique s'il s'agit d'un type noeud ou arc.
        /// </summary>
        public GraphObjectType Type { get; private set; }

        /// <summary>
        /// Spécifique aux objets de type Edge.
        /// Indique si l'ordre de déclarations des liens est important ou non dans l'interprétation du type.
        /// </summary>
        public bool Oriented { get; internal set; }

        /// <summary>
        /// Spécifique aux objets de type Vertex.
        /// Indique si le contenu du noeud, lorsqu'il est une chaîne de caractères, peut être affichés directement à l'utilisateur.
        /// </summary>
        public bool DirectContent { get; private set; }

        public GraphObjectTypeInfo(Guid id, string name, GraphObjectType type, bool oriented_edge = false, bool direct_content = false)
        {
            if (type == GraphObjectType.Edge && direct_content == true)
            {
                throw new ArgumentException();
            }
            if (type == GraphObjectType.Vertex && oriented_edge == true)
            {
                throw new ArgumentException();
            }
            Id = id;
            Name = name;
            Type = type;
            Oriented = oriented_edge;
            DirectContent = direct_content;
        }

        public GraphObjectTypeInfo(string id, string name, GraphObjectType type, bool oriented_edge = false, bool direct_content = false)
            : this(Guid.Parse(id), name, type, oriented_edge, direct_content) { }

        public override bool Equals(object obj)
        {
            if (obj is GraphObjectTypeInfo)
            {
                GraphObjectTypeInfo nt = (GraphObjectTypeInfo)obj;
                return Id.Equals(nt.Id);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}