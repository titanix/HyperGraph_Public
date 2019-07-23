namespace Leger
{
    public interface IEdge : IGraphObject
    {
        /// <summary>
        /// Nombre d'objets reliées par le lien.
        /// </summary>
        int Arity { get; }
    }
}