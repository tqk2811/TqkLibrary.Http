using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlBuilder
    {
        string _url = string.Empty;
        readonly NameValueCollection _nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public UrlBuilder(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            this._url = url;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urls"></param>
        public UrlBuilder(params string[] urls)
        {
            if (urls == null || urls.Length == 0) throw new ArgumentNullException(nameof(urls));
            this._url = string.Join("/", urls.Select(x => x.TrimStart('/').TrimEnd('/')));
            if (string.IsNullOrWhiteSpace(_url)) throw new ArgumentNullException(nameof(_url));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urls"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlBuilder(params object[] urls)
        {
            if (urls == null || urls.Length == 0) throw new ArgumentNullException(nameof(urls));
            this._url = string.Join("/", urls.Select(x => x.ToString()?.TrimStart('/').TrimEnd('/') ?? throw new ArgumentNullException($"Some element in {nameof(urls)} are null")));
            if (string.IsNullOrWhiteSpace(_url)) throw new ArgumentNullException(nameof(_url));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual UrlBuilder WithParam(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            // List<int>, int[]... are not IEnumerable<object> so they land here instead of the collection overload
            if (IsParamList(value, out IEnumerable? enumerable))
            {
                bool any = false;
                foreach (object item in enumerable!)
                {
                    WithParam(name, item);
                    any = true;
                }
                if (!any) throw new ArgumentNullException(nameof(value));
                return this;
            }
            string? v = FormatValue(value);
            if (string.IsNullOrWhiteSpace(v)) throw new ArgumentNullException(nameof(value));
            _nameValueCollection.Add(name, v);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlBuilder WithParam(string name, IEnumerable<object> values)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (values is null || !values.Any()) throw new ArgumentNullException(nameof(values));
            foreach (object value in values) WithParam(name, value);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual UrlBuilder WithParams(NameValueCollection nameValueCollection)
        {
            if (nameValueCollection is null) throw new ArgumentNullException(nameof(nameValueCollection));
            this._nameValueCollection.Add(nameValueCollection);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual UrlBuilder WithParamIfNotNull(string name, object? value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (IsParamList(value, out IEnumerable? enumerable))
            {
                foreach (object? item in enumerable!) WithParamIfNotNull(name, item);
            }
            else if (value != null)
            {
                string? v = FormatValue(value);
                if (!string.IsNullOrWhiteSpace(v)) _nameValueCollection.Add(name, v);
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlBuilder WithParamIfNotNull(string name, IEnumerable<object?>? values)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (values is null || !values.Any()) return this;
            foreach (object? value in values) WithParamIfNotNull(name, value);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlBuilder WithParamIf(string name, Func<bool> condition, object value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (condition is null) throw new ArgumentNullException(nameof(condition));
            if (condition.Invoke())
            {
                this.WithParam(name, value);
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="condition"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlBuilder WithParamIf(string name, Func<bool> condition, IEnumerable<object> values)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (condition is null) throw new ArgumentNullException(nameof(condition));
            if (condition.Invoke())
            {
                this.WithParam(name, values);
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string? query = _nameValueCollection.ToString();
            if (string.IsNullOrWhiteSpace(query)) return _url;

            // Giữ fragment ở cuối và nối tiếp query sẵn có trong url thay vì thêm '?' lần hai
            string url = _url;
            string fragment = string.Empty;
            int hashIndex = url.IndexOf('#');
            if (hashIndex >= 0)
            {
                fragment = url.Substring(hashIndex);
                url = url.Substring(0, hashIndex);
            }
            char separator = url.IndexOf('?') < 0 ? '?' : (url.EndsWith("?") || url.EndsWith("&") ? '\0' : '&');
            return separator == '\0' ? $"{url}{query}{fragment}" : $"{url}{separator}{query}{fragment}";
        }

        /// <summary>
        /// Đổi giá trị sang chuỗi theo InvariantCulture để server đọc được bất kể culture của máy client
        /// </summary>
        static bool IsParamList(object? value, out IEnumerable? enumerable)
        {
            // Chỉ trải mảng/list giá trị; string, byte[], dictionary, JToken... giữ nguyên là một giá trị
            if (value is ICollection collection && value is not byte[] && value is not IDictionary)
            {
                enumerable = collection;
                return true;
            }
            enumerable = null;
            return false;
        }

        static string? FormatValue(object? value)
        {
            switch (value)
            {
                case null: return null;
                case string s: return s;
                case bool b: return b ? "true" : "false";
                case DateTime dateTime: return dateTime.ToString("O", CultureInfo.InvariantCulture);
                case DateTimeOffset dateTimeOffset: return dateTimeOffset.ToString("O", CultureInfo.InvariantCulture);
                case IFormattable formattable: return formattable.ToString(null, CultureInfo.InvariantCulture);
                default: return value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static explicit operator Uri(UrlBuilder builder) => new Uri(builder.ToString(), UriKind.RelativeOrAbsolute);

    }
}
