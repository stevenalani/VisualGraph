using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class Node
    {
        public double PosX { get; internal set; }
        public double PosY { get; internal set; }
        public string Name { get; internal set; }
        public int Id { get; internal set; }

        public Task MoveToPosition(double x,double y)
        {
            PosX = x;
            PosY = y;
            return Task.CompletedTask;
        }
    }
}