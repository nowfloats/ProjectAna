using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Reflection;
namespace ANAConversationPlatform.Helpers
{
    public class CustomStringEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Enum)) return;

            var fieldInfo = value.GetType().GetFields().FirstOrDefault(x => x.Name == value.ToString());
            if (fieldInfo != null)
                writer.WriteValue((value as Enum).GetDescriptionOrDefault());
        }
    }
}
