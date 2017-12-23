namespace BaristaLabs.BaristaCore.Tests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a converter that uses Json.Net
    /// </summary>
    public class JsonNetConverter : IJsonConverter
    {
        public object Parse(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public string Stringify(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
