using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisualGraph.Shared.Serialization
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options){
            var readstring = reader.GetString().Split(":");
            return new Vector2(float.Parse(readstring[0]),float.Parse(readstring[1])); 
         }

        public override void Write(
            Utf8JsonWriter writer,
            Vector2 vec2value,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(vec2value.X.ToString(CultureInfo.InvariantCulture) + ":" + vec2value.Y.ToString(CultureInfo.InvariantCulture));
    }
}
