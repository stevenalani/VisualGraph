using System.Collections.Generic;
using System.Numerics;
using VisualGraph.Shared.Models.Interfaces;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Kapselt Daten von Kanten 
    /// </summary>
    public class Edge : ICSSProperties
    {
        /// <summary>
        /// Erstellt neue Kante
        /// </summary>
        public Edge()
        {
            Classes = new List<string>();
        }
        /// <summary>
        /// Ist die Kante aktuell als aktiv ausgewählt
        /// </summary>
        public bool IsActive;
        /// <summary>
        /// Css- Klasse für aktive Kanten
        /// </summary>
        public string Activeclass => IsActive ? "active" : "";
        /// <summary>
        /// Start- Knoten der Kante
        /// </summary>
        public Node StartNode { get; set; }
        /// <summary>
        /// End- Knoten der Kante
        /// </summary>
        public Node EndNode { get; set; }
        /// <summary>
        /// Gewicht der Kante
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Automatisches Gewicht (Abstand zw. Start- und Endknoten)
        /// </summary>
        public double AutoWeight => (EndNode - StartNode).Distance;
        /// <summary>
        /// Kanten ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// CSS- Klassen der Kante
        /// </summary>
        public List<string> Classes { get; set; } = new List<string>();
        /// <summary>
        /// Kurvenweite
        /// </summary>
        public float curveScale = 0.0f;
        /// <summary>
        /// Untergrenze für Kurvenweite
        /// </summary>
        public float curveScaleLowerBound = -10f;
        /// <summary>
        /// Obergrenze für Kurvenweite
        /// </summary>
        public float curveScaleUpperBound = 10f;
        /// <summary>
        /// Richtung der Kante
        /// </summary>
        public Vector2 Direction => Vector2.Normalize(EndNode.Pos - StartNode.Pos);
        /// <summary>
        /// Mittelpunkt der Kante
        /// </summary>
        public Vector2 Edgemiddle => (StartNode.Pos + Direction * ((EndNode.Pos - StartNode.Pos).Length() / 2));

    }
}