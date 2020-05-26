using Microsoft.AspNetCore.Components.Web;

namespace VisualGraph.Client.Shared.EventHandling
{
    public class GraphTouchEventArgs<T>
    {

        public T Target { get; set; }
        public TouchEventArgs MouseEventArgs { get; set; }
        public GraphTouchEventArgs(T target, TouchEventArgs e)
        {
            Target = target;
            MouseEventArgs = e;

        }
    }
}