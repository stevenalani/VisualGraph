namespace VisualGraph.Shared.Models
{
    /// <summary>
    /// POCO Klasse für die JSON- Serialisierung von Nodes/Graphen
    /// </summary>
    public class NodePoco : Node
    {
        
        public NodePoco() { }
        public NodePoco(Node node)
        {
            this.Classes = node.Classes;
            this.Id = node.Id;
            this.Name = node.Name;
            this.PosXTextPoco = node.Pos.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
            this.PosYTextPoco = node.Pos.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string PosXTextPoco { get; set; }
        public string PosYTextPoco { get; set; }

    }
}