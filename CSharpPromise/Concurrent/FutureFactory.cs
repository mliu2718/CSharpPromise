namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    public static class FutureFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFuture<T> Create<T>(Func<T> f)
        {
            Debug.Assert(f != null);

            return FutureFactory.Create<T>(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public static IFuture<T> Create<T>(Func<T> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<T>();

            executor.Execute(() => { p.Complete(TryFactory.Create(f)); }); // TODO: IExecutor.Execute MUST NOT throw

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IFuture<T> Failed<T>(Exception exception)
        {
            Debug.Assert(exception != null);

            return PromiseFactory.Failed<T>(exception).Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IFuture<T> FromTry<T>(ITry<T> result)
        {
            return PromiseFactory.FromTry(result).Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IFuture<T> Successful<T>(T result)
        {
            return PromiseFactory.Successful(result).Future;
        }
    }
}