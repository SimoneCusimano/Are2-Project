using Newtonsoft.Json;

namespace Are2Project.Models
{
    [JsonObject]
    public class Photo
    {
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
    }
}