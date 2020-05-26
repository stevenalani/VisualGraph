using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Client.Services;

namespace VisualGraph.Client.Components
{
    public class AlgorithmPage : ComponentBase
    {

        
        [Parameter]
        public string Filename { get; set; }
        [Inject]
        protected IGraphService GraphService { get; set; }
        public static string StartNodeClass = "startnode";
        public static string EndNodeClass = "endnode";
        public static string RouteClass = "vgpath";
        public static string PreviewClass = "vgpathpreview";
        public static string NodeNotReachableClass = "vgnodeunreachable";
        public static string IterationCurrentNodeClass = "iterationnode";
        public static string IterationClass = "vgiteration";


        protected override void OnInitialized()
        {
            if (Filename != null) {
                GraphService.LoadGraph(Filename);
            }
        }

    }
}
