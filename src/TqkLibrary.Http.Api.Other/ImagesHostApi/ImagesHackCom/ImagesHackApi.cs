using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Other.ImagesHostApi.ImagesHackCom.DataClass;

namespace TqkLibrary.Http.Api.Other.ImagesHostApi.ImagesHackCom
{
    //https://docs.google.com/document/d/16M3qaw27vgwuwXqExo0aIC0nni42OOuWu_OGvpYl7dE/pub#h.jcrh03smytne
    /// <summary>
    /// 
    /// </summary>
    public class ImagesHackApi : BaseApi
    {
        private const string EndPoint = "https://api.imageshack.com/v2/images";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public ImagesHackApi(string ApiKey) : base(ApiKey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<ImagesHackResponse<ImagesHackUploadResult>> UploadImage(byte[] bitmapBuffer, CancellationToken cancellationToken = default)
        {
            MultipartFormDataContent requestContent = new MultipartFormDataContent();
            ByteArrayContent imageContent_instructions = new ByteArrayContent(bitmapBuffer);
            imageContent_instructions.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            requestContent.Add(imageContent_instructions, "file", "file.jpeg");

            return Build()
                .WithUrlPost(new UrlBuilder(EndPoint).WithParam("api_key", ApiKey), requestContent)
                .ExecuteAsync<ImagesHackResponse<ImagesHackUploadResult>>(cancellationToken);
        }
    }
}