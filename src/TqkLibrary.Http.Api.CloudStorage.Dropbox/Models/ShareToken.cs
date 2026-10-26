using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ShareToken
    {
        [JsonProperty("itemId")]
        public object ItemId { get; set; }

        [JsonProperty("linkType")]
        public string LinkType { get; set; }

        [JsonProperty("linkKey")]
        public string LinkKey { get; set; }

        [JsonProperty("subPath")]
        public string SubPath { get; set; }

        [JsonProperty("secureHash")]
        public string SecureHash { get; set; }

        [JsonProperty("rlkey")]
        public string Rlkey { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
