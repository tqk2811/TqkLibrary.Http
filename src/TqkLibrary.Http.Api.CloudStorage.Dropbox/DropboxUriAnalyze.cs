using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class DropboxUriAnalyze
    {
        const string baseUrl = "https://www.dropbox.com/scl/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public DropboxUriAnalyze(string url) : this(new Uri(url))
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public DropboxUriAnalyze(Uri uri)
        {
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));

            string url = uri.ToString();
            if (!url.StartsWith(baseUrl))
                throw new InvalidOperationException($"Invalid dropbox link '{uri}'");

            string[] paths = uri.AbsolutePath.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();// "scl/fo/7metrzawj5yxaed6lo3pg/h" or "scl/fi/0sskovcwdudgl97x2fuz1/Intro.mp4"
            if (paths.Length < 4)
                throw new InvalidOperationException($"Invalid dropbox link '{uri}'");

            _query = HttpUtility.ParseQueryString(uri.Query);
            LinkType = paths[1] switch
            {
                "fo" => DropboxLinkType.Folder,
                "fi" => DropboxLinkType.File,
                "sh" => DropboxLinkType.Share,
                _ => null,
            };
            Id = paths[2];
            Name = paths[3];
        }

        readonly NameValueCollection _query;

        /// <summary>
        /// 
        /// </summary>
        public string? RlKey => _query["rlkey"];

        /// <summary>
        /// 
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// 
        /// </summary>
        public DropboxLinkType? LinkType { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public static implicit operator DropboxUriAnalyze(string url) => new DropboxUriAnalyze(url);
    }
}