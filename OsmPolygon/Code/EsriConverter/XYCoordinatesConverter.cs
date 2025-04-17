
namespace OsmPolygon.EsriConverter
{


    public class XYCoordinatesConverter 
        : System.Text.Json.Serialization.JsonConverter<System.Collections.Generic.List<XYCoordinates>>
    {
        public override System.Collections.Generic.List<XYCoordinates> Read(
            ref System.Text.Json.Utf8JsonReader reader, 
            System.Type typeToConvert, 
            System.Text.Json.JsonSerializerOptions options
        )
        {
            var result = new System.Collections.Generic.List<XYCoordinates>();

            if (reader.TokenType != System.Text.Json.JsonTokenType.StartArray)
                throw new System.Text.Json.JsonException("Expected StartArray");

            reader.Read(); // Move to the first nested array or EndArray

            while (reader.TokenType != System.Text.Json.JsonTokenType.EndArray)
            {
                if (reader.TokenType != System.Text.Json.JsonTokenType.StartArray)
                    throw new System.Text.Json.JsonException("Expected inner array");

                reader.Read(); // move to first number in pair
                decimal x = reader.GetDecimal();

                reader.Read(); // move to second number in pair
                decimal y = reader.GetDecimal();

                reader.Read(); // move past end of inner array
                if (reader.TokenType != System.Text.Json.JsonTokenType.EndArray)
                    throw new System.Text.Json.JsonException("Expected end of inner array");

                result.Add(new XYCoordinates(x, y));

                reader.Read(); // move to next inner array or EndArray
            }

            return result;
        }

        public override void Write(
            System.Text.Json.Utf8JsonWriter writer, 
            System.Collections.Generic.List<XYCoordinates> value,
            System.Text.Json.JsonSerializerOptions options
        )
        {
            writer.WriteStartArray();

            foreach (var coord in value)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(coord.X);
                writer.WriteNumberValue(coord.Y);
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}
