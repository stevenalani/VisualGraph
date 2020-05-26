using System;
using System.Collections.Generic;

namespace VisualGraph.Shared.Models.Interfaces
{
    public interface ICSSProperties
    {
        public string ClassesProppertie => string.Join(' ', Classes);
        public List<string> Classes { get; set; }
        public string Id { get; set; }
    }
}