using Newtonsoft.Json;

namespace Are2Project.Models
{
    [JsonObject]
    public class Paging
    {
        [JsonProperty(PropertyName = "previous")]
        public string Pre { get; set; }

        [JsonProperty(PropertyName = "next")]
        public string Next { get; set; }
    }
}