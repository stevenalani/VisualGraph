using System;
using System.Numerics;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.Interfaces
{
    public interface IGraphService
    {
        public Task<BasicGraphModel[]> GetAllGraphs();
        public Task<BasicGraphModel> GetGraph(string filename);

        public Task<string[]> GetGraphFilenames();
        public Task SaveGraph(BasicGraphModel graph, string filename = null);
        public event Action RefreshRequested;
        public Task InitZoomPan(string graphid);
        public Task DestroyZoomPan();
        public Task DisablePan();
        public Task EnablePan();
        public Task Fit();
        public Task Center();
        public Task<GraphDisplayParameters> GetGraphDisplayParameters(string graphid);
        public Task<GraphDisplayParameters> InitialGetGraphDisplayParameters(string graphid);
        public Task<SvgInformation> GetSvgInformation(string graphname); 
        public Task<Point2> GetTranslatedMousePos(string graphname, double x, double y);
        public Task<BasicGraphModel> LayoutGraph(BasicGraphModel graphModel);

    }
}