using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IGraphStore
    {
        IGraphObject GetObjectById(Guid id);
    }
}