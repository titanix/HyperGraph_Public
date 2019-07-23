using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IVertexIndex
    {
        string Name { get; }
        //void Add(IVertex vertex, params string[] strings);
        Dictionary<string, List<IVertex>> ToDictionary();
    }
}
