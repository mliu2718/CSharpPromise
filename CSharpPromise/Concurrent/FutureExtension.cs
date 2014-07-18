namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    public static class FutureExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="future"></param>
        /// <param name="that"></param>
        /// <returns></returns>
        public static IFuture<T> FallbackTo<T>(this IFuture<T> future, IFuture<T> that)
        {
            Debug.Assert(that != null);

            var p = PromiseFactory.Create<T>();

            future.OnSuccess((v) => { p.Success(v); });

            future.OnFailure((e) =>
                {
                    that.OnSuccess((v) => { p.Success(v); });
                    that.OnFailure((ignored) => { p.Failure(e); });
                });

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="future"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFuture<T> Recover<T>(this IFuture<T> future, Func<Exception, T> f)
        {
            Debug.Assert(f != null);

            return future.Recover(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="future"></param>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public static IFuture<T> Recover<T>(this IFuture<T> future, Func<Exception, T> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<T>();

            future.OnComplete((t) => { p.Complete(t.Recover(f)); }, executor);

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="future"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFuture<T> RecoverWith<T>(this IFuture<T> future, Func<Exception, IFuture<T>> f)
        {
            Debug.Assert(f != null);

            return future.RecoverWith(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="future"></param>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public static IFuture<T> RecoverWith<T>(this IFuture<T> future, Func<Exception, IFuture<T>> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<T>();

            future.OnSuccess((v) => { p.Success(v); });

            future.OnFailure((ex) =>
                {
                    try
                    {
                        IFuture<T> other = f(ex);
                        other.OnSuccess((v) => { p.Success(v); });
                        other.OnFailure((ignored) => { p.Failure(ex); });
                    }
                    catch (Exception)
                    {
                        p.Failure(ex);
                    }
                }, executor);

            return p.Future;
        }
    }
}