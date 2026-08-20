using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TqkLibrary.Http
{
    /// <summary>
    /// An error returned by an API, carrying enough request/response context that logs and the debugger
    /// do not leave you guessing.
    /// <para>
    /// <see cref="Message"/> is built from the properties when no message is passed to the constructor,
    /// so existing throw sites using object initializers get a full message without any change.
    /// </para>
    /// <para>
    /// Headers are NOT printed, to keep tokens and cookies out of log files; only <see cref="ContentType"/>
    /// and <see cref="ContentLength"/> are kept, because the body summary needs them.
    /// </para>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ApiException : Exception
    {
        /// <summary>
        /// Maximum body length printed inside <see cref="Message"/>, the line that reaches every log. Default 512.
        /// </summary>
        public static int MaxBodyLengthInMessage { get; set; } = 512;

        /// <summary>
        /// Maximum body length printed in the detail block of <see cref="ToString"/>. Default 8192.
        /// </summary>
        public static int MaxBodyLengthInDetail { get; set; } = 8192;

        /// <summary>
        /// How many leading characters of the body are scanned to detect binary content. Default 512.
        /// </summary>
        public static int BinarySniffLength { get; set; } = 512;

        readonly string? _explicitMessage;
        bool _isDataFilled;

        /// <summary>
        /// </summary>
        public ApiException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Custom message. When given, <see cref="Message"/> is no longer built automatically.</param>
        public ApiException(string message) : base(message)
        {
            _explicitMessage = message;
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Custom message. When given, <see cref="Message"/> is no longer built automatically.</param>
        /// <param name="innerException"></param>
        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
            _explicitMessage = message;
        }

        /// <summary>
        /// Builds the exception with the full context taken from the response. Prefer this over filling
        /// every property by hand.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="rawBody">The raw body that was read, if any.</param>
        /// <param name="innerException"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiException(HttpResponseMessage response, string? rawBody, Exception? innerException = null)
            : base(null, innerException)
        {
            if (response is null) throw new ArgumentNullException(nameof(response));

            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            RequestMethod = response.RequestMessage?.Method;
            RequestUri = response.RequestMessage?.RequestUri;
            RawBody = rawBody;
            // Read the content headers now: they are unusable once the response gets disposed.
            ContentType = response.Content?.Headers?.ContentType?.ToString();
            ContentLength = response.Content?.Headers?.ContentLength;
        }

        /// <summary>
        /// Status code of the response, null when the error did not come from a response.
        /// </summary>
        public HttpStatusCode? StatusCode { get; init; }

        /// <summary>
        /// Reason phrase of the status line, e.g. <c>Bad Gateway</c>.
        /// </summary>
        public string? ReasonPhrase { get; init; }

        /// <summary>
        /// Method of the request that was sent.
        /// </summary>
        public HttpMethod? RequestMethod { get; init; }

        /// <summary>
        /// Uri of the request that was sent, already combined with BaseAddress for a relative uri.
        /// </summary>
        public Uri? RequestUri { get; init; }

        /// <summary>
        /// The raw, undeserialized body. Always kept even when deserialization failed, since it is usually
        /// the only clue left.
        /// </summary>
        public string? RawBody { get; init; }

        /// <summary>
        /// Content-Type of the response, e.g. <c>text/html; charset=utf-8</c>.
        /// </summary>
        public string? ContentType { get; init; }

        /// <summary>
        /// Content-Length of the response, in bytes. For a binary body this is the only real size:
        /// <see cref="RawBody"/> is then just a mangled string produced by decoding bytes as text.
        /// </summary>
        public long? ContentLength { get; init; }

        /// <summary>
        /// Why this exception was thrown.
        /// </summary>
        public ApiErrorKind Kind { get; init; } = ApiErrorKind.HttpStatus;

        /// <summary>
        /// The deserialized body, readable without knowing the type argument, for shared logging code that
        /// catches <see cref="ApiException"/>. See <see cref="ApiException{T}.Body"/> for the typed value.
        /// </summary>
        public virtual object? BodyObject => null;

        /// <summary>
        /// Character count of <see cref="RawBody"/>.
        /// </summary>
        public int BodyLength => RawBody?.Length ?? 0;

        /// <summary>
        /// Whether the body is readable text. Failing either check means binary: a content type that is not
        /// textual, or content holding control characters (the content type lied).
        /// </summary>
        public bool IsBodyText => IsTextContentType(ContentType) && !LooksBinary(RawBody);

        /// <summary>
        /// One-line body summary, e.g. <c>text/html; charset=utf-8, 48213 chars</c>. A binary body is
        /// measured in bytes.
        /// </summary>
        public string BodySummary
        {
            get
            {
                string type = string.IsNullOrWhiteSpace(ContentType) ? "unknown" : ContentType!;
                return IsBodyText
                    ? $"{type}, {BodyLength} chars"
                    : $"{type}, {ContentLength ?? BodyLength} bytes";
            }
        }

        /// <summary>
        /// Human readable status, e.g. <c>502 Bad Gateway</c>.
        /// </summary>
        public string StatusText
        {
            get
            {
                if (!StatusCode.HasValue) return "(no status)";
                string reason = string.IsNullOrWhiteSpace(ReasonPhrase)
                    ? StatusCode.Value.ToString()
                    : ReasonPhrase!;
                return $"{(int)StatusCode.Value} {reason}";
            }
        }

        /// <summary>
        /// Message built from the context when the constructor received no message.
        /// </summary>
        public override string Message => _explicitMessage ?? BuildMessage();

        /// <summary>
        /// Also exposes the context through <see cref="Exception.Data"/> so structured log sinks pick it up.
        /// The body only goes in when it is printable, otherwise a sink would swallow a whole HTML page.
        /// </summary>
        public override IDictionary Data
        {
            get
            {
                IDictionary data = base.Data;
                if (!_isDataFilled)
                {
                    _isDataFilled = true;
                    FillData(data);
                }
                return data;
            }
        }

        /// <summary>
        /// Message and stack trace, followed by the HTTP detail block.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string detail = BuildHttpDetail();
            if (detail.Length == 0) return base.ToString();
            return base.ToString() + Environment.NewLine + detail;
        }

        /// <summary>
        /// Lets a derived class append its own section at the end of the detail block.
        /// </summary>
        /// <param name="stringBuilder"></param>
        protected virtual void AppendDetail(StringBuilder stringBuilder)
        {
        }

        /// <summary>
        /// Cuts <paramref name="text"/> down to <paramref name="maxLength"/>, with a suffix telling how much was cut.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        protected static string? Truncate(string? text, int maxLength)
        {
            if (text is null || maxLength < 0 || text.Length <= maxLength) return text;
            return text.Substring(0, maxLength) + $"... (+{text.Length - maxLength} chars)";
        }

        string BuildMessage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (RequestMethod is not null || RequestUri is not null)
            {
                if (RequestMethod is not null) stringBuilder.Append(RequestMethod.Method).Append(' ');
                if (RequestUri is not null) stringBuilder.Append(RequestUri.ToString()).Append(' ');
                stringBuilder.Append("-> ");
            }
            stringBuilder.Append(StatusText);
            if (Kind != ApiErrorKind.HttpStatus) stringBuilder.Append(" [").Append(Kind).Append(']');

            if (BodyLength > 0)
            {
                stringBuilder.Append(", body: ");
                if (IsBodyPrintable(MaxBodyLengthInMessage)) stringBuilder.Append(SingleLine(RawBody!));
                else stringBuilder.Append(BodySummary);
            }
            return stringBuilder.ToString();
        }

        string BuildHttpDetail()
        {
            if (StatusCode is null && RequestUri is null && BodyLength == 0) return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--- HTTP detail ---");
            if (RequestMethod is not null || RequestUri is not null)
                stringBuilder.Append("Request : ").Append(RequestMethod?.Method).Append(' ').AppendLine(RequestUri?.ToString());
            stringBuilder.Append("Response: ").AppendLine(StatusText);
            stringBuilder.Append("Kind    : ").AppendLine(Kind.ToString());

            if (BodyLength > 0)
            {
                stringBuilder.Append("Body    : ").Append(BodySummary);
                if (IsBodyPrintable(MaxBodyLengthInDetail))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(RawBody);
                }
                else
                {
                    stringBuilder.Append(" (").Append(BodyOmitReason(MaxBodyLengthInDetail)).AppendLine(")");
                }
            }

            AppendDetail(stringBuilder);
            return stringBuilder.ToString().TrimEnd();
        }

        bool IsBodyPrintable(int maxLength)
            => BodyLength > 0 && IsBodyText && BodyLength <= maxLength;

        string BodyOmitReason(int maxLength)
            => IsBodyText
                ? $"body omitted: {BodyLength} chars exceeds {maxLength}"
                : "body omitted: not text";

        void FillData(IDictionary data)
        {
            SetIfAbsent(data, nameof(StatusCode), StatusCode.HasValue ? (int)StatusCode.Value : (object?)null);
            SetIfAbsent(data, nameof(ReasonPhrase), ReasonPhrase);
            SetIfAbsent(data, nameof(RequestMethod), RequestMethod?.Method);
            SetIfAbsent(data, nameof(RequestUri), RequestUri?.ToString());
            SetIfAbsent(data, nameof(ContentType), ContentType);
            SetIfAbsent(data, nameof(ContentLength), ContentLength);
            SetIfAbsent(data, nameof(BodyLength), BodyLength);
            SetIfAbsent(data, nameof(Kind), Kind.ToString());
            if (IsBodyPrintable(MaxBodyLengthInDetail)) SetIfAbsent(data, nameof(RawBody), RawBody);
        }

        static void SetIfAbsent(IDictionary data, string key, object? value)
        {
            if (value is null || data.Contains(key)) return;
            data[key] = value;
        }

        string DebuggerDisplay
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder(StatusText);
                if (RequestMethod is not null) stringBuilder.Append(' ').Append(RequestMethod.Method);
                if (RequestUri is not null)
                    stringBuilder.Append(' ').Append(RequestUri.IsAbsoluteUri ? RequestUri.PathAndQuery : RequestUri.ToString());
                if (BodyLength > 0) stringBuilder.Append(" (").Append(BodySummary).Append(')');
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Squeezes the body onto one line for <see cref="Message"/>: line breaks and repeated whitespace
        /// collapse, otherwise pretty-printed JSON would tear the message across dozens of log lines.
        /// </summary>
        static string SingleLine(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool lastIsSpace = false;
            foreach (char c in text)
            {
                bool isSpace = c == ' ' || c == '\t' || c == '\r' || c == '\n';
                if (isSpace)
                {
                    if (!lastIsSpace && stringBuilder.Length > 0) stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append(c);
                }
                lastIsSpace = isSpace;
            }
            return stringBuilder.ToString().TrimEnd();
        }

        /// <summary>
        /// Whether the content type describes readable text. A missing content type counts as text.
        /// </summary>
        static bool IsTextContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return true;

            string mediaType = contentType!.Split(';')[0].Trim().ToLowerInvariant();
            if (mediaType.Length == 0) return true;
            if (mediaType.StartsWith("text/", StringComparison.Ordinal)) return true;
            if (mediaType.EndsWith("/json", StringComparison.Ordinal)) return true;
            if (mediaType.EndsWith("+json", StringComparison.Ordinal)) return true;
            if (mediaType.EndsWith("/xml", StringComparison.Ordinal)) return true;
            if (mediaType.EndsWith("+xml", StringComparison.Ordinal)) return true;
            if (mediaType.Equals("application/x-www-form-urlencoded", StringComparison.Ordinal)) return true;
            if (mediaType.Equals("application/javascript", StringComparison.Ordinal)) return true;
            return false;
        }

        /// <summary>
        /// Detects binary from the content instead of trusting the content type: an API serving a file as
        /// <c>text/plain</c>, or JSON as <c>application/octet-stream</c>, are both common.
        /// </summary>
        static bool LooksBinary(string? text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            int length = Math.Min(text!.Length, BinarySniffLength);
            for (int i = 0; i < length; i++)
            {
                char c = text[i];
                if (c == '\t' || c == '\r' || c == '\n') continue;
                // U+FFFD is the replacement character produced when UTF-8 decoding fails: a sure sign of
                // binary bytes read as a string.
                if (c == (char)0xFFFD) return true;
                if (char.IsControl(c)) return true;
            }
            return false;
        }
    }
}
