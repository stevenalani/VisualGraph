using System;
using System.Collections.Generic;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    public abstract class GraphCommand
    {
        public abstract void Invoke(BasicGraphModel g);
        internal Dictionary<Type, object[]> Parameters;
    }
}