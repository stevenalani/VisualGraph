using Frontenac.Blueprints;
using System;

namespace VisualGraph.Data.Additional.Models
{
    public class Edge
    {
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public double Weight { get; set; }
        public double AutoWeight => (EndNode - StartNode).Distance;
        public double Id { get; internal set; }

        
    }
}