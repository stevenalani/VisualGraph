using System;
using System.Globalization;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class Node
    {
        public double PosX { get; internal set; }
        public double PosY { get; internal set; }
        public string PosXText => PosX.ToString(CultureInfo.InvariantCulture);
        public string PosYText => PosY.ToString(CultureInfo.InvariantCulture);

        public string Name { get; internal set; }
        public int Id { get; internal set; }

        public string activeclass { get; set; } = "";
        public static Node operator +(Node a,Node b) => new Node { 
            Id = -1,
            Name = "valueonly",
            PosX = a.PosX + b.PosX,
            PosY = a.PosY+ b.PosY,
        };
        public static Node operator -(Node a, Node b){
            if(a == null){
                a = new Node{PosX = 0, PosY = 0};
            }
            if (b == null)
            {
                b = new Node { PosX = 0, PosY = 0 };
            }
            return new Node
            {
                Id = -1,
                Name = "valueonly",
                PosX = a.PosX - b.PosX,
                PosY = a.PosY - b.PosY,
            };
        }
        
        public double Distance => Math.Sqrt(PosX * PosX + PosY * PosY);
    }
}