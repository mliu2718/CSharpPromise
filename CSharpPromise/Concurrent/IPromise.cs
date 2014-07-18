namespace CSharpPromise.Concurrent
{
    using System;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPromise<T>
    {
        /// <summary>
        ///
        /// </summary>
        IFuture<T> Future
        {
            get;
        }

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
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryComplete(ITry<T> result);

        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        IPromise<T> Complete(ITry<T> result);

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IPromise<T> CompleteWith(IFuture<T> other);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cause"></param>
        /// <returns></returns>
        IPromise<T> Failure(Exception cause);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IPromise<T> Success(T value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IPromise<T> TryCompleteWith(IFuture<T> other);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cause"></param>
        /// <returns></returns>
        bool TryFailure(Exception cause);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TrySuccess(T value);
    }
}