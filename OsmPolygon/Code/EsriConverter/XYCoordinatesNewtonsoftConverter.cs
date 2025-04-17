
namespace OsmPolygon.EsriConverter
{


    public class XYCoordinatesNewtonsoftConverter 
        : Newtonsoft.Json.JsonConverter<System.Collections.Generic.List<XYCoordinates>>
    {
        public override System.Collections.Generic.List<XYCoordinates> ReadJson(
            Newtonsoft.Json.JsonReader reader,
            System.Type objectType,
            System.Collections.Generic.List<XYCoordinates> existingValue, 
            bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            System.Collections.Generic.List<XYCoordinates> result = new System.Collections.Generic.List<XYCoordinates>();

            Newtonsoft.Json.Linq.JArray outerArray = Newtonsoft.Json.Linq.JArray.Load(reader);

            foreach (Newtonsoft.Json.Linq.JToken inner in outerArray)
            {
                if (inner is not Newtonsoft.Json.Linq.JArray pair || pair.Count < 2)
                    throw new Newtonsoft.Json.JsonException("Expected array of at least 2 elements");

                decimal x = Newtonsoft.Json.Linq.Extensions.Value<decimal>(pair[0]);
                decimal y = Newtonsoft.Json.Linq.Extensions.Value<decimal>(pair[1]);

                result.Add(new XYCoordinates(x, y));
            }

            return result;
        }

        public override void WriteJson(
            Newtonsoft.Json.JsonWriter writer,
            System.Collections.Generic.List<XYCoordinates> value, 
            Newtonsoft.Json.JsonSerializer serializer
        )
        {
            writer.WriteStartArray();

            foreach (XYCoordinates coord in value)
            {
                writer.WriteStartArray();
                writer.WriteValue(coord.X);
                writer.WriteValue(coord.Y);
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }


    }


}
