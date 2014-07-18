namespace CSharpPromise.Concurrent
{
    using System;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFuture<out T>
    {
        /// <summary>
        ///
        /// </summary>
        bool IsCompleted
        {
            get;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        void OnComplete(Action<ITry<T>> f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        void OnComplete(Action<ITry<T>> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        void Await();

        /// <summary>
        ///
        /// </summary>
        /// <param name="atMost"></param>
        void Await(int millisecondsTimeout);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        T Result();

        /// <summary>
        ///
        /// </summary>
        /// <param name="atMost"></param>
        /// <returns></returns>
        T Result(int millisecondsTimeout);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        IFuture<T> AndThen(Action<ITry<T>> f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<T> AndThen(Action<ITry<T>> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        IFuture<S> Collect<S>(Func<T, S> f);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<S> Collect<S>(Func<T, S> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IFuture<Exception> Failed();

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        IFuture<T> Filter(Func<T, bool> p);

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<T> Filter(Func<T, bool> p, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        IFuture<S> FlatMap<S>(Func<T, IFuture<S>> f);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<S> FlatMap<S>(Func<T, IFuture<S>> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        void Foreach(Action<T> f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        void Foreach(Action<T> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        IFuture<S> Map<S>(Func<T, S> f);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<S> Map<S>(Func<T, S> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        void OnFailure(Action<Exception> f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        void OnFailure(Action<Exception> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        void OnSuccess(Action<T> f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        void OnSuccess(Action<T> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        IFuture<S> Transform<S>(Func<T, S> s, Func<Exception, Exception> f);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        IFuture<S> Transform<S>(Func<T, S> s, Func<Exception, Exception> f, IExecutor executor);
    }
}