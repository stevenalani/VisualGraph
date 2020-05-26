﻿using VisualGraph.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VisualGraph.Shared.Models;
using VisualGraph.Server.Providers;

namespace VisualGraph.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        
        private readonly ILogger<GraphController> logger;

        public GraphController(ILogger<GraphController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("GetGraphModel/{graphname}")]
        public async Task<BasicGraphModelPoco> GetGraphModel(string graphname)
        {
            if(graphname == string.Empty || graphname == null)
            {
                return null;
            }
            BasicGraphModel model = await GraphFileProvider.GetBasicGraph(graphname);
            BasicGraphModelPoco modelPoco = new BasicGraphModelPoco( model);
            return modelPoco;
        }

        [HttpGet]
        public async Task<IEnumerable<BasicGraphModelPoco>> GetGraphModels()
        {
            var models = (await GraphFileProvider.GetBasicGraphs()).Select( x => new BasicGraphModelPoco(x));
             
            return models;
        }

        [HttpGet("GetGraphFilenames")]
        public async Task<IEnumerable<string>> GetGraphFilenames()
        {
            var graphnames = await GraphFileProvider.GetGraphFileNames();
            return graphnames;
        }
        [HttpPost("SaveGraph/{filename}")]
        public async Task<bool> SaveGraph([FromRoute]string filename,[FromBody] BasicGraphModelPoco graph)
        {
            return await GraphFileProvider.WriteToGraphMlFile(new BasicGraphModel(graph), filename);
        }
    }
}
