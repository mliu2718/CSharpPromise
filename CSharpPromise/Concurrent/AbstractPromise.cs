namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractPromise<T> : IPromise<T>
    {
        /// <summary>
        ///
        /// </summary>
        public abstract IFuture<T> Future
        {
            get;
        }

        /// <summary>
        ///
        /// </summary>
        public abstract bool IsCompleted
        {
            get;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public abstract bool TryComplete(ITry<T> result);

        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public IPromise<T> Complete(ITry<T> result)
        {
            Debug.Assert(result != null);

            if (this.TryComplete(result))
            {
                return this;
            }
                
            throw new InvalidOperationException("Promise already completed");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IPromise<T> CompleteWith(IFuture<T> other)
        {
            Debug.Assert(other != null);

            other.OnComplete((t) => { this.Complete(t); });

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cause"></param>
        /// <returns></returns>
        public IPromise<T> Failure(Exception cause)
        {
            Debug.Assert(cause != null);

            return this.Complete(new Failure<T>(cause));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IPromise<T> Success(T value)
        {
            return this.Complete(new Success<T>(value));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IPromise<T> TryCompleteWith(IFuture<T> other)
        {
            Debug.Assert(other != null);

            other.OnComplete((t) => { this.TryComplete(t); });

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cause"></param>
        /// <returns></returns>
        public bool TryFailure(Exception cause)
        {
            Debug.Assert(cause != null);

            return this.TryComplete(new Failure<T>(cause));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySuccess(T value)
        {
            return this.TryComplete(new Success<T>(value));
        }
    }
}