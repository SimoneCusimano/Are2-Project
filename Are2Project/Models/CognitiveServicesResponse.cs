using Newtonsoft.Json;
using System.Collections.Generic;

namespace Are2Project.Models
{
    [JsonObject]
    public class CognitiveServicesResponse
    {
        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("faces")]
        public List<Face> Faces { get; set; }

        [JsonProperty("adult")]
        public Adult Adult { get; set; }

        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("imageType")]
        public ImageType ImageType { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    [JsonObject]
    public class Metadata
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }

    [JsonObject]
    public class ImageType
    {
        [JsonProperty("clipArtType")]
        public long ClipArtType { get; set; }

        [JsonProperty("lineDrawingType")]
        public long LineDrawingType { get; set; }
    }

    [JsonObject]
    public class Face
    {
        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
    }

    [JsonObject]
    public class FaceRectangle
    {
        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }

    [JsonObject]
    public class Description
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("captions")]
        public List<Caption> Captions { get; set; }
    }

    [JsonObject]
    public class Caption
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    [JsonObject]
    public class Color
    {
        [JsonProperty("dominantColorForeground")]
        public string DominantColorForeground { get; set; }

        [JsonProperty("dominantColorBackground")]
        public string DominantColorBackground { get; set; }

        [JsonProperty("dominantColors")]
        public List<string> DominantColors { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; }

        [JsonProperty("isBwImg")]
        public bool IsBwImg { get; set; }
    }

    [JsonObject]
    public class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("detail")]
        public Detail Detail { get; set; }
    }

    [JsonObject]
    public class Detail
    {
        [JsonProperty("celebrities")]
        public List<object> Celebrities { get; set; }
    }

    [JsonObject]
    public class Adult
    {
        [JsonProperty("isAdultContent")]
        public bool IsAdultContent { get; set; }

        [JsonProperty("adultScore")]
        public double AdultScore { get; set; }

        [JsonProperty("isRacyContent")]
        public bool IsRacyContent { get; set; }

        [JsonProperty("racyScore")]
        public double RacyScore { get; set; }
    }

}
