using Newtonsoft.Json;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FolderSharePermission
    {
        [JsonProperty("canCopyToDropboxRoles")]
        public List<string> CanCopyToDropboxRoles { get; set; }

        [JsonProperty("canSyncToDropboxRoles")]
        public List<object> CanSyncToDropboxRoles { get; set; }

        [JsonProperty("canRequestAccessRoles")]
        public List<object> CanRequestAccessRoles { get; set; }

        [JsonProperty("canDownloadRoles")]
        public List<string> CanDownloadRoles { get; set; }

        [JsonProperty("canRemoveLinkUids")]
        public List<object> CanRemoveLinkUids { get; set; }

        [JsonProperty("canPrintRoles")]
        public List<string> CanPrintRoles { get; set; }

        [JsonProperty("canViewContextMenuRoles")]
        public List<string> CanViewContextMenuRoles { get; set; }

        [JsonProperty("canViewMetadataRoles")]
        public List<object> CanViewMetadataRoles { get; set; }

        [JsonProperty("isEditFolderLink")]
        public bool IsEditFolderLink { get; set; }

        [JsonProperty("isViewFolderLink")]
        public bool IsViewFolderLink { get; set; }

        [JsonProperty("syncVarsByRoles")]
        public object SyncVarsByRoles { get; set; }

        [JsonProperty("isSharedFolder")]
        public bool IsSharedFolder { get; set; }

        [JsonProperty("folderLinkPreviewType")]
        public int FolderLinkPreviewType { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
