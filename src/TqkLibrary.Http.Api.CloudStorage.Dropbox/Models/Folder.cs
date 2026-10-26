using Newtonsoft.Json;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Folder
    {
        [JsonProperty("_mount_access_perms")]
        public List<string> MountAccessPerms { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("open_in_app_data")]
        public object OpenInAppData { get; set; }

        [JsonProperty("shared_folder_id")]
        public object SharedFolderId { get; set; }

        [JsonProperty("ns_id")]
        public int NsId { get; set; }

        [JsonProperty("sort_key")]
        public List<string> SortKey { get; set; }

        [JsonProperty("folder_id")]
        public string FolderId { get; set; }

        [JsonProperty("is_confidential_folder")]
        public bool IsConfidentialFolder { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
