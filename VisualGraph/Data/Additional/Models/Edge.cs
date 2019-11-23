using Frontenac.Blueprints;

namespace VisualGraph.Data.Additional.Models
{
    public class Edge
    {
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public double Weight { get; set; }
        public double Id { get; internal set; }
    }
}