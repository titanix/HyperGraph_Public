using System.Text;

namespace Leger.IO
{
    public class SerializableString : IStringSerializable
    {
        string value;

        public SerializableString() { }

        public SerializableString(string value)
        {
            this.value = value;
        }

        public string Value { get { return value; } }

        public void Deserialize(string reader)
        {
            value = reader;
        }

        public void Serialize(StringBuilder writer)
        {
            writer.Append(value);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
