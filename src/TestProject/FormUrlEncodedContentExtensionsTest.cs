using Newtonsoft.Json;
using TqkLibrary.Http.Extensions;

namespace TestProject
{
    [TestClass]
    public class FormUrlEncodedContentExtensionsTest
    {
        class Form
        {
            [JsonProperty("user_name")]
            public string? UserName { get; set; }
            public int Age { get; set; }
            public string? Note { get; set; }
        }

        [TestMethod]
        public async Task ToFormUrlEncodedContent_UsesJsonPropertyName()
        {
            string body = await new Form { UserName = "a", Note = "n" }.ToFormUrlEncodedContent().ReadAsStringAsync();
            Assert.AreEqual("user_name=a&Age=0&Note=n", body);
        }

        [TestMethod]
        public async Task ToFormUrlEncodedContent_SkipsNullProperty()
        {
            string body = await new Form { UserName = "a" }.ToFormUrlEncodedContent().ReadAsStringAsync();
            Assert.AreEqual("user_name=a&Age=0", body);
        }
    }
}
