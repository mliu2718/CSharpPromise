namespace CSharpPromise.Concurrent
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        void Execute(Action f);
    }
}