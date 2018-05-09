namespace Leger
{
    using System;
    using System.Collections.Generic;

    public interface IGraphStore
    {
        IGraphObject GetObjectById(Guid id);
        IEnumerable<IVertex> SearchNodes(string search);
    }
}