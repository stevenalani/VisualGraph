using Frontenac.Blueprints;
using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.ConjugateGradient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VisualGraph.Data.Additional.Models
{
    public class Edge : ICSSProperties
    {
        public Edge()
        {
            Classes = new List<string>();
        }
        public bool IsActive;
        public string Activeclass => IsActive ? "active" : "";
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public double Weight { get; set; }
        public double AutoWeight => (EndNode - StartNode).Distance;
        public int Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public float curveScale = 0.3f;
        public float curveScaleLowerBound = 0f;
        public float curveScaleUpperBound = 10f;
        public Vector2 Direction => Vector2.Normalize(EndNode.Pos - StartNode.Pos);
        public Vector2 Edgemiddle => (StartNode.Pos + Direction * ((EndNode.Pos - StartNode.Pos).Length() / 2));

    }
}