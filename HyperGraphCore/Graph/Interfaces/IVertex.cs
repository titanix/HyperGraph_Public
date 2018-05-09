using System.Collections.Generic;

using Leger.IO;

namespace Leger
{
    public interface IVertex : IGraphObject, IStringSerializable
    {
        IEnumerable<IndexedString> IndexableStrings { get; }
        List<IEdge> Links { get; }
        string LanguageLayer { get; }
    }
}