using Newtonsoft.Json;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Entry
    {
        [JsonProperty("bytes")]
        public int Bytes { get; set; }

        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("ns_id")]
        public int NsId { get; set; }

        [JsonProperty("open_in_app_data")]
        public object OpenInAppData { get; set; }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("preview_type")]
        public string PreviewType { get; set; }

        [JsonProperty("revision_id")]
        public string RevisionId { get; set; }

        [JsonProperty("sjid")]
        public int Sjid { get; set; }

        [JsonProperty("sort_key")]
        public List<string> SortKey { get; set; }

        [JsonProperty("thumbnail_url_tmpl")]
        public string ThumbnailUrlTmpl { get; set; }

        [JsonProperty("ts")]
        public int Ts { get; set; }

        [JsonProperty("is_symlink")]
        public bool IsSymlink { get; set; }

        [JsonProperty("is_cloud_doc")]
        public bool IsCloudDoc { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
