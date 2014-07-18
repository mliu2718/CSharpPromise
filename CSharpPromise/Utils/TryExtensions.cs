namespace CSharpPromise.Utils
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///
    /// </summary>
    public static class TryExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetOrElse<T>(this ITry<T> t, T defaultValue)
        {
            Debug.Assert(t != null);

            if (t.IsSuccess)
            {
                return t.Get();
            }

            return defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ITry<T> OrElse<T>(this ITry<T> t, ITry<T> defaultValue)
        {
            Debug.Assert(t != null && defaultValue != null);

            if (t.IsSuccess)
            {
                return t;
            }

            return defaultValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ITry<T> Recover<T>(this ITry<T> t, Func<Exception, T> f)
        {
            Debug.Assert(t != null && f != null);

            if (t.IsSuccess)
            {
                return t;
            }

            return TryFactory.Create(() => f(t.Failed().Get()));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ITry<T> RecoverWith<T>(this ITry<T> t, Func<Exception, ITry<T>> f)
        {
            Debug.Assert(t != null && f != null);

            if (t.IsSuccess)
            {
                return t;
            }

            try
            {
                return f(t.Failed().Get());
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ITry<T> Flatten<T>(this ITry<ITry<T>> t)
        {
            Debug.Assert(t != null);

            return TryFactory.Create(() => t.Get().Get());
        }
    }
}