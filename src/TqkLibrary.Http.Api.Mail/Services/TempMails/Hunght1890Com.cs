using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Mail.Services.TempMails
{
    public class Hunght1890Com : BaseApi
    {
        const string _Endpoint = "https://hunght1890.com";
        public Hunght1890Com()
        {

        }
        public Hunght1890Com(HttpMessageHandler httpMessageHandler, bool disposeHandler = false) : base(httpMessageHandler, disposeHandler)
        {

        }

        public Task<ConfigureData> GetConfigureAsync(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_Endpoint, "static/config.json"))
            .ExecuteAsync<ConfigureData>(cancellationToken);




        public Task<List<MailData>> GetMailsAsync(string address, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_Endpoint, address))
            .WithCheckStatusCode(false)
            .ExecuteAsync<List<MailData>>();






        public class ConfigureData
        {
            [JsonProperty("domains")]
            public List<string>? Domains { get; set; }
        }
        public class MailData
        {
            [JsonProperty("body")]
            public string? Body { get; set; }

            [JsonProperty("from")]
            public string? From { get; set; }

            [JsonProperty("subject")]
            public string? Subject { get; set; }

            /// <summary>
            /// YYYY-MM-dd HH:mm:ss (utc)
            /// </summary>
            [JsonProperty("timestamp")]
            public string? Timestamp { get; set; }

            [JsonProperty("to")]
            public string? To { get; set; }
        }
    }
}
