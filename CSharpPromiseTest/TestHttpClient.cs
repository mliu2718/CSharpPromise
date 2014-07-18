namespace CSharpPromiseTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using CSharpPromise.Net.Http;
    using CSharpPromise.Utils;
    using CSharpPromise.Concurrent;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestHttpClient : SilverlightTest
    {
        [TestMethod]
        [Asynchronous]
        public void TestDownloadComplete()
        {
            HttpClient client = new HttpClient(new HttpRequest(new Uri("http://pc-nowtv.vod.sky.com/skyplayer/SMPCMOVIES/375b1e0262f86410VgnVCM1000000b43150a____/MV_WOLVERINE_ARD.ism/QualityLevels(1700000)/Fragments(video=140000000)")));
            client.OnProgress((sender) =>
            {
                Debug.WriteLine("{0}, {1}, PROGRESS, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
            });

            client.OnSuccess((sender, resp) =>
            {
                Debug.WriteLine("{0}, {1}, SUCCESS, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });

            client.OnCancel((sender) =>
            {
                Debug.WriteLine("{0}, {1}, CANCELLED, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });

            client.OnFailure((sender, ex) =>
            {
                Debug.WriteLine("{0}, {1}, FAILURE, {2}/{3}, {4}, {5}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });


            Debug.WriteLine("{0}, {1}, START, {2}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, client.Request.RequestUri.AbsolutePath);
            var futureHttpResponse = client.GetResponse();
        }

        [TestMethod]
        [Asynchronous]
        public void TestDownloadCancelled()
        {
            HttpClient client = new HttpClient(new HttpRequest(new Uri("http://pc-nowtv.vod.sky.com/skyplayer/SMPCMOVIES/375b1e0262f86410VgnVCM1000000b43150a____/MV_WOLVERINE_ARD.ism/QualityLevels(1700000)/Fragments(video=200000000)")));
            client.OnProgress((sender) =>
            {
                Debug.WriteLine("{0}, {1}, PROGRESS, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
            });

            client.OnProgress((sender) => { if (sender.BytesReceived * 100.0 / sender.TotalBytesToReceive > 25.0) { sender.Cancel(); } });

            client.OnSuccess((sender, resp) =>
            {
                Debug.WriteLine("{0}, {1}, SUCCESS, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });

            client.OnCancel((sender) =>
            {
                Debug.WriteLine("{0}, {1}, CANCELLED, {2}/{3}, {4}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });

            client.OnFailure((sender, ex) =>
            {
                Debug.WriteLine("{0}, {1}, FAILURE, {2}/{3}, {4}, {5}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, sender.BytesReceived, sender.TotalBytesToReceive, sender.Request.RequestUri.AbsolutePath);
                EnqueueTestComplete();
            });


            Debug.WriteLine("{0}, {1}, START, {2}", Thread.CurrentThread.ManagedThreadId, DateTime.UtcNow.Ticks, client.Request.RequestUri.AbsolutePath);
            var futureHttpResponse = client.GetResponse();
        }
    }
}
