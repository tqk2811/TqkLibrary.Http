using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.Other.ImagesHostApi.ImagesHackCom.DataClass
{
    public class ImageHackAlbum
    {
        public string id { get; set; }
        public string title { get; set; }

        [JsonProperty("public")]
        public bool? IsPublic { get; set; }
    }
}