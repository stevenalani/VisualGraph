using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Shared
{
    public static class VGAppSettings
    {
        public static List<string> Roles { get; set; } = new List<string> {
            "Administrator",
            "Member"
        };
    }
}
