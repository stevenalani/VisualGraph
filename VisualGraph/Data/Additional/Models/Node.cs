using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class Node : ICSSProperties
    {
        public Node()
        {
            Classes = new List<string>(); 
        }
        public Vector2 Pos = new Vector2();
        public List<Edge> Edges = new List<Edge>();
        public bool IsActive;

        internal List<Node> Neighbours(bool isDirectional) => isDirectional? Edges.Select(x => x.EndNode).Where( x => x != this ).ToList(): Edges.SelectMany(x => new[] { x.StartNode, x.EndNode }).Where(x => x != this).ToList();

        public string PosXText => Pos.X.ToString(CultureInfo.InvariantCulture);
        public string PosYText => Pos.Y.ToString(CultureInfo.InvariantCulture);

        public string Name { get; internal set; }
        public int Id { get; set; }

        public List<string> Classes { get; set; }
        public string Activeclass => IsActive? "active":"";
        public static Node operator +(Node a,Node b) => new Node { 
            Id = -1,
            Name = "valueonly",
            Pos = a.Pos + b.Pos,
        };
        public static Node operator -(Node a, Node b){
            if(a == null){
                a = new Node { Pos = new Vector2() };
            }
            if (b == null)
            {
                b = new Node { Pos = new Vector2() };
            }
            return new Node
            {
                Id = -1,
                Name = "valueonly",
                Pos = a.Pos - b.Pos,
            };
        }
        
        public double Distance => Math.Sqrt((Pos.X * Pos.X ) + (Pos.Y * Pos.Y));

    }
}