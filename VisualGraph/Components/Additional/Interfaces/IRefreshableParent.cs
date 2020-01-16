using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualGraph.Data.Additional.Models;

namespace VisualGraph.Components.Additional.Interfaces
{
    public interface IRefreshableParent
    {
        void Refresh();
        void Refresh(BasicGraphModel graph);
    }
}
