using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using VisualGraph.Shared.Models.Interfaces;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Diese Klasse stellt die Knoten im BasicGraphModel dar
    /// </summary>
    public class Node : ICSSProperties
    {
        /// <summary>
        /// Erstellt neue Instanz der Klasse
        /// </summary>
        public Node()
        {
            Classes = new List<string>();
        }
        /// <summary>
        /// Position der Kante
        /// </summary>
        public Vector2 Pos = new Vector2();
        /// <summary>
        /// Mit diesem Knoten verbundene Kanten
        /// </summary>
        public List<Edge> Edges = new List<Edge>();
        /// <summary>
        /// Gibt an ob die Kante aktiv ist
        /// </summary>
        public bool IsActive;
        /// <summary>
        /// Knoten, die mit diesem benachbart sind
        /// </summary>
        /// <param name="isDirectional">gibt an, ob es ein gerichteter Graph ist. Wenn war werden nur ausgehende Kanten geprüft. Sonst beide richtungen</param>
        /// <returns>Liste alle benachberten Knoten</returns>
        public List<Node> Neighbours(bool isDirectional) => isDirectional ? Edges.Select(x => x.EndNode).Where(x => x != this).ToList() : Edges.SelectMany(x => new[] { x.StartNode, x.EndNode }).Where(x => x != this).ToList();
        /// <summary>
        /// X- Koordinate der Position als InvariantCulture- String
        /// </summary>
        public string PosXText => Pos.X.ToString(CultureInfo.InvariantCulture);
        /// <summary>
        /// Y- Koordinate der Position als InvariantCulture- String
        /// </summary>
        public string PosYText => Pos.Y.ToString(CultureInfo.InvariantCulture);
        /// <summary>
        /// Name des Knoten
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ID des Knotens
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// hinzugefügte CSS- Klassen
        /// </summary>
        public List<string> Classes { get; set; }
        /// <summary>
        /// CSS- Klasse für aktive Knoten
        /// </summary>
        public string Activeclass => IsActive ? "active" : "";
        /// <summary>
        /// Addiert die Positionen der beiden Nodes und gibt neuen Knoten zurück
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Knoten mit Summe der Positionen der Eingabe- Knoten</returns>
        public static Node operator +(Node a, Node b) => new Node
        {
            Id = "-1",
            Name = "valueonly",
            Pos = a.Pos + b.Pos,
        };
        /// <summary>
        /// Subtrahiert die Positionen der beiden Nodes und gibt neuen Knoten zurück
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Knoten mit Differenz der Positionen der Eingabe- Knoten</returns>

        public static Node operator -(Node a, Node b)
        {
            if (a == null)
            {
                a = new Node { Pos = new Vector2() };
            }
            if (b == null)
            {
                b = new Node { Pos = new Vector2() };
            }
            return new Node
            {
                Id = "-1",
                Name = "valueonly",
                Pos = a.Pos - b.Pos,
            };
        }
        /// <summary>
        /// Länge des Positions- Vektors
        /// </summary>
        public double Distance => Math.Sqrt((Pos.X * Pos.X) + (Pos.Y * Pos.Y));

    }
}