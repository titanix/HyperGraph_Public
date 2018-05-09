namespace Leger
{
    public static class Extensions
    {
        public static string ToLetter(this GraphObjectType gt)
        {
            switch (gt)
            {
                case GraphObjectType.Vertex:
                    return "v";
                case GraphObjectType.Edge:
                    return "e";
                default:
                    return "!";
            }
        }

        public static GraphObjectType? ToGraphObjectType(this string str)
        {
            if (str.Equals("v"))
            {
                return GraphObjectType.Vertex;
            }
            if (str.Equals("e"))
            {
                return GraphObjectType.Edge;
            }
            return null;
        }
    }
}
