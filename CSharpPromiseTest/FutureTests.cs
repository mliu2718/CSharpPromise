namespace CSharpPromiseTest
{
    using System;
    using System.Threading;
    using CSharpPromise.Utils;
    using CSharpPromise.Concurrent;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FutureTests : SilverlightTest
    {
        [TestMethod]
        [Asynchronous]
        public void TestFuture()
        {
            var f = FutureFactory.Create(() => "Hello, Future!");
            f.OnSuccess((v) =>
                {
                    Assert.AreEqual("Hello, Future!", v);
                    EnqueueTestComplete();
                }
            );
        }

        [TestMethod]
        [Asynchronous]
        public void TestFuture2()
        {
            var a = FutureFactory.Create(() => { Thread.Sleep(1000); return 1; });
            var b = FutureFactory.Create(() => { Thread.Sleep(2000); return 2; });

            var c = a.FlatMap((x) => b.Map((y) => x + y));
            c.OnSuccess((v) =>
                {
                    Assert.AreEqual(3, v);
                    EnqueueTestComplete();
                }
            );
        }

        [TestMethod]
        [Asynchronous]
        public void TestFuture3()
        {
            var a = FutureFactory.Create(() => { Thread.Sleep(1000); return 1; });
            var b = FutureFactory.Create<int>(() => { Thread.Sleep(2000); throw new Exception(); });

            var c = a.FlatMap((x) => b.Map((y) => x + y));
            c.OnSuccess((v) =>
            {
                Assert.AreEqual(3, v);
                EnqueueTestComplete();
            });

            c.OnFailure((e) =>
                {
                    EnqueueTestComplete();
                });
        }
    }
}