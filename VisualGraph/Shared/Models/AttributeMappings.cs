using System.Collections.Generic;

namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// Mappings zwischen aus Web geladenem Graphen und BasicGraphModels
    /// Diese Klasse stellt ein Dictionary dar, welches für die Attribute von Knoten und Kanten des BasicGraphModels
    /// je einen String mit dem entsprechenden Attributnamen zuweisen lässt
    /// </summary>
    public class AttributeMappings : Dictionary<string, Dictionary<string, string>>
    {
        /// <summary>
        /// Erstellt Instanz der Klasse mit Wörterbüchern für Attribute des BasicGraphModels
        /// </summary>
        public AttributeMappings() : base()
        {
            Add("Node", new Dictionary<string, string>() {
                {"Id","" },
                {"Name","" },
                {"Posx","" },
                {"Posy","" }
            });
            Add("Edge", new Dictionary<string, string>() {
                {"Id","" },
                {"Weight","" },
                {"IsDirected","" },
            });
        }
    }
}
