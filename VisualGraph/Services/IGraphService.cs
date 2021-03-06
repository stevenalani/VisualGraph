﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VisualGraph.Components;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Services
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
        public Task<bool> SaveGraph(string filename = null);
        public Task InitZoomPan(DotNetObjectReference<BasicGraph> reference);
        public Task DestroyZoomPan();
        public Task DisablePan();
        public Task EnablePan();
        public Task UpdateBBox();
        public Task Fit();
        public Task Center();
        public Task Crop();
        public Task Resize();
       
        public Task<SvgPanZoomInformation> GetSvgPanZoomInformation();
        public Task<SvgContainerInformation> GetSvgContainerInformation();
        public Task<Point2> GetTranslatedMousePos(double x, double y);
        public Task LayoutGraph(double scalex = 2.2, double scaley = 2.2);
        public Task<BrowserSizes> GetBrowserSizes();
        public Task UseGraphModel(BasicGraphModel graphModel, bool withDefaultCallbacks = true);
        public Task<RenderFragment> GetRenderFragment(BasicGraphModel graphModel, bool withDefaultCallbacks = true);
        public Task<RenderFragment> GetRenderFragment(bool withDefaultCallbacks = true);
        
        public Task<RenderFragment> GetCssMarkup();
        public Task<RenderFragment> GetSettingsRenderFragment();
        public Task<RenderFragment> GetEditFormRenderFragment();
        public Task Rerender();
    }
}