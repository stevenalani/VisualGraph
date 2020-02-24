using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using VisualGraph.Components;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Services.Interfaces
{
    public interface IGraphService
    {
        public BasicGraphModel CurrentGraphModel { get; set; }
        public BasicGraph CurrentGraph { get; set; }
        public GraphStyleParameters GraphStyleParameters { get; set; }
        public Settings Settings { get; set; }
        public GraphEditForm GraphEditForm { get; set; }
        public SettingsCSS SettingsCSS { get; set; }
        public Task LoadGraph(string filename);

        public Task LoadGraphStyleParameters();
        public Task SaveGraphStyleParameters(GraphStyleParameters graphStyleParameters = null);
        public Task<BasicGraphModel[]> GetAllGraphs();
        public Task<BasicGraphModel> GetGraph(string filename);

        public Task<string[]> GetGraphFilenames();
        public Task SaveGraph(BasicGraphModel graph, string filename = null);
        public Task InitZoomPan(DotNetObjectReference<BasicGraph> reference);
        public Task DestroyZoomPan();
        public Task DisablePan();
        public Task EnablePan();
        public Task UpdateBBox();
        public Task Fit();
        public Task Center();
        public Task Crop();
        public Task Resize();
       
        public Task<SvgPanZoomInformation> GetSvgPanZoomInformation(string graphname);
        public Task<SvgContainerInformation> GetSvgContainerInformation(string graphname);
        public Task<Point2> GetTranslatedMousePos(string graphname, double x, double y);
        public Task LayoutGraph();



        public Task<RenderFragment> GetRenderFragment(BasicGraphModel graphModel);
        public Task<RenderFragment> GetRenderFragment();
        public Task<RenderFragment> GraphStyeTag();
        public Task<RenderFragment> GetSettingsRenderFragment();
        public Task<RenderFragment> GetEditFormRenderFragment();
        public Task Rerender();
    }
}