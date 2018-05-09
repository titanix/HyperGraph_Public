using System;

namespace Leger
{
    public class NonOrientedEdgeTypeInfo : GraphObjectTypeInfo
    {
        public NonOrientedEdgeTypeInfo(Guid guid, string name, string description = "")
            : base(guid, name, GraphObjectType.Edge, false, false) { Description = description; }

        public NonOrientedEdgeTypeInfo(string guid, string name, string description = "")
            : this(Guid.Parse(guid), name, description) { }
    }
}