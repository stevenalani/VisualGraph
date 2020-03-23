using Microsoft.AspNetCore.Components.Web;

namespace VisualGraph.Data.Additional.EventHandling
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