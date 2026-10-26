using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace TqkLibrary.Http
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public class ContractResolverChain : DefaultContractResolver
    //{
    //    readonly DefaultContractResolver[] contractResolvers;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="contractResolvers"></param>
    //    /// <exception cref="ArgumentNullException"></exception>
    //    public ContractResolverChain(params DefaultContractResolver[] contractResolvers)
    //    {
    //        if (contractResolvers == null || contractResolvers.Length == 0) throw new ArgumentNullException(nameof(contractResolvers));
    //        this.contractResolvers = contractResolvers;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="member"></param>
    //    /// <param name="memberSerialization"></param>
    //    /// <returns></returns>
    //    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    //    {
    //        return base.CreateProperty(member, memberSerialization);
    //    }
    //}
    /// <summary>
    /// 
    /// </summary>
    public class MyContractResolver : CamelCasePropertyNamesContractResolver
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
                    return !string.IsNullOrEmpty(val);//!val.Equals("N/A", StringComparison.OrdinalIgnoreCase);
                };
            }
            return prop;
        }
    }
}