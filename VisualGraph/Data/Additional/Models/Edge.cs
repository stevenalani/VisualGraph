using Frontenac.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualGraph.Data.Additional.Models
{
    public class Edge : ICSSProperties
    {
        public Edge()
        {
            Classes = new List<string>();
        }
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public double Weight { get; set; }
        public double AutoWeight => (EndNode - StartNode).Distance;
        public int Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
    }

    public interface ICSSProperties
    {
        public string ClassesProppertie => String.Join(' ', Classes );
        public List<string> Classes { get; set; }
        public int Id { get; set; }
    }
}