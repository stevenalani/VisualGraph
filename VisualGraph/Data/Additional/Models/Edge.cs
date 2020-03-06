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
        public Vector2 Direction => Vector2.Normalize(EndNode.Pos - StartNode.Pos);
        public Vector2 CurveAnchorPosition { get;set; }
        public Vector2 TextAnchorPosition { get; set; }
        private void calcInitialAnchors()
        {
            var halfway = StartNode.Pos + Direction * ((EndNode.Pos - StartNode.Pos).Length() / 2);
            var rotatedDir = Vector2.Transform(Direction, Matrix3x2.CreateRotation(MathF.PI / 2));
            var lengthofVector = rotatedDir * (halfway - StartNode.Pos).Length();
            CurveAnchorPosition = lengthofVector * curveScale + halfway;
            TextAnchorPosition = lengthofVector * 0.1f + halfway;
        }
    }
}