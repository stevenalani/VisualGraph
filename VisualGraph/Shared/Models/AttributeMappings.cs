using System.Collections.Generic;

namespace VisualGraph.Shared.Models
{
    public class AttributeMappings : Dictionary<string, Dictionary<string, string>>
    {
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
