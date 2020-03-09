using System;
using System.Collections.Generic;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public interface IGraphAlgorithm
    {
        public List<Tuple<Node, double>> GetShortestRoute(string startnodeId, string endnodeId);
        public int Iterate(bool auto = false);
        public string Name { get; }
    }
}
