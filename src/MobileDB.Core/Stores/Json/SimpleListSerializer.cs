using System;
using System.Collections;
using Newtonsoft.Json;

namespace MobileDB.Stores.Json
{
    internal class SimpleListSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as IEnumerable;

            if (list == null)
                throw new ArgumentException("Value must be a IEnumerable", "value");

            foreach (var item in list)
            {
                serializer.Serialize(writer, item);
                writer.WriteRaw(Environment.NewLine);
            }
        }
    }
}