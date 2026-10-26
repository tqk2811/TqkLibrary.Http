using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.Other.ImagesHostApi.ImagesHackCom.DataClass
{
    public class ImagesHackUploadImage
    {
        public string id { get; set; }
        public int? server { get; set; }
        public int? bucket { get; set; }
        public string filename { get; set; }
        public string direct_link { get; set; }
        public string original_filename { get; set; }
        public string title { get; set; }
        public ImageHackAlbum album { get; set; }
        public long? creation_date { get; set; }

        [JsonProperty("public")]
        public bool? IsPublic { get; set; }

        public bool? hidden { get; set; }
        public long? filesize { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public int? likes { get; set; }
        public bool? liked { get; set; }
        public bool? is_owner { get; set; }
        public ImageHackOwner owner { get; set; }
        public bool? adult_content { get; set; }
    }
}