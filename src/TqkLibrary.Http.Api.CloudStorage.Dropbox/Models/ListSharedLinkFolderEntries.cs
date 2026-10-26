using Newtonsoft.Json;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ListSharedLinkFolderEntries
    {
        [JsonProperty("entries")]
        public List<Entry> Entries { get; set; }

        [JsonProperty("share_tokens")]
        public List<ShareToken> ShareTokens { get; set; }

        [JsonProperty("shared_link_infos")]
        public List<SharedLinkInfo> SharedLinkInfos { get; set; }

        [JsonProperty("share_permissions")]
        public List<SharePermission> SharePermissions { get; set; }

        [JsonProperty("takedown_request_type")]
        public object TakedownRequestType { get; set; }

        [JsonProperty("total_num_entries")]
        public int TotalNumEntries { get; set; }

        [JsonProperty("has_more_entries")]
        public bool HasMoreEntries { get; set; }

        [JsonProperty("next_request_voucher")]
        public object NextRequestVoucher { get; set; }

        [JsonProperty("folder")]
        public Folder Folder { get; set; }

        [JsonProperty("folder_share_permission")]
        public FolderSharePermission FolderSharePermission { get; set; }

        [JsonProperty("folder_share_token")]
        public FolderShareToken FolderShareToken { get; set; }

        [JsonProperty("folder_shared_link_info")]
        public FolderSharedLinkInfo FolderSharedLinkInfo { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
