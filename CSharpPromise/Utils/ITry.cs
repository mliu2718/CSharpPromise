namespace CSharpPromise.Utils
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITry<out T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ITry<Exception> Failed();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        ITry<T> Filter(Func<T, bool> p);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        ITry<U> FlatMap<U>(Func<T, ITry<U>> f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        void Foreach(Action<T> f);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// 
        /// </summary>
        bool IsFailure
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsSuccess
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        ITry<U> Map<U>(Func<T, U> f);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        ITry<U> Transform<U>(Func<T, ITry<U>> s, Func<Exception, ITry<U>> f);
    }
}