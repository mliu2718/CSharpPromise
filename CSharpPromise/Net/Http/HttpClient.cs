namespace CSharpPromise.Net.Http
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;
    using CSharpPromise.Concurrent;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly Exception cancelledException = new Exception("Cancelled");

        /// <summary>
        ///
        /// </summary>
        private readonly HttpRequest request;

        /// <summary>
        ///
        /// </summary>
        private readonly IPromise<HttpResponse> promise = PromiseFactory.Create<HttpResponse>();

        /// <summary>
        ///
        /// </summary>
        private IFuture<HttpResponse> futureHttpResponse = null;

        /// <summary>
        ///
        /// </summary>
        private HttpWebRequest webRequest = null;

        /// <summary>
        ///
        /// </summary>
        private HttpWebResponse webResponse = null;

        /// <summary>
        ///
        /// </summary>
        private Stream responseStream = null;

        /// <summary>
        ///
        /// </summary>
        private int bytesReceived = 0;

        /// <summary>
        ///
        /// </summary>
        private int contentLength = int.MaxValue;

        /// <summary>
        ///
        /// </summary>
        private string contentType;

        /// <summary>
        ///
        /// </summary>
        private WebHeaderCollection headers;

        /// <summary>
        ///
        /// </summary>
        private string method;

        /// <summary>
        ///
        /// </summary>
        private Uri responseUri;

        /// <summary>
        ///
        /// </summary>
        private HttpStatusCode statusCode;

        /// <summary>
        ///
        /// </summary>
        private string statusDescription;

        /// <summary>
        ///
        /// </summary>
        private byte[] buffer;

        /// <summary>
        ///
        /// </summary>
        private Action<HttpClient> onProgress = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        public HttpClient(HttpRequest request)
        {
            this.request = request;
        }

        /// <summary>
        ///
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return this.request;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.futureHttpResponse != null && !this.futureHttpResponse.IsCompleted;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return this.futureHttpResponse != null && this.futureHttpResponse.IsCompleted;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int BytesReceived
        {
            get
            {
                return this.bytesReceived;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int TotalBytesToReceive
        {
            get
            {
                return this.contentLength;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnProgress(Action<HttpClient> f)
        {
            if (!this.IsCompleted)
            {
                this.onProgress += f;

                if (this.IsCompleted)
                {
                    this.onProgress = null;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnSuccess(Action<HttpClient, HttpResponse> f)
        {
            this.promise.Future.OnSuccess((resp) => { f(this, resp); });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnFailure(Action<HttpClient, Exception> f)
        {
            this.promise.Future.OnFailure((ex) => { if (ex != HttpClient.cancelledException) { f(this, ex); } });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnCancel(Action<HttpClient> f)
        {
            this.promise.Future.OnFailure((ex) => { if (ex == HttpClient.cancelledException) { f(this); } });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IFuture<HttpResponse> GetResponse()
        {
            if (!this.IsBusy && !this.IsCompleted && Interlocked.CompareExchange(ref this.futureHttpResponse, this.promise.Future, null) == null)
            {
                try
                {
                    this.webRequest = (HttpWebRequest)WebRequest.Create(this.Request.RequestUri);
                    this.webRequest.AllowReadStreamBuffering = false;
                    this.webRequest.BeginGetResponse(new AsyncCallback(this.BeginGetResposneCallback), null);
                }
                catch (Exception ex)
                {
                    this.promise.TryFailure(ex);
                }
                finally
                {
                    if (this.IsCompleted)
                    {
                        this.Close();
                    }
                }
            }

            return this.futureHttpResponse;
        }

        /// <summary>
        ///
        /// </summary>
        public void Cancel()
        {
            if (this.IsBusy)
            {
                this.promise.TryFailure(HttpClient.cancelledException);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ar"></param>
        private void BeginGetResposneCallback(IAsyncResult ar)
        {
            Debug.Assert(ar != null);

            try
            {
                if (!this.IsCompleted)
                {
                    this.webResponse = (HttpWebResponse)this.webRequest.EndGetResponse(ar);
                    this.contentLength = Convert.ToInt32(this.webResponse.ContentLength);

                    this.contentType = TryFactory.Create(() => this.webResponse.ContentType).Recover((ignored) => string.Empty).Get();
                    this.headers = TryFactory.Create(() => this.webResponse.Headers).Recover((ignored) => null).Get();
                    this.method = this.webResponse.Method;
                    this.responseUri = TryFactory.Create(() => this.webResponse.ResponseUri).Recover((ignored) => null).Get();
                    this.statusCode = this.webResponse.StatusCode;
                    this.statusDescription = this.webResponse.StatusDescription;

                    this.buffer = new byte[this.contentLength];
                    this.responseStream = this.webResponse.GetResponseStream();

                    Action<HttpClient> onProgress = this.onProgress;
                    if (onProgress != null)
                    {
                        onProgress(this);
                    }

                    this.responseStream.BeginRead(this.buffer, this.bytesReceived, this.contentLength - this.bytesReceived, new AsyncCallback(this.BeginReadCallback), null);
                }
            }
            catch (Exception ex)
            {
                this.promise.TryFailure(ex);
            }
            finally
            {
                if (this.IsCompleted)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ar"></param>
        private void BeginReadCallback(IAsyncResult ar)
        {
            Debug.Assert(ar != null);

            try
            {
                if (!this.IsCompleted)
                {
                    int count = this.responseStream.EndRead(ar);
                    if (count > 0 && Interlocked.Add(ref this.bytesReceived, count) < this.contentLength)
                    {
                        this.responseStream.BeginRead(this.buffer, this.bytesReceived, this.contentLength - this.bytesReceived, new AsyncCallback(this.BeginReadCallback), null);

                        Action<HttpClient> onProgress = this.onProgress;
                        if (onProgress != null)
                        {
                            onProgress(this);
                        }
                    }
                    else
                    {
                        if (this.bytesReceived == this.contentLength)
                        {
                            this.promise.TrySuccess(new HttpResponse(this.contentLength, this.contentType, this.headers, this.method, this.responseUri, this.statusCode, this.statusDescription, this.buffer));
                        }
                        else
                        {
                            this.promise.TryFailure(new IndexOutOfRangeException("Invalid content length"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.promise.TryFailure(ex);
            }
            finally
            {
                if (this.IsCompleted)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void Close()
        {
            Debug.Assert(this.IsCompleted);

            try
            {
                if (this.responseStream != null)
                {
                    this.responseStream.Close();
                }
            }
            finally
            {
                this.responseStream = null;
            }

            try
            {
                if (this.webResponse != null)
                {
                    this.webResponse.Close();
                }
            }
            finally
            {
                this.webResponse = null;
            }

            DirectExecutor.Instance.Execute(() => {
                try
                {
                    if (this.webRequest != null)
                    {
                        this.webRequest.Abort();
                    }
                }
                finally
                {
                    this.webRequest = null;
                }
            });

            this.onProgress = null;
        }
    }
}