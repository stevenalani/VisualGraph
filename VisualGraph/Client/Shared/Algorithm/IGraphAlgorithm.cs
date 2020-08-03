using System;
using System.Collections.Generic;
using VisualGraph.Client.Shared.Models;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared.Algorithm
{
    /// <summary>
    /// Interface für Algorithmen
    /// </summary>
    public interface IGraphAlgorithm
    {
        /// <summary>
        /// Bereichnet die günstigste Route vom Startknoten zum Endknoten
        /// </summary>
        /// <param name="startnodeId">ID des Startknotens</param>
        /// <param name="endnodeId">ID des Endknotens</param>
        /// <returns>Route als Liste mit Knoten und Kosten Paaren</returns>
        public List<Tuple<Node, double>> GetShortestRoute(string startnodeId, string endnodeId);
        /// <summary>
        /// Führt den Algorithmus aus
        /// </summary>
        /// <param name="auto">Wenn wahr, wird der Algorithmus vollständig ausgeführt. Sonst nur eine Iteration</param>
        /// <returns>Anzahl der übrigen Iterationen</returns>
        public int Iterate(bool auto = false);
        /// <summary>
        /// Name des Algorithmus
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Ergebnisse des Algorithmus
        /// </summary>
        public List<AlgorithmResultTuple> Results { get; }
        /// <summary>
        /// Start Knoten für den der Algorithmus ausgeführt werden soll
        /// </summary>
        public Node StartNode { get; }
        /// <summary>
        /// End- Knoten für die zu berechnende Route
        /// </summary>
        public Node EndNode { get; }
        /// <summary>
        /// Der Algorithmus funktioniert für negative Kanten
        /// </summary>
        public bool CanHandleNegativeEgdes { get; }
        /// <summary>
        /// Verbleibende Iterationen bis der Algorithmus vollständig ausgeführt wurde 
        /// </summary>
        public int RemainingSteps { get; }


    }
}
