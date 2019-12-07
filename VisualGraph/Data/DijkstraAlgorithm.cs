using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class DijkstraAlgorithm
    {
        List<Node> Q; 
        Node currentNode;
        public Node StartNode { get; private set; }
        public Node EndNode { get; set; }
        Dictionary<Node, double> distances = new Dictionary<Node, double>();
        Dictionary<Node, Node> Previous = new Dictionary<Node, Node>();
        
        public int StepCount;
        public List<DijkstraResultTuple> Results = new List<DijkstraResultTuple>();
        public int RemainingSteps => Q.Count;
        
        public DijkstraAlgorithm( BasicGraphModel model,int startNodeId = -1)
        {
            StepCount = 0;
            Q = model.Nodes.OrderBy(x => x.Id).ToList();
            Init(startNodeId);
        }
        private void Init(int startNodeId = -1)
        {
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
            var edgeWeight = neighbour.Edges.First(x => x.StartNode == currentNode || x.EndNode == currentNode).Weight;
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
                foreach(var neighbour in currentNode.Neighbours)
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
        public List<Node> GetShortestRoute()
        {
            List<Node> route = new List<Node>();
            
            if(EndNode != null)
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
}
