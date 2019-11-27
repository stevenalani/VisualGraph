using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class GraphDisplayParameters
    {
        public string Name { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public double E { get; set; }
        public double F { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public override string ToString()
        {
            return $"a={A}\r\nb={B}\r\nc={C}\r\nd={D}\r\ne={E}\r\nf={F}";
        }
        public double Parentheight { get; set; }
        public double Parentwidth { get; set; }
}
}
