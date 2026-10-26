using Microsoft.Graph.Models;
using Microsoft.Kiota.Serialization.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TqkLibrary.Http.CloudStorage.OneDrive
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OneDriveApiNonLogin : IDisposable
    {
        const string EndPoint = "https://api.onedrive.com/v1.0/drives/";

        readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        public OneDriveApiNonLogin() : this(
#if NET5_0_OR_GREATER
            new SocketsHttpHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false,
            }.DisableFindIpV6(),
#else
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false,
            },
#endif
            true
        )
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public OneDriveApiNonLogin(HttpMessageHandler httpMessageHandler, bool disposeHandler) : this(new HttpClient(httpMessageHandler, disposeHandler))
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected OneDriveApiNonLogin(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        /// <summary>
        /// 
        /// </summary>
        ~OneDriveApiNonLogin()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool isDisposing)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<OneDriveLinkInfo?> DecodeShortLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri is null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.Host.Equals("1drv.ms"))
                throw new InvalidOperationException($"Short link host must be '1drv.ms', current is '{uri.Host}'");

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (httpResponseMessage.Headers.Location is null)
                return null;
            var query = HttpUtility.ParseQueryString(httpResponseMessage.Headers.Location.Query);

            using HttpRequestMessage httpRequestMessage2 = new HttpRequestMessage(HttpMethod.Get, httpResponseMessage.Headers.Location);
            using HttpResponseMessage httpResponseMessage2 = await _httpClient.SendAsync(httpRequestMessage2, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (httpResponseMessage2.Headers.Location is not null)
            {
                var query2 = HttpUtility.ParseQueryString(
                    httpResponseMessage2.Headers.Location.IsAbsoluteUri ?
                        httpResponseMessage2.Headers.Location.Query :
                        httpResponseMessage2.Headers.Location.OriginalString.TrimStart('/')
                        );
                foreach (var item in query2.AllKeys)
                {
                    query[item] = query2[item];
                }
            }
            string[] paths = uri.AbsolutePath.TrimStart('/').Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            return new OneDriveLinkInfo()
            {
                Id = query["id"],
                ResourceId = query["resid"],
                AuthKey = query["authkey"],
                ItHint = query["ithint"],
                E = query["e"],
                MigratedTospo = query["migratedtospo"],
                Redeem = query["redeem"],
                Cid = query["cid"],
                Type = paths.FirstOrDefault(),
            };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DriveItem> GetMetadataAsync(OneDriveLinkInfo oneDriveLinkInfo, CancellationToken cancellationToken = default)
            => GetMetadataAsync(oneDriveLinkInfo?.ResourceId!, oneDriveLinkInfo?.AuthKey!, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DriveItem> GetMetadataAsync(string itemId, string authKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentNullException(nameof(itemId));
            if (string.IsNullOrWhiteSpace(authKey))
                throw new ArgumentNullException(nameof(authKey));
            if (!itemId.Contains("!") || itemId.Count(x => x == '!') != 1)
                throw new InvalidOperationException($"invalid {nameof(itemId)}, must be contain only one '!'");
            string[] itemIdSplit = itemId.Split('!');

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["select"] = "createdBy,createdDateTime,description,driveId,file,fileSystemInfo,id,@content.downloadUrl,image,lastModifiedDateTime,location,name,parentReference,photo,size,shared,tags,video,viewpoint";
            query["expand"] = "tags";
            query["authkey"] = authKey;

            var url = new UrlBuilder(EndPoint, itemIdSplit[0], "items", itemId)
                .WithParams(query);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url.ToString());
            httpRequestMessage.Headers.Add("Origin", "https://onedrive.live.com");
            httpRequestMessage.Headers.Referrer = new Uri("https://onedrive.live.com");

            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);

            string text = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage.EnsureSuccessStatusCode();
            JsonParseNode jsonParseNode = new JsonParseNode(JsonDocument.Parse(text).RootElement);
            return jsonParseNode.GetObjectValue(DriveItem.CreateFromDiscriminatorValue);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Stream> DownloadFileAsync(OneDriveLinkInfo oneDriveLinkInfo, CancellationToken cancellationToken = default)
            => DownloadFileAsync(oneDriveLinkInfo?.ResourceId!, oneDriveLinkInfo?.AuthKey!, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(string itemId, string authKey, CancellationToken cancellationToken = default)
        {
            var driverItem = await GetMetadataAsync(itemId, authKey, cancellationToken);
            return await DownloadFileAsync(driverItem, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(DriveItem driveItem, CancellationToken cancellationToken = default)
        {
            if (driveItem.AdditionalData.TryGetValue("@content.downloadUrl", out object? val) && val is string url)
            {
                using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequestMessage.Headers.Add("Origin", "https://onedrive.live.com");
                httpRequestMessage.Headers.Referrer = new Uri("https://onedrive.live.com");

                HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
                return new HttpResponseStreamWrapper(httpResponseMessage, await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStreamAsync());
            }
            else throw new InvalidOperationException($"invalid downloadUrl");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DriveItemCollectionResponse> ListChildItems(OneDriveLinkInfo oneDriveLinkInfo, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(oneDriveLinkInfo?.AuthKey))
            {
                return await ListChildItems(oneDriveLinkInfo?.ResourceId!, oneDriveLinkInfo?.AuthKey!, cancellationToken);
            }
            else
            {
                TokenApiV2Data tokenApiV2Data = await GetTokenApiV2(cancellationToken);
                return await ListChildItemsV2(oneDriveLinkInfo?.ResourceId!, tokenApiV2Data, cancellationToken);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<DriveItemCollectionResponse> ListChildItems(string itemId, string authKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentNullException(nameof(itemId));
            if (string.IsNullOrWhiteSpace(authKey))
                throw new ArgumentNullException(nameof(authKey));
            if (!itemId.Contains("!") || itemId.Count(x => x == '!') != 1)
                throw new InvalidOperationException($"invalid {nameof(itemId)}, must be contain only one '!'");
            string[] itemIdSplit = itemId.Split('!');

            var query = HttpUtility.ParseQueryString(string.Empty);
            //query["top"] = "100";
            //query["expand"] = "thumbnails,lenses,tags";
            query["select"] = "*,ocr,webDavUrl,sharepointIds,isRestricted,commentSettings,specialFolder,containingDrivePolicyScenarioViewpoint";
            query["authkey"] = authKey;
            query["ump"] = "1";

            var url = new UrlBuilder(EndPoint, itemIdSplit[0].ToLower(), "items", itemId, "children")
                .WithParams(query);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url.ToString());
            httpRequestMessage.Headers.Add("Origin", "https://onedrive.live.com");
            httpRequestMessage.Headers.Referrer = new Uri("https://onedrive.live.com");

            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);

            string text = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage.EnsureSuccessStatusCode();
            JsonParseNode jsonParseNode = new JsonParseNode(JsonDocument.Parse(text).RootElement);
            return jsonParseNode.GetObjectValue(DriveItemCollectionResponse.CreateFromDiscriminatorValue);
        }



        const string EndPointV2 = "https://my.microsoftpersonalcontent.com/_api/v2.0/drives";
        public string AppIdApiV2 { get; set; } = "5cbed6ac-a083-4e14-b191-b4ba07653de2";
        public async Task<TokenApiV2Data> GetTokenApiV2(CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api-badgerp.svc.ms/v1.0/token");
            using StringContent stringContent = new StringContent($"{{\"appId\":\"{AppIdApiV2}\"}}", Encoding.UTF8, "application/json");
            httpRequestMessage.Content = stringContent;
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequestMessage.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            httpRequestMessage.Headers.Add("Accept-Language", "en");
            httpRequestMessage.Headers.Add("Appid", "1141147648");
            httpRequestMessage.Headers.Add("Cache-Control", "private");
            httpRequestMessage.Headers.TryAddWithoutValidation("Content-Type", "application/json;odata=verbose");
            httpRequestMessage.Headers.Add("Dnt", "1");
            httpRequestMessage.Headers.Add("Origin", "https://onedrive.live.com");
            httpRequestMessage.Headers.Add("Priority", "u=1, i");
            httpRequestMessage.Headers.Referrer = new Uri("https://onedrive.live.com");
            httpRequestMessage.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"136\", \"Google Chrome\";v=\"136\", \"Not.A/Brand\";v=\"99\"");
            httpRequestMessage.Headers.Add("sec-ch-ua-mobile", "?0");
            httpRequestMessage.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            httpRequestMessage.Headers.Add("sec-fetch-dest", "empty");
            httpRequestMessage.Headers.Add("sec-fetch-mode", "cors");
            httpRequestMessage.Headers.Add("sec-fetch-site", "cross-site");
            httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
            httpRequestMessage.Headers.Add("X-Forcecache", "1");
            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
            string text = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TokenApiV2Data>(text)!;
        }
        public async Task<DriveItemCollectionResponse> ListChildItemsV2(string itemId, TokenApiV2Data tokenApiV2Data, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentNullException(nameof(itemId));
            if (string.IsNullOrWhiteSpace(tokenApiV2Data?.AuthScheme))
                throw new ArgumentNullException(nameof(tokenApiV2Data));
            if (string.IsNullOrWhiteSpace(tokenApiV2Data?.Token))
                throw new ArgumentNullException(nameof(tokenApiV2Data));

            if (!itemId.Contains("!") || itemId.Count(x => x == '!') != 1)
                throw new InvalidOperationException($"invalid {nameof(itemId)}, must be contain only one '!'");

            string[] itemIdSplit = itemId.Split('!');

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["%24top"] = "100";
            query["orderby"] = "folder,name";
            query["%24expand"] = "thumbnails,tags";
            query["select"] = "*,ocr,webDavUrl,sharepointIds,isRestricted,commentSettings,specialFolder,containingDrivePolicyScenarioViewpoint";
            query["ump"] = "1";
            var url = new UrlBuilder(EndPointV2, itemIdSplit[0].ToLower(), "items", itemId, "children")
                .WithParams(query);

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url.ToString());
            httpRequestMessage.Headers.Add("Origin", "https://onedrive.live.com");
            httpRequestMessage.Headers.Referrer = new Uri("https://onedrive.live.com");
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            //httpRequestMessage.Headers.Add("Dnt", "1");
            //httpRequestMessage.Headers.Add("Priority", "u=1, i");
            //httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
            //httpRequestMessage.Headers.Add("Accept-Encoding", "deflate");

            //httpRequestMessage.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"136\", \"Google Chrome\";v=\"136\", \"Not.A/Brand\";v=\"99\"");
            //httpRequestMessage.Headers.Add("sec-ch-ua-mobile", "?0");
            //httpRequestMessage.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            //httpRequestMessage.Headers.Add("sec-fetch-dest", "empty");
            //httpRequestMessage.Headers.Add("sec-fetch-mode", "cors");
            //httpRequestMessage.Headers.Add("sec-fetch-site", "cross-site");
            //httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(tokenApiV2Data!.AuthScheme, tokenApiV2Data!.Token);

            string boundary = Guid.NewGuid().ToString();
            string content = @$"--{boundary}
Content-Disposition: form-data;name=data
Prefer: HonorNonIndexedQueriesWarningMayFailRandomly, allowthrottleablequeries, Include-Feature=AddToOneDrive;Vault
X-ClientService-ClientTag: ODC Web
Application: ODC Web
Scenario: BrowseFiles
ScenarioType: AUO
X-HTTP-Method-Override: GET
Content-Type: application/json
Authorization: {tokenApiV2Data!.AuthScheme} {tokenApiV2Data!.Token}


--{boundary}--";
            using ByteArrayContent stringContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
            stringContent.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data;boundary={boundary}");
            httpRequestMessage.Content = stringContent;
            using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);

            string text = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage.EnsureSuccessStatusCode();
            JsonParseNode jsonParseNode = new JsonParseNode(JsonDocument.Parse(text).RootElement);
            return jsonParseNode.GetObjectValue(DriveItemCollectionResponse.CreateFromDiscriminatorValue);
        }
    }
}