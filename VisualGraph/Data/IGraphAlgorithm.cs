using System.Collections.Generic;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data
{
    public interface IGraphAlgorithm
    {
        public List<Node> GetShortestRoute(int startnodeId, int endnodeId);
        public int Iterate(bool auto = false);
        public string Name { get; }
    }
}
