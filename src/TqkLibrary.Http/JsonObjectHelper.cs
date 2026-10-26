using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonObjectHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static FormUrlEncodedContent ToFormUrlEncodedContent(this object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            Type type = obj.GetType();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var p in type.GetProperties())
            {
                string key = p.Name;

                var prop = p.GetCustomAttributes(false).OfType<JsonProperty>().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(prop?.PropertyName)) key = prop.PropertyName;

                keyValuePairs[key] = p.GetValue(obj).ToString();
            }

            return new FormUrlEncodedContent(keyValuePairs);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentNullException"></exception>
        //public static MultipartFormDataContent ToMultipartFormDataContent(this object obj)
        //{
        //    if (obj is null) throw new ArgumentNullException(nameof(obj));
        //    Type type = obj.GetType();

        //    MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
        //    return multipartFormDataContent;
        //}
    }
}
