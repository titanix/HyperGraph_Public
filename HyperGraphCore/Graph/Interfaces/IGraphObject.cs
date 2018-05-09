using System;
using System.Collections.Generic;

namespace Leger
{
    /// <summary>
    /// Une classe qui implémente cette interface peut être la cible d'un lien (Edge).
    /// </summary>
    public interface IGraphObject
    {
        /// <summary>
        /// Renvoie le type d'objet du graph associé que représente l'objet.
        /// </summary>
        GraphObjectTypeInfo TypeIdentity { get; }

        /// <summary>
        /// Identifiant unique du noeud dans le graph.
        /// </summary>
        Guid ObjectId { get; }

        /// <summary>
        /// Référence vers la couche d'accès aux données.
        /// Nécessaire si l'accès 'lazy' aux éléments du graph est utilisé.
        /// </summary>
        IGraphStore GraphStore { get; set; }

        /// <summary>
        /// Renvoie tous les éléments du graph liés à cet objet.
        /// </summary>
        /// <returns></returns>
        List<IGraphObject> GetLinkedObjects();

        /// <summary>
        /// Renvoie les noeuds ou les liens liés à cet objet.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<IGraphObject> GetLinkedObjects(GraphObjectType type);

        /// <summary>
        /// Permet de fixer la liste des objets du graph.
        /// </summary>
        /// <param name="objects"></param>
        void SetLinkedObjects(IEnumerable<IGraphObject> objects);

        /// <summary>
        /// Ajoute aux liens internes de l'objet une référence
        /// vers un lien créé vers le dit objet ; cela permet
        /// de naviguer depuis l'instance vers ce lien.
        /// </summary>
        /// <param name="edge"></param>
        void AddEdgeLink(IEdge edge);

        /// <summary>
        /// Réalise l'opération inverse de AddEdgeLink.
        /// </summary>
        /// <param name="source"></param>
        void RemoveEdgeLink(IEdge edge);
    }
}