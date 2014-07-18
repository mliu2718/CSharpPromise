namespace CSharpPromise.Net.Http
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        ///
        /// </summary>
        private readonly Uri requestUri;

        /// <summary>
        ///
        /// </summary>
        /// <param name="requestUri"></param>
        public HttpRequest(Uri requestUri)
        {
            this.requestUri = requestUri;
        }

        /// <summary>
        ///
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                return this.requestUri;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.RequestUri.OriginalString;
        }
    }
}