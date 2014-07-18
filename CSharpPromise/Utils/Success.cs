namespace CSharpPromise.Utils
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Success<T> : ITry<T>, IEquatable<ITry<T>>
    {
        /// <summary>
        ///
        /// </summary>
        private readonly T value;

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public Success(T value)
        {
            this.value = value;
        }

        /// <summary>
        ///
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFailure
        {
            get { return false; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSuccess
        {
            get { return true; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return this.Value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public ITry<U> FlatMap<U>(Func<T, ITry<U>> f)
        {
            Debug.Assert(f != null);

            try
            {
                return f(this.Value);
            }
            catch (Exception ex)
            {
                return new Failure<U>(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void Foreach(Action<T> f)
        {
            Debug.Assert(f != null);

            f(this.Value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public ITry<U> Map<U>(Func<T, U> f)
        {
            Debug.Assert(f != null);

            return TryFactory.Create(() => f(this.Value));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ITry<T> Filter(Func<T, bool> p)
        {
            Debug.Assert(p != null);

            try
            {
                if (p(this.Value))
                {
                    return this;
                }

                return new Failure<T>(new IndexOutOfRangeException(string.Format("Predicate does not hold for {0}", value == null ? "null" : this.value.ToString())));
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ITry<Exception> Failed()
        {
            return new Failure<Exception>(new InvalidOperationException("Success.Failed"));
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
            Debug.Assert(s != null);

            try
            {
                return s(this.Value);
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
            if ((object)this.value == null)
            {
                return 0;
            }

            return this.value.GetHashCode();
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

            Success<T> success = other as Success<T>;
            if ((object)success == null)
            {
                return false;
            }

            if ((object)this.value == null && (object)success.value == null)
            {
                return true;
            }

            if ((object)this.value == null || (object)success.value == null)
            {
                return false;
            }

            return this.value.Equals(success.value);
        }
    }
}