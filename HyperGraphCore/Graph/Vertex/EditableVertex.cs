using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Leger.IO;

namespace Leger
{
    public class EditableVertex : Vertex<SerializableString>
    {
        public EditableVertex() { }

        public EditableVertex(GraphObjectTypeInfo type, SerializableString content, string lang, Guid id)
            : base(type, content, lang, id)
        { }

        public void SetType(GraphObjectTypeInfo type)
        {
            base.type = type;
        }

        public void SetLanguage(string language)
        {
            base.language = language;
        }

        public void SetContent(string content)
        {
            SerializableString s = new SerializableString(content);
            base.content = s;
        }

        public void SetIndexableStrings(string[] strings)
        {
            throw new NotImplementedException();
            //base.indexedStrings = strings.ToList();
        }

        public void SetIdentifier(Guid id)
        {
            base.id = id;
        }
    }
}
