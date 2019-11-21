using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Frontenac.Blueprints;
using Frontenac.Blueprints.Impls.TG;
using Frontenac.Blueprints.Util.IO.GraphML;
using Frontenac.Gremlinq;
using Microsoft.AspNetCore.Mvc;

namespace VisualGraph.Data
{
    public class KeyDefinition
    {
        private readonly Type _targetType;
        private readonly string _keyName;
        private readonly Type _dataType;
        private readonly string _defaultValue;
        public KeyDefinition(Type typeOfTarget, string name, Type dataType,string defaultvalue = null)
        {
            _targetType = typeOfTarget;
            _keyName = name;
            _dataType = dataType;
            _defaultValue = defaultvalue;
        }

        public string GetTargetTypeName()
        {
            return _targetType.Name.ToLower();
        }

        public string GetAttributeName()
        {
            return _keyName.ToLower();
        }

        public string GetDataTypeName()
        {
            return _dataType.Name.ToLower();
        }

        public string GetDefaultKeyValue()
        {
            return _defaultValue.ToLower();
        }

    }
    public class GraphFileProvider
    {
        private static string _graphdir = Path.GetFullPath("./Graphs");

        /*private Dictionary<string, KeyDefinition> keys = new Dictionary<string, KeyDefinition>()
        {
            // Node Attributes
            {"d0", new KeyDefinition(typeof(Node), "color", typeof(string), "grey")},
            {"d1", new KeyDefinition(typeof(Node), "title", typeof(string))},
            {"d2", new KeyDefinition(typeof(Node), "description", typeof(string))},
            {"d3", new KeyDefinition(typeof(Node), "posx", typeof(double))},
            {"d4", new KeyDefinition(typeof(Node), "posy", typeof(double))},
            // Edge Attributes
            {"d5", new KeyDefinition(typeof(Edge), "weight", typeof(double))},
            {"d6", new KeyDefinition(typeof(Edge<>), "weight", typeof(double))},
            {"d7", new KeyDefinition(typeof(Edge), "weight", typeof(double))},
            {"d8", new KeyDefinition(typeof(Edge), "weight", typeof(double))},
            
        };*/
        public static void EnsureGraphDirExists()
        {
            if (!Directory.Exists(_graphdir))
            {
                Directory.CreateDirectory(_graphdir);
            }
        }

        public static string[] GetGraphFileNames()
        {
            return Directory.GetFiles(_graphdir);
        }
        public static TinkerGrapĥ[] GetGraphs()
        {

            List<TinkerGrapĥ> graphs = new List<TinkerGrapĥ>();

            foreach (var file in Directory.GetFiles(_graphdir))
            {
                StreamReader streamReader = new StreamReader(file);
                TinkerGrapĥ g = new TinkerGrapĥ(); ;
                GraphMlReader.InputGraph(g, streamReader.BaseStream);
                graphs.Add(g);
            }
            return graphs.ToArray();
        }
        public static TinkerGrapĥ GetGraph(string filename)
        {
            if (filename == String.Empty) return null;
            
            var file = Directory.GetFiles(_graphdir).FirstOrDefault(x => x.Contains(filename));
            
            if (file == String.Empty) return null;

            StreamReader streamReader = new StreamReader(file);
            TinkerGrapĥ g = new TinkerGrapĥ();;
            GraphMlReader.InputGraph(g,streamReader.BaseStream);
            streamReader.Close();
            return g;
        }

        public static void WriteGraph(TinkerGrapĥ graph, string name)
        {
            GraphMlWriter writer = new GraphMlWriter(graph);
            writer.OutputGraph("./Graphs" + name);
        }
    }
}