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

        public string activeclass { get; set; } = ""
            ;
    }
}