using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace TqkLibrary.Http
{
    /// <summary>
    /// An <see cref="ApiException"/> carrying the error body deserialized into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the error body returned by the API.</typeparam>
    public class ApiException<T> : ApiException
    {
        /// <summary>
        /// </summary>
        public ApiException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Custom message. When given, <see cref="ApiException.Message"/> is no longer built automatically.</param>
        public ApiException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Custom message. When given, <see cref="ApiException.Message"/> is no longer built automatically.</param>
        /// <param name="innerException"></param>
        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Builds the exception with the full context taken from the response. Prefer this over filling
        /// every property by hand.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="rawBody">The raw body that was read, if any.</param>
        /// <param name="body">The deserialized body, null when deserialization failed.</param>
        /// <param name="innerException">Usually the deserialization error, kept so the cause stays traceable.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiException(HttpResponseMessage response, string? rawBody, T? body = default, Exception? innerException = null)
            : base(response, rawBody, innerException)
        {
            Body = body;
        }

        /// <summary>
        /// The deserialized error body. Null when the body could not be parsed, in which case
        /// <see cref="ApiException.RawBody"/> is what is left to look at.
        /// </summary>
        public T? Body { get; init; }

        /// <summary>
        /// </summary>
        public override object? BodyObject => Body;

        /// <summary>
        /// Appends the deserialized body, unless it is merely a copy of <see cref="ApiException.RawBody"/>.
        /// </summary>
        /// <param name="stringBuilder"></param>
        protected override void AppendDetail(StringBuilder stringBuilder)
        {
            if (Body is null) return;
            // When T is string, Body is RawBody: printing it twice only makes the log longer.
            if (Body is string text && string.Equals(text, RawBody, StringComparison.Ordinal)) return;

            string formatted = Format(Body);
            stringBuilder.Append("Body<").Append(typeof(T).Name).Append(">: ");
            if (formatted.Length <= MaxBodyLengthInDetail)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(formatted);
            }
            else
            {
                stringBuilder.Append(formatted.Length).Append(" chars (body omitted: exceeds ").Append(MaxBodyLengthInDetail).AppendLine(")");
            }
        }

        static string Format(T body)
        {
            try
            {
                return JsonConvert.SerializeObject(body, Formatting.Indented);
            }
            catch
            {
                // A custom type with a broken converter must still print something: never throw from ToString.
                return body?.ToString() ?? string.Empty;
            }
        }
    }
}
