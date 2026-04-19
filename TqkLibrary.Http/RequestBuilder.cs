using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestBuilder
    {
        readonly BaseApi _baseApi;
        internal RequestBuilder(BaseApi baseApi)
        {
            this._baseApi = baseApi;
            this._httpClient = baseApi._httpClient;
        }
        HttpClient _httpClient { get; }
        HttpContent? _httpContent = null;
        bool _httpContentDispose = true;

        Dictionary<string, string> _headers = new Dictionary<string, string>();
        HttpMethod? _method = null;
        Uri? _uri = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public RequestBuilder WithUrl(UrlBuilder uri, HttpMethod method)
        {
            return WithUrl((Uri)uri, method);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlGet(UrlBuilder uri)
        {
            return WithUrl((Uri)uri, HttpMethod.Get);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlGet(string uri)
        {
            return WithUrl(new Uri(uri, UriKind.RelativeOrAbsolute), HttpMethod.Get);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlGet(Uri uri)
        {
            return WithUrl(uri, HttpMethod.Get);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public RequestBuilder WithUrl(string uri, HttpMethod method)
        {
            return WithUrl(new Uri(uri, UriKind.RelativeOrAbsolute), method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public RequestBuilder WithUrl(Uri uri, HttpMethod method)
        {
            this._uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this._method = method ?? throw new ArgumentNullException(nameof(method));
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpContent"></param>
        /// <param name="dispose"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPost(string uri, HttpContent httpContent, bool dispose = true)
        {
            return WithUrl(new Uri(uri, UriKind.RelativeOrAbsolute), HttpMethod.Post).WithBody(httpContent, dispose);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpContent"></param>
        /// <param name="dispose"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPost(Uri uri, HttpContent httpContent, bool dispose = true)
        {
            return WithUrl(uri, HttpMethod.Post).WithBody(httpContent, dispose);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpContent"></param>
        /// <param name="dispose"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPost(UrlBuilder uri, HttpContent httpContent, bool dispose = true)
        {
            return WithUrl((Uri)uri, HttpMethod.Post).WithBody(httpContent, dispose);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPostJson(string uri, object obj)
        {
            return WithUrl(new Uri(uri, UriKind.RelativeOrAbsolute), HttpMethod.Post).WithJsonBody(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPostJson(Uri uri, object obj)
        {
            return WithUrl(uri, HttpMethod.Post).WithJsonBody(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public RequestBuilder WithUrlPostJson(UrlBuilder uri, object obj)
        {
            return WithUrl((Uri)uri, HttpMethod.Post).WithJsonBody(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="dispose"></param>
        /// <returns></returns>
        public RequestBuilder WithBody(HttpContent httpContent, bool dispose = true)
        {
            this._httpContent = httpContent ?? throw new ArgumentNullException(nameof(httpContent));
            this._httpContentDispose = dispose;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RequestBuilder WithJsonBody(object obj)
        {
            return WithJsonBody(obj, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RequestBuilder WithJsonBody(object obj, Encoding encoding)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            string json = JsonConvert.SerializeObject(obj, _baseApi.DefaultJsonSerializerSettings);
            this._baseApi._logger?.LogTrace($"{nameof(WithJsonBody)}: {{json}}",json);
            this._httpContent = new StringContent(json, encoding, "application/json");
            this._httpContentDispose = true;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilder WithHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
            _headers.Add(key, value);
            return this;
        }
        public RequestBuilder ParseHeadersFromChrome(string headers)
        {
            if (string.IsNullOrEmpty(headers)) throw new ArgumentNullException(nameof(headers));
            /*
sec-ch-ua:
"Chromium";v="130", "Google Chrome";v="130", "Not?A_Brand";v="99"
sec-ch-ua-arch:
"x86"
sec-ch-ua-bitness:
"64"
sec-ch-ua-form-factors:
"Desktop"
             */
            using StringReader stringReader = new StringReader(headers);
            while (true)
            {
                string? header = stringReader.ReadLine()?.TrimEnd(':');
                if (string.IsNullOrWhiteSpace(header))
                    break;

                string? data = stringReader.ReadLine();
                if (string.IsNullOrWhiteSpace(data))
                    throw new InvalidOperationException($"invalid data format");

                _headers.Add(header!, data);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (_method == null || _uri == null) throw new InvalidOperationException($"method or uri is null");

            Uri? uri = _uri;
            if (!uri.IsAbsoluteUri)
            {
                if (_httpClient.BaseAddress == null)
                    throw new InvalidOperationException("Relative URI requires HttpClient.BaseAddress.");
                if (!Uri.TryCreate(_httpClient.BaseAddress, _uri, out uri))
                    throw new UriFormatException($"Cannot combine BaseAddress '{_httpClient.BaseAddress}' with relative URI '{_uri}'.");
            }

            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(_method, uri);

            foreach (var header in _headers)
            {
                this._baseApi._logger?.LogTrace("Header: {key}: {value}", header.Key, header.Value);
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            if (httpRequestMessage.Headers.Accept.Count == 0 && _httpClient.DefaultRequestHeaders.Accept.Count == 0)
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(httpRequestMessage.Headers.Host) && string.IsNullOrWhiteSpace(_httpClient.DefaultRequestHeaders.Host))
                httpRequestMessage.Headers.Host = uri.Host;

            if (_httpContent != null)
                httpRequestMessage.Content = _httpContent;
            await _baseApi.OnBeforeRequestAsync(httpRequestMessage);

            HttpResponseMessage httpResponseMessage = await _httpClient
                .SendAsync(
                    httpRequestMessage, 
                    HttpCompletionOption.ResponseHeadersRead, 
                    cancellationToken
                    )
                .ConfigureAwait(false);

            if (_httpContentDispose) _httpContent?.Dispose();

            await _baseApi.OnAfterRequestAsync(httpResponseMessage);

            return httpResponseMessage;
        }
        bool _isCheckStatusCode = true;
        public RequestBuilder WithCheckStatusCode(bool isCheck)
        {
            _isCheckStatusCode = isCheck;
            return this;
        }


        static readonly Type typeString = typeof(string);
        static readonly Type typeBuffer = typeof(byte[]);
        static readonly Type typeStream = typeof(Stream);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<TResult> ExecuteAsync<TResult, TException>(CancellationToken cancellationToken = default)
            where TResult : class
            where TException : class
        {
            HttpResponseMessage rep = await ExecuteAsync(cancellationToken);
            if (!_isCheckStatusCode || rep.IsSuccessStatusCode)
            {
                if (typeof(TResult).Equals(typeBuffer))
                {
                    try
                    {
#if NETFRAMEWORK || NETSTANDARD
                        return ((await rep.Content.ReadAsByteArrayAsync()) as TResult)!;
#else
                        return ((await rep.Content.ReadAsByteArrayAsync(cancellationToken)) as TResult)!;
#endif
                    }
                    finally
                    {
                        rep.Dispose();
                    }
                }
                if (typeof(TResult).Equals(typeStream))//not dispose rep
                {
                    Stream stream = await rep.Content.ReadAsStreamAsync();
                    return new HttpResponseStreamWrapper(rep, stream) as TResult;
                }
            }


            try
            {
                string? content_res = await rep.Content
#if NETFRAMEWORK || NETSTANDARD
                        .ReadAsStringAsync()
#else
                    .ReadAsStringAsync(cancellationToken)
#endif
                        .ConfigureAwait(false);

                if (!_isCheckStatusCode || rep.IsSuccessStatusCode)
                {
                    if (typeof(TResult).Equals(typeString))
                    {
                        return (content_res as TResult)!;
                    }
                    else
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<TResult>(content_res, _baseApi.DefaultJsonSerializerSettings)!;
                        }
                        catch
                        {
                            //jump to error
                        }
                    }
                }

                if (typeof(TException).Equals(typeString))
                {
                    throw new ApiException<string>()
                    {
                        Body = content_res,
                        StatusCode = rep.StatusCode
                    };
                }
                else throw new ApiException<TException>()
                {
                    Body = JsonConvert.DeserializeObject<TException>(content_res)!,
                    StatusCode = rep.StatusCode
                };
            }
            finally
            {
                rep.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public Task<TResult> ExecuteAsync<TResult>(CancellationToken cancellationToken = default) where TResult : class
        {
            return ExecuteAsync<TResult, string>(cancellationToken);
        }
    }
}