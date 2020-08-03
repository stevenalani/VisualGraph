using System;
using System.Collections.Generic;
using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Basisklasse für Befehle des CommandProcessors
    /// </summary>
    internal abstract class GraphCommand
    {
        internal abstract void Invoke(BasicGraphModel g);
        internal Dictionary<Type, object[]> Parameters;
    }
}