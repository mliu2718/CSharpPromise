namespace CSharpPromiseTest
{
    using CSharpPromise.Utils;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TryTests : SilverlightTest
    {
        [TestMethod]
        public void TestTry()
        {
            var dividend = TryFactory.Create(() => int.Parse("10"));
            var divisor = TryFactory.Create(() => int.Parse("2"));
            var problem = dividend.FlatMap((x) => divisor.Map((y) => x / y));
            Assert.AreEqual(5, problem.Get());
        }

        [TestMethod]
        public void TestTryFactory()
        {
            var dividend = TryFactory.Create(() => int.Parse("abc"));
            var divisor = TryFactory.Create(() => int.Parse("2"));
            Assert.IsTrue(dividend.IsFailure);
            Assert.IsTrue(divisor.IsSuccess);
        }

        [TestMethod]
        public void TestRecover()
        {
            var dividend = TryFactory.Create(() => int.Parse("abc"));
            var divisor = TryFactory.Create(() => int.Parse("2"));
            var problem = dividend.FlatMap((x) => divisor.Map((y) => x / y)).Recover((e) => 5);
            Assert.AreEqual(5, problem.Get()); ;
        }

        [TestMethod]
        public void TestRecoverWith()
        {
            var dividend = TryFactory.Create(() => int.Parse("abc")).RecoverWith((e) => TryFactory.Create(() => int.Parse("10")));
            var divisor = TryFactory.Create(() => int.Parse("2"));
            var problem = dividend.FlatMap((x) => divisor.Map((y) => x / y));
            Assert.AreEqual(5, problem.Get()); ;
        }
    }
}