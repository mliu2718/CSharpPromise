namespace CSharpPromise.Utils
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Failure<T> : ITry<T>, IEquatable<ITry<T>>
    {
        /// <summary>
        ///
        /// </summary>
        private readonly Exception exception;

        /// <summary>
        ///
        /// </summary>
        /// <param name="exception"></param>
        public Failure(Exception exception)
        {
            Debug.Assert(exception != null);

            this.exception = exception;
        }

        /// <summary>
        ///
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFailure
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            throw this.Exception;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void Foreach(Action<T> f)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public ITry<U> FlatMap<U>(Func<T, ITry<U>> f)
        {
            return new Failure<U>(this.Exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public ITry<U> Map<U>(Func<T, U> f)
        {
            return new Failure<U>(this.Exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ITry<T> Filter(Func<T, bool> p)
        {
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ITry<Exception> Failed()
        {
            return new Success<Exception>(this.Exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public ITry<U> Transform<U>(Func<T, ITry<U>> s, Func<Exception, ITry<U>> f)
        {
            Debug.Assert(f != null);

            try
            {
                return f(this.Exception);
            }
            catch (Exception ex)
            {
                return new Failure<U>(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ITry<T> other = obj as ITry<T>;
            if ((object)other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.exception.GetHashCode();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ITry<T> other)
        {
            if ((object)other == null)
            {
                return false;
            }

            Failure<T> failure = other as Failure<T>;
            if ((object)failure == null)
            {
                return false;
            }

            return this.exception.Equals(failure.exception);
        }
    }
}