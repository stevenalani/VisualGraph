using Microsoft.AspNetCore.Components.Web;

namespace VisualGraph.Client.Shared.EventHandling
{
    public class GraphMouseEventArgs<T>
    {

        public T Target { get; set; }
        public MouseEventArgs MouseEventArgs { get; set; }
        public GraphMouseEventArgs(T target, MouseEventArgs e)
        {
            Target = target;
            MouseEventArgs = e;

        }
    }
}
