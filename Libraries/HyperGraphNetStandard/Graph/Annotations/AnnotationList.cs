using System;
using System.Collections.Generic;

namespace Leger
{
    /// <summary>
    ///  Used to manipulate a list of annotations outside a Graph instance.
    /// </summary>
    public class AnnotationList : List<Tuple<Guid, Annotation>>
    {
        public AnnotationList() { }

        public AnnotationList(List<Tuple<Guid, Annotation>> arg)
        {
            AddRange(arg);
        }
    }
}
