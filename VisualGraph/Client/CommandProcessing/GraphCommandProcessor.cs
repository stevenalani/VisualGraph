using VisualGraph.Shared.Models;

namespace VisualGraph.Client.CommandProcessing
{
    public class GraphCommandProcessor
    {
        public GraphCommandProcessor(BasicGraphModel model)
        {
            this.model = model;
        }
        private BasicGraphModel model { get; set; }
        private GraphCommandInterpreter graphCommandInterpreter = new GraphCommandInterpreter();
        public string Process(string input)
        {
            try
            {
                var command = graphCommandInterpreter.InterpretCommand(input);
                if (command == null) return $"No command was not found in {input}";
                graphCommandInterpreter.InterpretAndSetCommandParameters(command, input);
                command.Invoke(model);
                return "Done: " + input;
            }
            catch
            {
                return $"there went something wrong! please check your input: {input}";
            }
        }
    }
}
