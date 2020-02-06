using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
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

        public DijkstraAlgorithm( BasicGraphModel model,int startNodeId = -1)
        {
            this.Model = model;
            Init(startNodeId);
        }
        private void Init(int startNodeId = -1)
        {
            Results = new List<DijkstraResultTuple>();
            distances = new Dictionary<Node, double>();
            Previous = new Dictionary<Node, Node>();
            Results = new List<DijkstraResultTuple>();
            StepCount = 0;
            Q = Model.Nodes.OrderBy(x => x.Id).ToList();
            StartNode = currentNode = (startNodeId != -1) ? Q.First(x => x.Id == startNodeId) : Q[0];
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
            var edgeWeight = Model.IsDirectional? neighbour.Edges.First(x => x.StartNode == currentNode).Weight : neighbour.Edges.First(x => x.StartNode == currentNode || x.EndNode == currentNode).Weight;
            var alt = distanceCurrent + edgeWeight;
            
            if(alt < distances[neighbour])
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
                var minval = distances.Where( d=> Q.Contains(d.Key)).Min(d => d.Value);
                currentNode = Q.First(x => x == distances.FirstOrDefault(d => d.Value == minval && d.Key == x).Key);
                Q.Remove(currentNode);
                var neighbours = currentNode.Neighbours(Model.IsDirectional);
                foreach (var neighbour in neighbours)
                {
                    if (Q.Contains(neighbour))
                    {
                        Update(neighbour);
                    }
                }
                Results.Add(new DijkstraResultTuple(StartNode, new Dictionary<Node,Node>(Previous), new Dictionary<Node,double>(distances)));
                if (!auto)
                    break;
            }
            return Q.Count;
        }
        public List<Node> GetShortestRoute(int startId = -1, int endId = -1)
        {
            if (startId == -1 || endId == -1) return null;
                StartNode = Model.Nodes.FirstOrDefault(x => x.Id == startId);
                Init(StartNode.Id);
                Iterate(true);

            List<Node> route = new List<Node>();
            EndNode = Model.Nodes.FirstOrDefault(x => x.Id == endId);
            if (EndNode != null)
            {
                Node currentNode = EndNode;
                while(currentNode != null)
                {
                    route.Insert(0, currentNode);
                    if (currentNode == StartNode)
                        break;
                    Previous.TryGetValue(currentNode,out currentNode);
                }
            }
            return route;
        }        
    }

    public interface IGraphAlgorithm
    {
        public List<Node> GetShortestRoute(int startnodeId, int endnodeId);
        public int Iterate(bool auto = false);
        public string Name { get; }
    }
}
