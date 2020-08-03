using System;
using System.Collections.Generic;
using System.Linq;
using VisualGraph.Client.Shared.Models;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared.Algorithm
{
    /// <summary>
    /// Dijkstra Algorithmus. 
    /// </summary>
    public class DijkstraAlgorithm : IGraphAlgorithm
    {
        BasicGraphModel Model;
        List<Node> Q;
        Node currentNode;
        /// <summary>
        /// Start Knoten für den der Algorithmus ausgeführt werden soll
        /// </summary>
        public Node StartNode { get; private set; }
        /// <summary>
        /// End- Knoten für die zu berechnende Route
        /// </summary>
        public Node EndNode { get; set; }
        Dictionary<Node, double> distances;
        Dictionary<Node, Node> Previous;
        /// <summary>
        /// Anzahl der bereits ausgeführten Iterationen
        /// </summary>
        public int StepCount;
        /// <summary>
        /// Ergebnisse des Algorithmus
        /// </summary>
        public List<AlgorithmResultTuple> Results { get; private set; }
        /// <summary>
        /// Verbleibende Iterationen bis der Algorithmus vollständig ausgeführt wurde 
        /// </summary>
        public int RemainingSteps => Q.Count;
        /// <summary>
        /// Name des Algorithmus
        /// </summary>
        public string Name => "Dijkstra Algorithm";
        /// <summary>
        /// Der Algorithmus funktioniert für negative Kanten
        /// </summary>
        public bool CanHandleNegativeEgdes => false;
        /// <summary>
        /// Erstellt eine Instanz des Algorithmus
        /// </summary>
        /// <param name="model">Graph Model</param>
        /// <param name="startNodeId">ID des Startknotens</param>
        public DijkstraAlgorithm(BasicGraphModel model, string startNodeId = "-1")
        {
            Model = model;
            Init(startNodeId);
        }
        private void Init(string startNodeId = "-1")
        {
            Results = new List<AlgorithmResultTuple>();
            distances = new Dictionary<Node, double>();
            Previous = new Dictionary<Node, Node>();
            Results = new List<AlgorithmResultTuple>();
            StepCount = 0;
            Q = Model.Nodes.OrderBy(x => x.Id).ToList();
            StartNode = currentNode = startNodeId != "-1" ? Q.First(x => x.Id == startNodeId) : Q[0];
            foreach (var node in Q)
            {
                if (node != currentNode)
                {
                    distances.Add(node, double.PositiveInfinity);
                    Previous.Add(node, null);
                }
            }
            distances[currentNode] = 0;
        }
        private void Update(Node neighbour)
        {
            var distanceCurrent = distances[currentNode];
            var edgeWeight = Model.IsDirected ? neighbour.Edges.First(x => x.StartNode == currentNode).Weight : neighbour.Edges.First(x => x.StartNode == currentNode || x.EndNode == currentNode).Weight;
            var alt = distanceCurrent + edgeWeight;

            if (alt < distances[neighbour])
            {
                distances[neighbour] = alt;
                Previous[neighbour] = currentNode;
            }
        }
        /// <summary>
        /// Führt den Algorithmus aus
        /// </summary>
        /// <param name="auto">Wenn wahr, wird der Algorithmus vollständig ausgeführt. Sonst nur eine Iteration</param>
        /// <returns>Anzahl der übrigen Iterationen</returns>
        public int Iterate(bool auto = false)
        {
            while (Q.Count > 0)
            {
                StepCount++;
                var minval = distances.Where(d => Q.Contains(d.Key)).Min(d => d.Value);
                currentNode = Q.First(x => x == distances.FirstOrDefault(d => d.Value == minval && d.Key == x).Key);
                Q.Remove(currentNode);
                var neighbours = currentNode.Neighbours(Model.IsDirected);
                foreach (var neighbour in neighbours)
                {
                    if (Q.Contains(neighbour))
                    {
                        Update(neighbour);
                    }
                }
                Results.Add(new AlgorithmResultTuple(currentNode, new Dictionary<Node, Node>(Previous), new Dictionary<Node, double>(distances)));
                if (!auto)
                    break;
            }
            return Q.Count;
        }
        /// <summary>
        /// Bereichnet die günstigste Route vom Startknoten zum Endknoten
        /// </summary>
        /// <param name="startId">ID des Startknotens</param>
        /// <param name="endId">ID des Endknotens</param>
        /// <returns>Route als Liste mit Knoten und Kosten Paaren</returns>
        public List<Tuple<Node, double>> GetShortestRoute(string startId = "-1", string endId = "-1")
        {
            if (startId == "-1" || endId == "-1") return null;
            StartNode = Model.Nodes.FirstOrDefault(x => x.Id == startId);
            Init(StartNode.Id);
            Iterate(true);

            List<Tuple<Node, double>> route = new List<Tuple<Node, double>>();
            EndNode = Model.Nodes.FirstOrDefault(x => x.Id == endId);
            if (EndNode != null)
            {
                Node currentNode = EndNode;
                Node previousNode = null;
                double edgeweight;
                while (currentNode != null)
                {
                    Previous.TryGetValue(currentNode, out previousNode);

                    if (previousNode != null)
                    {
                        if (Model.IsDirected)
                        {
                            edgeweight = currentNode.Edges.FirstOrDefault(x => x.EndNode == currentNode && x.StartNode == previousNode).Weight;
                        }
                        else
                        {
                            var edges = currentNode.Edges.Where(x => x.EndNode == currentNode && x.StartNode == previousNode || x.StartNode == currentNode && x.EndNode == previousNode);
                            edgeweight = edges.First(x => x.Weight == edges.Min(y => y.Weight)).Weight;
                        }

                    }
                    else
                    {
                        edgeweight = 0.0;
                    }
                    route.Insert(0, new Tuple<Node, double>(currentNode, edgeweight));
                    currentNode = previousNode;

                }
            }
            return route;
        }
    }
}
