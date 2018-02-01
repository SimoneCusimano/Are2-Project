using Newtonsoft.Json;

namespace Are2Project.Models
{
    [JsonObject]
    public class Device
    {
        [JsonProperty(PropertyName = "os")]
        public string Os { get; set; }
    }
}