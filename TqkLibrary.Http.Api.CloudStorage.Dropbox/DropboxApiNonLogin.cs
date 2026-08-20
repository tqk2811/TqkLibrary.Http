using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TqkLibrary.Http;
using TqkLibrary.Http.Api.CloudStorage.Dropbox.Models;
using TqkLibrary.Http.HttpClientHandles;

namespace TqkLibrary.Http.Api.CloudStorage.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class DropboxApiNonLogin : IDisposable
    {
        readonly HttpClient httpClient;
        readonly CookieHandler cookieHandler;
        /// <summary>
        /// 
        /// </summary>
        public DropboxApiNonLogin() : this(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = false,
            },
            true
        )
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public DropboxApiNonLogin(HttpClientHandler httpClientHandler, bool disposeHandler) : this(new CookieHandler(httpClientHandler), disposeHandler)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        protected DropboxApiNonLogin(CookieHandler cookieHandler, bool disposeHandler) : this(new HttpClient(cookieHandler, disposeHandler))
        {
            this.cookieHandler = cookieHandler ?? throw new ArgumentNullException(nameof(cookieHandler));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected DropboxApiNonLogin(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        /// <summary>
        /// 
        /// </summary>
        ~DropboxApiNonLogin()
        {
            httpClient.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task GetCookieAsync(string url = "https://www.dropbox.com/", CancellationToken cancellationToken = default)
            => GetCookieAsync(new Uri(url), cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetCookieAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            if (!uri.ToString().StartsWith("https://www.dropbox.com"))
                throw new InvalidOperationException($"url must be StartsWith 'https://www.dropbox.com'");
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string? GetT()
        {
            var cookies = cookieHandler.CookieContainer.GetCookies(new Uri("https://www.dropbox.com"));
            foreach (Cookie cookie in cookies)
            {
                if ("t".Equals(cookie.Name))
                    return cookie.Value;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ListSharedLinkFolderEntries> ListPublicFolderAsync(DropboxUriAnalyze folder, CancellationToken cancellationToken = default)
        {
            if (folder is null)
                throw new ArgumentNullException(nameof(folder));
            if (folder.LinkType != DropboxLinkType.Folder)
                throw new InvalidOperationException($"This link folder is not support '{folder.Uri}'");

            if (string.IsNullOrWhiteSpace(GetT()))
            {
                await GetCookieAsync(folder.Uri, cancellationToken);
            }

            var folderQuery = HttpUtility.ParseQueryString(folder.Uri.Query);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.dropbox.com/list_shared_link_folder_entries");

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["is_xhr"] = "true";
            query["t"] = GetT();
            query["link_key"] = folder.Id;
            query["link_type"] = "c";
            query["secure_hash"] = folder.Name;
            query["sub_path"] = string.Empty;
            query["rlkey"] = folder.RlKey;
            using StringContent form = new StringContent(query.ToString()!, Encoding.UTF8, "application/x-www-form-urlencoded");
            httpRequestMessage.Content = form;
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
            string json_text = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<ListSharedLinkFolderEntries>(json_text)!;
        }

#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetMetadataAsync(DropboxUriAnalyze file, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));
            if (file.LinkType != DropboxLinkType.File)
                throw new InvalidOperationException($"This link file is not support '{file.Uri}'");

            if (string.IsNullOrWhiteSpace(GetT()))
            {
                await GetCookieAsync(file.Uri, cancellationToken);
            }

            var folderQuery = HttpUtility.ParseQueryString(file.Uri.Query);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.dropbox.com/2/files/get_file_content_metadata");
            //httpRequestMessage.Headers.Referrer = new Uri("https://drive.google.com/");
            //httpRequestMessage.Headers.Add("Accept", "application/json");
            //using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            //string json_text = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false);
            //return JsonConvert.DeserializeObject<Google.Apis.Drive.v2.Data.File>(json_text);
            return null;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(DropboxUriAnalyze file, CancellationToken cancellationToken = default)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));
            if (file.LinkType != DropboxLinkType.File)
                throw new InvalidOperationException($"This link file is not support '{file.Uri}'");

            if (string.IsNullOrWhiteSpace(GetT()))
            {
                await GetCookieAsync(file.Uri, cancellationToken);
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.dropbox.com/sharing/fetch_user_content_link");

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["is_xhr"] = "true";
            query["t"] = GetT();
            query["url"] = file.Uri.ToString();
            query["origin"] = "PREVIEW_PAGE";
            using StringContent form = new StringContent(query.ToString()!, Encoding.UTF8, "application/x-www-form-urlencoded");
            httpRequestMessage.Content = form;
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();
            if (httpResponseMessage.Content.Headers.ContentType?.MediaType?.Contains("text/plain") == true)
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();

                using HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, content);
                HttpResponseMessage res = await httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                Stream stream = await res.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
                return new HttpResponseStreamWrapper(httpResponseMessage, stream);
            }
            else
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new ApiException<string>(httpResponseMessage, content, content);
            }
        }
    }
}