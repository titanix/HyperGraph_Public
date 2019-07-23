using System;
using System.Collections.Generic;

namespace Leger
{
    public interface IGraphSerializable : IEnumerable<IVertex>
    {
        List<Annotation> GetAnnotations(IGraphObject @object);
        List<Tuple<Guid, Annotation>> GetAllAnnotations();
    }
}