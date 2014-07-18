namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class ThreadPoolExecutor : IExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly IExecutor instance = new ThreadPoolExecutor();

        /// <summary>
        /// 
        /// </summary>
        private ThreadPoolExecutor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static IExecutor Instance
        {
            get
            {
                return ThreadPoolExecutor.instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        public void Execute(Action f)
        {
            try
            {
                ThreadPool.QueueUserWorkItem((ignored) => { DirectExecutor.Instance.Execute(f); });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}