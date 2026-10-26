using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Preview
    {
        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
