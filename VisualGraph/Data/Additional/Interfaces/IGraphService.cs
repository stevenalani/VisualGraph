using System;
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
        public void CallRequestRefresh();
        public void StartParameterRefresh();
        public void StopParameterRefresh();

        public Task<GraphDisplayParameters> GetGraphDisplayParameters(string graphid);
        public Task<GraphDisplayParameters> InitialGetGraphDisplayParameters(string graphid);
        public Task<BasicGraphModel> LayoutGraph(BasicGraphModel graphModel);
    }
}