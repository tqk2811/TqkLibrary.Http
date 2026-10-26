using System.Net;
using System.Net.Http.Headers;
using TqkLibrary.Http;
using TqkLibrary.Http.Extensions;
using TqkLibrary.Http.Streams;

namespace TestProject
{
    [TestClass]
    public class HttpResponseMessageExtensionsTest
    {
        [TestMethod]
        public void GetRetryAfter_Delta_ReturnsDelta()
        {
            using HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(34));

            Assert.AreEqual(TimeSpan.FromSeconds(34), response.GetRetryAfter());
        }

        [TestMethod]
        public void GetRetryAfter_FutureDate_ReturnsPositiveWait()
        {
            using HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(5));

            TimeSpan? wait = response.GetRetryAfter();
            Assert.IsNotNull(wait);
            Assert.IsTrue(wait > TimeSpan.FromMinutes(4));
        }

        [TestMethod]
        public void GetRetryAfter_PastDateOrMissing_ReturnsNull()
        {
            using HttpResponseMessage past = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            past.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(-5));
            using HttpResponseMessage missing = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

            Assert.IsNull(past.GetRetryAfter());
            Assert.IsNull(missing.GetRetryAfter());
        }
    }
}
