namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    ///
    /// </summary>
    public class UserInterfaceThreadExecutor : IExecutor
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly IExecutor instance = new UserInterfaceThreadExecutor();

        /// <summary>
        ///
        /// </summary>
        private UserInterfaceThreadExecutor()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public static IExecutor Instance
        {
            get
            {
                return UserInterfaceThreadExecutor.instance;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void Execute(Action f)
        {
            Debug.Assert(f != null);

            try
            {
                if (Deployment.Current.Dispatcher.CheckAccess())
                {
                    DirectExecutor.Instance.Execute(f);
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => { DirectExecutor.Instance.Execute(f); });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}