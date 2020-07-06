
using System.Linq;

namespace DelaunatorSharp
{

    // [Newtonsoft.Json.JsonConverter(typeof(ConcreteTypeConverter<IPoint, Point>))]
    public interface IPoint
    {
        double X { get; set; }
        double Y { get; set; }
    }


    public class ConcreteTypeConverter<TInterface, TConcrete> 
        : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            //assume we can convert to anything for now
            return (objectType == typeof(TInterface));
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return (TInterface)(object)serializer.Deserialize<TConcrete>(reader);
        }
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }


    public class ConcreteArrayTypeConverter<TInterface, TImplementation>
     : Newtonsoft.Json.JsonConverter where TImplementation : TInterface
    {
        public override bool CanConvert(System.Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var res = serializer.Deserialize<TImplementation[]>(reader);
            var val = res.Select(x => (TInterface)x).ToArray();

            return val;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }


    public class ConcreteListTypeConverter<TInterface, TImplementation> 
        : Newtonsoft.Json.JsonConverter where TImplementation : TInterface
    {
        public override bool CanConvert(System.Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var res = serializer.Deserialize<System.Collections.Generic.List<TImplementation>>(reader);
            var val = res.Select(x => (TInterface)x).ToList();

            return val;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }


}
