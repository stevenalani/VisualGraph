using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.Additional.Interfaces
{
    public interface IGraphService
    {
        public Task<BasicGraph[]> GetAllGraphs();
        public Task<BasicGraph> GetGraph(string filename);

        public Task<string[]> GetGraphFilenames();
        public Task SaveGraph(BasicGraph graph, string filename = null);
    }
}