namespace Leger.IO
{
    using System.IO;
    using System.Text;

    public interface IStringSerializable
    {
        void Deserialize(string reader);
        void Serialize(StringBuilder writer);
    }

    public static class Helpers
    {
        public static string SerializeAsString(this IStringSerializable obj)
        {
            StringBuilder sb = new StringBuilder();
            obj.Serialize(sb);
            return sb.ToString();
        }
    }
}