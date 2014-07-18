namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class DirectExecutor : IExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly IExecutor instance = new DirectExecutor();

        /// <summary>
        /// 
        /// </summary>
        private DirectExecutor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static IExecutor Instance
        {
            get
            {
                return DirectExecutor.instance;
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
                f();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}