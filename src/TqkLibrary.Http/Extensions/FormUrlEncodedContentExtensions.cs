using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace TqkLibrary.Http.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class FormUrlEncodedContentExtensions
    {
        /// <summary>
        /// Builds a form from the public properties of <paramref name="obj"/>. The key is the name in
        /// <see cref="JsonPropertyAttribute"/> when given, otherwise the property name; null properties are skipped.
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
                object? value = p.GetValue(obj);
                if (value is null) continue;

                string key = p.Name;

                var prop = p.GetCustomAttributes(false).OfType<JsonPropertyAttribute>().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(prop?.PropertyName)) key = prop!.PropertyName!;

                keyValuePairs[key] = value.ToString();
            }

            return new FormUrlEncodedContent(keyValuePairs);
        }
    }
}
