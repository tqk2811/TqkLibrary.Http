namespace TqkLibrary.Http
{
    /// <summary>
    /// Why an <see cref="ApiException"/> was thrown.
    /// </summary>
    public enum ApiErrorKind
    {
        /// <summary>
        /// The server answered with a failure status code.
        /// </summary>
        HttpStatus = 0,
        /// <summary>
        /// The status code was successful but the body could not be deserialized into the expected result type.
        /// Without this, the log shows <c>StatusCode = 200</c> next to a thrown exception, which reads as nonsense.
        /// </summary>
        DeserializeFailed = 1,
    }
}
