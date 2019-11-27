using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Components;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.EventHandling
{
    public class GraphMouseEventArgs<T> 
    {

        public T Target {get;set;}
        public MouseEventArgs MouseEventArgs { get; set; }
        public GraphMouseEventArgs(T target,MouseEventArgs e)
        {
            Target = target;
            MouseEventArgs = e;
            
        }
    }
}
