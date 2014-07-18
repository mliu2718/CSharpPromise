namespace CSharpPromise.Utils
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///
    /// </summary>
    public static class TryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ITry<T> Create<T>(Func<T> f)
        {
            Debug.Assert(f != null);

            try
            {
                return new Success<T>(f());
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }
    }
}