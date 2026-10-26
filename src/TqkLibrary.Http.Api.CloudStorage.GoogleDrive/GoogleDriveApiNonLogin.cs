using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.CloudStorage.GoogleDrive
{
    /// <summary>
    /// 
    /// </summary>
    public class GoogleDriveApiNonLogin : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApiKey { get; set; } = "AIzaSyC1qbk75NzWBvSaDh6KnsjjA9pIrP4lYIE";
        readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        public GoogleDriveApiNonLogin() : this(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
            },
            true
        )
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public GoogleDriveApiNonLogin(HttpMessageHandler httpMessageHandler, bool disposeHandler) : this(new HttpClient(httpMessageHandler, disposeHandler))
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected GoogleDriveApiNonLogin(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        /// <summary>
        /// 
        /// </summary>
        ~GoogleDriveApiNonLogin()
        {
            _httpClient.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<FileList> ListPublicFolderAsync(DriveFileListOption option, CancellationToken cancellationToken = default)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            string? url = option.NextLink;

            if (string.IsNullOrEmpty(url))
            {
                if (string.IsNullOrWhiteSpace(option.Query)) throw new ArgumentNullException(nameof(option.Query));
                if (string.IsNullOrWhiteSpace(option.Fields)) throw new ArgumentNullException(nameof(option.Fields));
                if (string.IsNullOrWhiteSpace(option.OrderBy)) throw new ArgumentNullException(nameof(option.OrderBy));

                NameValueCollection queryBuilder = HttpUtility.ParseQueryString(string.Empty);
                queryBuilder.Add("openDrive", "false");
                queryBuilder.Add("reason", "102");
                queryBuilder.Add("syncType", "0");
                queryBuilder.Add("errorRecovery", "false");
                queryBuilder.Add("appDataFilter", "NO_APP_DATA");
                queryBuilder.Add("spaces", "drive");
                queryBuilder.Add("maxResults", option.MaxResults.ToString());
                queryBuilder.Add("supportsTeamDrives", option.SupportsTeamDrives.ToString().ToLower());
                queryBuilder.Add("includeTeamDriveItems", option.IncludeTeamDriveItems.ToString().ToLower());
                queryBuilder.Add("corpora", "default");
                queryBuilder.Add("retryCount", "0");
                queryBuilder.Add("key", ApiKey);
                queryBuilder.Add("q", option.Query);
                queryBuilder.Add("fields", option.Fields);
                queryBuilder.Add("orderBy", option.OrderBy);
                if (!string.IsNullOrEmpty(option.PageToken)) queryBuilder.Add("pageToken", option.PageToken);

                url = $"https://clients6.google.com/drive/v2beta/files?{queryBuilder.ToString()!.Replace("+", "%20")}";
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Referrer = new Uri("https://drive.google.com/");
            httpRequestMessage.Headers.Add("Accept", "application/json");
            if (!string.IsNullOrWhiteSpace(option.Resourcekey))
                httpRequestMessage.Headers.Add("x-goog-drive-resource-keys", $"{option.FolderId}/{option.Resourcekey}");
            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            string json_text = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FileList>(json_text)!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Google.Apis.Drive.v2.Data.File> GetMetadataAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentNullException(fileId);

            var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryBuilder.Add("supportsTeamDrives", "true");
            queryBuilder.Add("includeBadgedLabels", "true");
            queryBuilder.Add("enforceSingleParent", "true");
            queryBuilder.Add("key", ApiKey);
            queryBuilder.Add("fields", "*");

            string url = $"https://content.googleapis.com/drive/v2beta/files/{fileId}?{queryBuilder.ToString()}";
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Referrer = new Uri("https://drive.google.com/");
            httpRequestMessage.Headers.Add("Accept", "application/json");
            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            string json_text = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Google.Apis.Drive.v2.Data.File>(json_text)!;
        }

        static readonly Regex regex_form = new Regex("form.*?action=\"(.*?)\" method=\"(.*?)\"");
        static readonly Regex regex_formItem = new Regex("name=\"(.*?)\" value=\"(.*?)\"");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            string url = $"https://drive.google.com/uc?export=download&id={fileId}";
            HttpMethod method = HttpMethod.Get;
            int tryCount = 0;
            while (true)
            {
                tryCount++;
                if (tryCount > 5)
                {
                    throw new Exception($"Can't download file {fileId}");
                }

                using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, url);
                //httpRequestMessage.Headers.Referrer = new Uri("https://drive.google.com/");
                //httpRequestMessage.Headers.Add("Origin", "https://drive.google.com/");
                HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                httpResponseMessage.EnsureSuccessStatusCode();
                if (httpResponseMessage.Content.Headers.ContentType?.MediaType?.Contains("text/html") == true)
                {
                    try
                    {
                        string content = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

                        Match match = regex_form.Match(content);
                        if (match.Success)
                        {
                            string method_str = match.Groups[2].Value.ToLower();
                            switch (method_str)
                            {
                                case "get":
                                    {
                                        var matches = regex_formItem.Matches(content);
                                        Uri uri = new Uri(HttpUtility.HtmlDecode(match.Groups[1].Value));
                                        var q = HttpUtility.ParseQueryString(uri.Query);
                                        foreach (Match m in matches)
                                        {
                                            q[m.Groups[1].Value] = HttpUtility.HtmlDecode(m.Groups[2].Value);
                                        }
                                        var q_str = q.ToString();
                                        if (string.IsNullOrWhiteSpace(q_str))
                                        {
                                            url = uri.OriginalString;
                                            method = HttpMethod.Get;
                                        }
                                        else
                                        {
                                            if (string.IsNullOrWhiteSpace(uri.Query))
                                            {
                                                url = $"{uri.OriginalString}?{q_str}";
                                            }
                                            else
                                            {
                                                url = $"{uri.OriginalString.Replace(uri.Query, string.Empty)}?{q_str}";
                                            }
                                            method = HttpMethod.Get;
                                        }
                                        continue;
                                    }

                                case "post":
                                    {
                                        url = HttpUtility.HtmlDecode(match.Groups[1].Value);
                                        method = HttpMethod.Post;
                                        continue;
                                    }
                            }
                        }

                        throw new Exception(content);
                    }
                    finally
                    {
                        httpResponseMessage.Dispose();
                    }
                }
                else// if (httpResponseMessage.Content.Headers.ContentType.MediaType.Contains("application"))
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    return new HttpResponseStreamWrapper(httpResponseMessage, stream);
                }
                //throw new InvalidDataException($"Can't find download link for fileId: {fileId}, {url}");
            }
        }
    }
}