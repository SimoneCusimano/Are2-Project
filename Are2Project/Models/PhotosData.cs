using Newtonsoft.Json;
using System.Collections.Generic;

namespace Are2Project.Models
{
    [JsonObject]
    public class PhotosData
    {
        [JsonProperty(PropertyName = "images")]
        public IEnumerable<Photo> Photos { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}