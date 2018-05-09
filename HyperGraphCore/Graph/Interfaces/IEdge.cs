namespace Leger
{
    using System.Collections.ObjectModel;

    using Leger.IO;

    public interface IEdge : IGraphObject
    {
        /// <summary>
        /// Nombre d'objets reliées par le lien.
        /// </summary>
        int Arity { get; }
    }
}