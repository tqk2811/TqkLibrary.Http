using Newtonsoft.Json;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.Other.ImagesHostApi.ImgurCom.DataClass
{
    public class ImgurImage
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("datetime", NullValueHandling = NullValueHandling.Ignore)]
        public long? Datetime { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("animated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Animated { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public long? Width { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public long? Height { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
        public long? Views { get; set; }

        [JsonProperty("bandwidth", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bandwidth { get; set; }

        //[JsonProperty("vote")]
        //public object Vote { get; set; }

        [JsonProperty("favorite", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Favorite { get; set; }

        //[JsonProperty("nsfw")]
        //public object Nsfw { get; set; }

        //[JsonProperty("section")]
        //public object Section { get; set; }

        //[JsonProperty("account_url")]
        //public object AccountUrl { get; set; }

        [JsonProperty("account_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccountId { get; set; }

        [JsonProperty("is_ad", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAd { get; set; }

        [JsonProperty("in_most_viral", NullValueHandling = NullValueHandling.Ignore)]
        public bool? InMostViral { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        [JsonProperty("ad_type", NullValueHandling = NullValueHandling.Ignore)]
        public long? AdType { get; set; }

        [JsonProperty("ad_url", NullValueHandling = NullValueHandling.Ignore)]
        public string AdUrl { get; set; }

        [JsonProperty("in_gallery", NullValueHandling = NullValueHandling.Ignore)]
        public bool? InGallery { get; set; }

        [JsonProperty("deletehash", NullValueHandling = NullValueHandling.Ignore)]
        public string Deletehash { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public string Link { get; set; }
    }
}