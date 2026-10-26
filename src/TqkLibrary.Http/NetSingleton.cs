using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using TqkLibrary.Http.HttpClientHandles;

namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public static class NetSingleton
    {
        static NetSingleton()
        {
            JsonSerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new MyContractResolver(),
#if DEBUG
                MissingMemberHandling = MissingMemberHandling.Error,
#endif
            };

            // UseCookies phải TẮT tường minh: HttpClientHandler mặc định bật nó cùng một
            // CookieContainer riêng, mà handler dưới đây là MỘT instance static dùng cho cả tiến
            // trình (xem BaseApi()) ⇒ bật cookie là mọi đối tượng BaseApi chia nhau CHUNG một hộp
            // cookie, bất kể bao nhiêu instance hay bao nhiêu người dùng. Trên server nhiều người
            // dùng, cookie phiên của người đăng nhập sau đè lên người trước và hai người bị tráo
            // danh tính cho nhau. Client nào cần cookie thì bọc CookieHandler — hộp cookie khi đó
            // thuộc riêng instance đó, đúng như BaseGoogleDocsHelper và DropboxApiNonLogin đang làm.
            if (IsBrowserRuntime())
            {
                HttpClientHandler = new WrapperHttpClientHandler()
                {
                    UseCookies = false,
                };
            }
            else
            {
                HttpClientHandler = new WrapperHttpClientHandler()
                {
                    UseCookies = false,
                    AllowAutoRedirect = true,
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                };
            }
        }

        internal static bool IsBrowserRuntime()
        {
#if NET5_0_OR_GREATER
            return OperatingSystem.IsBrowser();
#else
            return RuntimeInformation.ProcessArchitecture.ToString().Equals("Wasm", StringComparison.OrdinalIgnoreCase);
#endif

        }


        /// <summary>
        /// Handler dùng chung cho cả tiến trình, là thứ <see cref="BaseApi"/> lấy khi được dựng mà
        /// không truyền handler vào.
        /// </summary>
        /// <remarks>
        /// Vì đây là MỘT instance cho cả tiến trình, đừng đặt lên nó thứ gì thuộc về một người dùng
        /// hay một phiên: <c>UseCookies</c> (đã tắt sẵn — xem lý do ở static constructor),
        /// <c>CookieContainer</c>, <c>Credentials</c>, <c>ClientCertificates</c>. Mọi thứ đặt ở đây
        /// là dùng chung cho tất cả. Cần cookie riêng từng phiên thì bọc
        /// <see cref="HttpClientHandles.CookieHandler"/> quanh handler này rồi truyền
        /// <see cref="HttpClient"/> đó vào <see cref="BaseApi"/>.
        /// <para>
        /// Handler này KHÔNG dispose được (<see cref="HttpClientHandles.WrapperHttpClientHandler"/>
        /// vô hiệu hoá <c>Dispose</c> để bảo vệ singleton). Hệ quả: truyền nó vào
        /// <see cref="BaseApi"/> với <c>disposeHandler: true</c> là câu lệnh không có tác dụng, và
        /// không báo lỗi.
        /// </para>
        /// </remarks>
        public static WrapperHttpClientHandler HttpClientHandler { get; }
        internal static readonly JsonSerializerSettings JsonSerializerSettings;
    }
}