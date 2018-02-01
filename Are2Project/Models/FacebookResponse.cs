using Newtonsoft.Json;

namespace Are2Project.Models
{
    [JsonObject]
    public class FacebookResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }
}