using System.Diagnostics.CodeAnalysis;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Classes
{
    public class CaptchaTaskTextResult
    {
        public required bool IsSuccess { get; set; }
        public string? Value { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
