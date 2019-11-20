using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontenac.Blueprints.Impls.TG;

namespace VisualGraph.Data
{
    public class GraphService
    {
        public Task<TinkerGrapĥ[]> GetAllGraphs()
        {
            return Task.FromResult(GraphFileProvider.GetGraphs());
        }
        public Task<TinkerGrapĥ> GetGraph(string filename)
        {
            return Task.FromResult(GraphFileProvider.GetGraph(filename));
        }

        public Task<string[]> GetGraphFilenames()
        {
            return Task.FromResult(GraphFileProvider.GetGraphFileNames());
        }

        public Task WriteGraph(TinkerGrapĥ graph, string filename)
        {
            GraphFileProvider.WriteGraph(graph,filename);
            var wrotegraph = GraphFileProvider.GetGraph(filename);
            if (wrotegraph != null)
            {
                return Task.FromException(new Exception("There was an error"));
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
