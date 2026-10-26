using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Other.Others
{
    /// <summary>
    /// 
    /// </summary>
    public class GoogleTrans : BaseApi
    {
        /// <summary>
        /// 
        /// </summary>
        public GoogleTrans() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> Translate(string text, TransLanguage from, TransLanguage to, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder("https://translate.googleapis.com/translate_a/single")
                .WithParam("client", "gtx")
                .WithParam("sl", from)
                .WithParam("tl", to)
                .WithParam("dt", "t")
                .WithParam("q", $"\"{text}\""))
            .ExecuteAsync<string>(cancellationToken);
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum TransLanguage
    {
        auto,
        en,
        vi,

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
