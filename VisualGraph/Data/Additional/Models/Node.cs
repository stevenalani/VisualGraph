using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class Node
    {
        public Point2 Pos { get; set; } = new Point2();
        public List<Edge> Edges = new List<Edge>();
        public bool IsActive;

        internal List<Node> Neighbours => Edges.SelectMany( x => new[]{ x.StartNode, x.EndNode }).Where(x => x != this).ToList();

        public string PosXText => Pos.X.ToString(CultureInfo.InvariantCulture);
        public string PosYText => Pos.Y.ToString(CultureInfo.InvariantCulture);

        public string Name { get; internal set; }
        public int Id { get; internal set; }
        
        public string activeclass => IsActive? "active":"";
        public static Node operator +(Node a,Node b) => new Node { 
            Id = -1,
            Name = "valueonly",
            Pos = a.Pos + b.Pos,
        };
        public static Node operator -(Node a, Node b){
            if(a == null){
                a = new Node { Pos = new Point2() };
            }
            if (b == null)
            {
                b = new Node { Pos = new Point2() };
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