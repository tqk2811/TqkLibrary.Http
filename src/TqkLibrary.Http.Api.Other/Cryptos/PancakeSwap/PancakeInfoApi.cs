using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Other.Cryptos.PancakeSwap.DataClass;

namespace TqkLibrary.Http.Api.Other.Cryptos.PancakeSwap
{
    /// <summary>
    /// 
    /// </summary>
    public class PancakeInfoApi : BaseApi
    {
        const string EndPoint = "https://api.pancakeswap.info/api/v2";
        /// <summary>
        /// 
        /// </summary>
        public PancakeInfoApi() : base()
        {

        }
        /// <summary>
        /// Returns the token information, based on address.
        /// </summary>
        /// <returns></returns>
        public Task<PancakeSwapBaseResponse<PancakeSwapTokenData>> Token(string address, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "tokens", address))
            .ExecuteAsync<PancakeSwapBaseResponse<PancakeSwapTokenData>>(cancellationToken);

        /// <summary>
        /// Returns the tokens in the top ~1000 pairs on PancakeSwap, sorted by reserves.
        /// </summary>
        /// <returns></returns>
        public Task<PancakeSwapBaseResponse<Dictionary<string, PancakeSwapTokenData>>> Tokens(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "tokens"))
            .ExecuteAsync<PancakeSwapBaseResponse<Dictionary<string, PancakeSwapTokenData>>>(cancellationToken);

        /// <summary>
        /// Returns data for the top ~1000 PancakeSwap pairs, sorted by reserves.
        /// </summary>
        /// <returns></returns>
        public Task<PancakeSwapBaseResponse<Dictionary<string, PancakeSwapTokenPairsData>>> PancakeSwapPairs(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "pairs"))
            .ExecuteAsync<PancakeSwapBaseResponse<Dictionary<string, PancakeSwapTokenPairsData>>>(cancellationToken);
    }
}
