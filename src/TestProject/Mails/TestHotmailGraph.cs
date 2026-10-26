using Microsoft.Graph;
using TqkLibrary.Http.Api.Mail.OutlookGraphApi;
using TqkLibrary.Http.Api.Mail.OutlookGraphApi.Classes;

namespace TestProject.Mails
{
    [TestClass]
    public class TestHotmailGraph
    {
        public static IEnumerable<object[]> Datas
        {
            get
            {
                yield return new object[]
                {
                    //arellgtanck9347x3@outlook.com
                    "9e5f94bc-e8a4-4e73-b8be-63364c29d753",
                    "M.C557_BAY.0.U.-CrHov9ElTgAMRq2n1pSzIdMPwWF1f8zQ3g1SKvEfUMarmq7DY2BFi*GqJ1SI2QUAUtYzHG98Mzb96fvfNXMYCLmEoVnoasAW1WB64fY0TLBSVCjpsp!IkSzSsAM8R4Xik1elfVnh8jotmwKPKzhFfjMOOjJNQbToI5Dx7TUg58Sb06MzILZrSjLHQmWPiL!*et*nFI!7hChpSJ0mBj1TfYHS9dutfcWtI5rKJxN*BCVBkhCjs007o99e0Hr9CNgGOrhffwpQ0or0zUMxB04Co9aBLOKkPq2P5oX1tGh!PoZg6DklMTA!t1ryVXL6VYko79A1uMNZVED5WNIa*iL*056GWhhsD5qwDW2mE!9eNEeMmxjCWWGNevBocnTL4W4y8w$$",
                };
            }
        }
        [TestMethod(), DynamicData(nameof(Datas))]
        public async Task TestHotmailGraphApi(string clientId, string refreshToken)
        {
            using HttpclientAuthenticationProvider authenticationProvider = new HttpclientAuthenticationProvider(
                new AuthenticationRefreshToken()
                {
                    ClientId = clientId,
                    RefreshToken = refreshToken
                });
            using GraphServiceClient graphClient = new GraphServiceClient(authenticationProvider);
            var message = await graphClient.Me.Messages.GetAsync();
        }
    }
}
