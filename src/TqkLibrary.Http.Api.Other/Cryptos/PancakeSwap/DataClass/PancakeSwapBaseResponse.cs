namespace TqkLibrary.Http.Api.Other.Cryptos.PancakeSwap.DataClass
{
    public class PancakeSwapBaseResponse<T>
    {
        public long updated_at { get; set; }
        public T data { get; set; }
    }
}
