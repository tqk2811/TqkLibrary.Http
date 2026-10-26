using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Content
    {
        [JsonProperty(".tag")]
        public string Tag { get; set; }

        [JsonProperty("thumbnail_url_tmpl")]
        public string ThumbnailUrlTmpl { get; set; }

        [JsonProperty("default_src")]
        public string DefaultSrc { get; set; }

        [JsonProperty("full_size_src")]
        public string FullSizeSrc { get; set; }

        [JsonProperty("src_set")]
        public string SrcSet { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
