using System;
using System.Collections.Generic;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Data.CommandProcessing
{
    public abstract class GraphCommand
    {
        public abstract void Invoke(BasicGraphModel g);
        internal Dictionary<Type, object[]> Parameters;
    }
}