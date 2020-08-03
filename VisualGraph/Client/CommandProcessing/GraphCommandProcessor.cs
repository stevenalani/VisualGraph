using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    /// <summary>
    /// Der GraphCommandProcessor führt Befehle aus Skripten oder der Graphconsole auf den zugewiesenen Graphen aus.
    /// </summary>
    public class GraphCommandProcessor
    {
        /// <summary>
        /// Erstellt eine Instanz der Klasse
        /// </summary>
        /// <param name="model">Model auf das die Befehle ausgeführt werden soll</param>
        public GraphCommandProcessor(BasicGraphModel model)
        {
            this.model = model;
        }
        private BasicGraphModel model { get; set; }
        private GraphCommandInterpreter graphCommandInterpreter = new GraphCommandInterpreter();
        /// <summary>
        /// Interpretiert den Eingabeparameter und versucht diesen als Befehl auszuführen.
        /// </summary>
        /// <param name="input">String mit Befehl und Parameten</param>
        /// <returns>Nachticht über Erfolg oder Fehler</returns>
        public string Process(string input)
        {
            try
            {
                var command = graphCommandInterpreter.InterpretCommand(input);
                if (command == null) return $"No command was not found in {input}";
                graphCommandInterpreter.InterpretAndSetCommandParameters(command, input);
                command.Invoke(model);
                return "Done: " + input + System.Environment.NewLine;
            }
            catch
            {
                return $"there went something wrong! please check your input: {input}" + System.Environment.NewLine;
            }
        }
    }
}
