namespace CSharpPromise.Net.Http
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    /// <summary>
    ///
    /// </summary>
    public sealed class HttpResponse
    {
        /// <summary>
        ///
        /// </summary>
        private readonly long contentLength;

        /// <summary>
        ///
        /// </summary>
        private readonly string contentType;

        /// <summary>
        ///
        /// </summary>
        private readonly WebHeaderCollection headers;

        /// <summary>
        ///
        /// </summary>
        private readonly string method;

        /// <summary>
        ///
        /// </summary>
        private readonly Uri responseUri;

        /// <summary>
        ///
        /// </summary>
        private readonly HttpStatusCode statusCode;

        /// <summary>
        ///
        /// </summary>
        private readonly string statusDescription;

        /// <summary>
        ///
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        ///
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <param name="method"></param>
        /// <param name="responseUri"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusDescription"></param>
        /// <param name="buffer"></param>
        internal HttpResponse(long contentLength, string contentType, WebHeaderCollection headers, string method, Uri responseUri, HttpStatusCode statusCode, string statusDescription, byte[] buffer)
        {
            Debug.Assert(buffer != null && contentLength == buffer.Length);

            this.contentLength = contentLength;
            this.contentType = contentType;
            this.headers = headers;
            this.method = method;
            this.responseUri = responseUri;
            this.statusCode = statusCode;
            this.statusDescription = statusDescription;
            this.buffer = buffer;
        }

        /// <summary>
        ///
        /// </summary>
        public long ContentLength
        {
            get
            {
                return this.contentLength;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string Method
        {
            get
            {
                return this.method;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public WebHeaderCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Uri ResponseUri
        {
            get
            {
                return this.responseUri;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return this.statusCode;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return this.statusDescription;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Stream GetResponseStream()
        {
            return new MemoryStream(this.buffer, false);
        }
    }
}