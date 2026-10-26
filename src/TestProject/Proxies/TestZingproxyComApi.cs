
using TqkLibrary.Http;
using TqkLibrary.Http.Api.Proxy.Services;

namespace TestProject.Proxies
{
    [TestClass]
    public class TestZingproxyComApi
    {
        public static IEnumerable<object[]> Datas
        {
            get
            {
                yield return new object[]
                {
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2ODY2MzM2MTJjMTU5NWNmNGQzNjUyNWEiLCJsYXN0UGFzc3dvcmRDaGFuZ2UiOiIyMDI1LTA3LTAzVDA3OjM4OjEwLjAwNFoiLCJpYXQiOjE3NTE1Mzg5OTQsImV4cCI6MTc1OTMxNDk5NH0.Z9ikJeZTFiDf9aNKaKw0zAkb-YmknrJ8kLFwdcvH5ZM",
                };
            }
        }
        [TestMethod(), DynamicData(nameof(Datas))]
        public async Task Test(string accessToken)
        {
            using SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
            socketsHttpHandler.DisableFindIpV6();
            using ZingproxyComApi zingproxyComApi = new(accessToken, socketsHttpHandler, false);
            var list = await zingproxyComApi.DanCuVietNam.ListAsync(ZingproxyComApi.RunningType.Running);
            var changed = await zingproxyComApi.DanCuVietNam.GetIpAsync(list.Proxies!.First());
            var proxy = await zingproxyComApi.GetProxy(list.Proxies!.First()!);
        }
    }
}
