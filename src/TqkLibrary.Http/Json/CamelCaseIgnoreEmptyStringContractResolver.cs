using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace TqkLibrary.Http.Json
{
    /// <summary>
    /// camelCase property names and skips <see cref="string"/> properties that are null or empty when serializing.
    /// </summary>
    public class CamelCaseIgnoreEmptyStringContractResolver : CamelCasePropertyNamesContractResolver
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.PropertyType == typeof(string) && prop.Readable)
            {
                prop.ShouldSerialize = obj =>
                {
                    string val = (string)prop.ValueProvider.GetValue(obj);
                    return !string.IsNullOrEmpty(val);
                };
            }
            return prop;
        }
    }
}
