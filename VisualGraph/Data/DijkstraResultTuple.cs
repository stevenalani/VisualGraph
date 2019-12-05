using System;
using System.Collections.Generic;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public class DijkstraResultTuple : Tuple<Node, Dictionary<Node, Node>, Dictionary<Node, double>>
    {
        public Node StartNode => Item1;
        public Dictionary<Node, Node> Previous => Item2;
        public Dictionary<Node, double> Distances => Item3;
        public DijkstraResultTuple(Node startNode, Dictionary<Node, Node> previous, Dictionary<Node, double> distances) : base(startNode, previous, distances){}
    }
}
