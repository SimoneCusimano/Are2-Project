using Newtonsoft.Json;
using System.Collections.Generic;

namespace Are2Project.Models
{
    [JsonObject]
    public class Profile
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "devices")]
        public IList<Device> Devices { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string Firstname { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string Lastname { get; set; }
    }
}