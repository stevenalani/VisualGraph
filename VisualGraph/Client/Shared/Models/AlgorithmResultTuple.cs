using System;
using System.Collections.Generic;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared.Models
{
    /// <summary>
    /// Stellt die resultate einer Algorithmus- Iteration dar. 
    /// </summary>
    public class AlgorithmResultTuple : Tuple<Node, Dictionary<Node, Node>, Dictionary<Node, double>>
    {
        /// <summary>
        /// Knoten der Algorithmus Iteration
        /// </summary>
        public Node CurrentNode => Item1;
        /// <summary>
        /// Aktuelles Wörterbuch mit Vorgängern aller Knoten. 
        /// </summary>
        public Dictionary<Node, Node> Previous => Item2;
        /// <summary>
        /// Aktuelles Wörterbuch mit Kosten aller Knoten. 
        /// </summary>
        public Dictionary<Node, double> Distances => Item3;
        /// <summary>
        /// Erstellt Instanz der Klasse
        /// </summary>
        /// <param name="currentNode">Knoten der aktuellen Iteration</param>
        /// <param name="previous">Aktuelles Wörterbuch mit Vorgängern aller Knoten</param>
        /// <param name="distances">Aktuelles Wörterbuch mit Kosten aller Knoten</param>
        public AlgorithmResultTuple(Node currentNode, Dictionary<Node, Node> previous, Dictionary<Node, double> distances) : base(currentNode, previous, distances) { }
    }
}
