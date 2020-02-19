using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using VisualGraph.Components;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.Interfaces
{
    public interface IGraphService
    {
        public BasicGraph CurrentGraph { get; }
        public GraphStyleParameters GraphStyleParameters { get; }
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel graphModel);
        public Task LoadGraphStyleParameters();
        public Task SaveGraphStyleParameters(GraphStyleParameters graphStyleParameters = null);
        public Task<BasicGraphModel[]> GetAllGraphs();
        public Task<BasicGraphModel> GetGraph(string filename);

        public Task<string[]> GetGraphFilenames();
        public Task SaveGraph(BasicGraphModel graph, string filename = null);
        public Task InitZoomPan(DotNetObjectReference<BasicGraph> reference, string graphid);
        public Task DestroyZoomPan();
        public Task DisablePan();
        public Task EnablePan();
        public Task UpdateBBox();
        public Task Fit();
        public Task Center();
        public Task Crop();
        public Task Resize();
        public Task Rerender();
        public Task<SvgPanZoomInformation> GetSvgPanZoomInformation(string graphname);
        public Task<SvgContainerInformation> GetSvgContainerInformation(string graphname);
        public Task<Point2> GetTranslatedMousePos(string graphname, double x, double y);
        public Task<BasicGraphModel> LayoutGraph(BasicGraphModel graphModel);
        public Task<RenderFragment> GraphStyeTag ();
    }
}