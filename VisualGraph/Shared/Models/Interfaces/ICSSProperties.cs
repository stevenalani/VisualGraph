using System.Collections.Generic;

namespace VisualGraph.Shared.Models.Interfaces
{
    /// <summary>
    /// Interface für Elemente mit Css- Klassen und einer Id
    /// </summary>
    public interface ICSSProperties
    {
        public string ClassesProppertie => string.Join(' ', Classes);
        public List<string> Classes { get; set; }
        public string Id { get; set; }
    }
}