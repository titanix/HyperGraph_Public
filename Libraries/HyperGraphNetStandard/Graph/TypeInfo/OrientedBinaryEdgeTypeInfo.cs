using System;

namespace Leger
{
    public class OrientedBinaryEdgeTypeInfo : GraphObjectTypeInfo
    {
        public OrientedBinaryEdgeTypeInfo(Guid guid, string name, string description = "")
            : base(guid, name, GraphObjectType.Edge, true, false) { Description = description; }

        public OrientedBinaryEdgeTypeInfo(string guid, string name, string description = "")
            : this(Guid.Parse(guid), name, description) { }
    }
}