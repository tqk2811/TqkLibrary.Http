using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FolderSharedLinkInfo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("downloadTestUrl")]
        public string DownloadTestUrl { get; set; }

        [JsonProperty("hasPublicAudienceOrVisibility")]
        public bool HasPublicAudienceOrVisibility { get; set; }

        [JsonProperty("ownerName")]
        public string OwnerName { get; set; }

        [JsonProperty("ownerTeamLogo")]
        public object OwnerTeamLogo { get; set; }

        [JsonProperty("ownerTeamBackground")]
        public object OwnerTeamBackground { get; set; }

        [JsonProperty("ownerTeamName")]
        public object OwnerTeamName { get; set; }

        [JsonProperty("teamMemberBrandingPolicyEnabled")]
        public bool TeamMemberBrandingPolicyEnabled { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
