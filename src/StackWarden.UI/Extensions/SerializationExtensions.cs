using Newtonsoft.Json;

namespace StackWarden.UI.Extensions
{
    public static class SerializationExtensions
    {
        public static string ToJson(this object source)
        {
            var serializedObject = JsonConvert.SerializeObject(source);

            return serializedObject;
        }

        public static T FromJson<T>(this string source)
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(source);

            return deserializedObject;
        }
    }
}