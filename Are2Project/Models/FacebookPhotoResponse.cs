using Newtonsoft.Json;
using System.Collections.Generic;

namespace Are2Project.Models
{
    [JsonObject]
    public class FacebookPhotoResponse
    {
        [JsonProperty(PropertyName = "data")]
        public IList<PhotosData> Photos { get; set; }

        [JsonProperty(PropertyName = "paging")]
        public Paging PagingInfo { get; set; }
    }
}