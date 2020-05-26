using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Client.Shared;
using VisualGraph.Client.Shared.Models;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.Shared.Algorithm
{
    public class DijkstraAlgorithm : IGraphAlgorithm
    {
        BasicGraphModel Model;
        List<Node> Q;
        Node currentNode;
        public Node StartNode { get; private set; }
        public Node EndNode { get; set; }
        Dictionary<Node, double> distances;
        Dictionary<Node, Node> Previous;

        public int StepCount;
        public List<DijkstraResultTuple> Results;
        public int RemainingSteps => Q.Count;

        public string Name => "Dijkstra Algorithm";

        public DijkstraAlgorithm(BasicGraphModel model, string startNodeId = "-1")
        {
            Model = model;
            Init(startNodeId);
        }
        private void Init(string startNodeId = "-1")
        {
            Results = new List<DijkstraResultTuple>();
            distances = new Dictionary<Node, double>();
            Previous = new Dictionary<Node, Node>();
            Results = new List<DijkstraResultTuple>();
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
                Results.Add(new DijkstraResultTuple(currentNode, new Dictionary<Node, Node>(Previous), new Dictionary<Node, double>(distances)));
                if (!auto)
                    break;
            }
            return Q.Count;
        }
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
