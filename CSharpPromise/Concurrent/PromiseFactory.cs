namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    public static class PromiseFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IPromise<T> Create<T>()
        {
            return new PromiseImpl<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IPromise<T> Failed<T>(Exception exception)
        {
            Debug.Assert(exception != null);

            return PromiseFactory.FromTry(new Failure<T>(exception));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IPromise<T> FromTry<T>(ITry<T> result)
        {
            Debug.Assert(result != null);

            var p = PromiseFactory.Create<T>();

            p.Complete(result);

            return p;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IPromise<T> Successful<T>(T result)
        {
            return PromiseFactory.FromTry(new Success<T>(result));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class PromiseImpl<T> : AbstractPromise<T>
        {
            /// <summary>
            ///
            /// </summary>
            private readonly FutureImpl<T> future = new FutureImpl<T>();

            /// <summary>
            ///
            /// </summary>
            public PromiseImpl()
            {
            }

            /// <summary>
            ///
            /// </summary>
            public override IFuture<T> Future
            {
                get
                {
                    return this.future;
                }
            }

            /// <summary>
            ///
            /// </summary>
            public override bool IsCompleted
            {
                get
                {
                    return this.future.IsCompleted;
                }
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            public override bool TryComplete(ITry<T> result)
            {
                Debug.Assert(result != null);

                return this.future.TryComplete(result);
            }

            /// <summary>
            ///
            /// </summary>
            /// <typeparam name="T"></typeparam>
            private sealed class FutureImpl<U> : AbstractFuture<U>
            {
                /// <summary>
                ///
                /// </summary>
                private readonly ManualResetEvent asyncWaitHandle = new ManualResetEvent(false);

                /// <summary>
                ///
                /// </summary>
                private Action<ITry<U>> callbacks = null;

                /// <summary>
                ///
                /// </summary>
                private ITry<U> value = null;

                /// <summary>
                ///
                /// </summary>
                public FutureImpl()
                {
                }

                /// <summary>
                ///
                /// </summary>
                public override bool IsCompleted
                {
                    get
                    {
                        return this.value != null;
                    }
                }

                /// <summary>
                ///
                /// </summary>
                /// <param name="f"></param>
                /// <param name="executor"></param>
                public override void OnComplete(Action<ITry<U>> f, IExecutor executor)
                {
                    Action<ITry<U>> callback = (t) => { executor.Execute(() => { f(t); }); }; // TODO: Executor.Execute MUST NOT throw

                    if (this.IsCompleted)
                    {
                        callback(this.value);
                    }
                    else
                    {
                        this.callbacks += callback;
                        if (this.IsCompleted)
                        {
                            var callbacks = Interlocked.Exchange(ref this.callbacks, null);
                            if (callbacks != null)
                            {
                                callbacks(this.value);
                            }
                        }
                    }
                }

                /// <summary>
                ///
                /// </summary>
                public override void Await()
                {
                    this.asyncWaitHandle.WaitOne();
                }

                /// <summary>
                ///
                /// </summary>
                /// <param name="atMost"></param>
                public override void Await(int millisecondsTimeout)
                {
                    this.asyncWaitHandle.WaitOne(millisecondsTimeout);
                    if (!this.IsCompleted)
                    {
                        this.TryComplete(new Failure<U>(new TimeoutException()));
                    }
                }

                /// <summary>
                ///
                /// </summary>
                /// <returns></returns>
                public override U Result()
                {
                    this.Await();
                    return this.value.Get();
                }

                /// <summary>
                ///
                /// </summary>
                /// <param name="atMost"></param>
                /// <returns></returns>
                public override U Result(int millisecondsTimeout)
                {
                    this.Await(millisecondsTimeout);
                    return this.value.Get();
                }

                /// <summary>
                ///
                /// </summary>
                /// <param name="value"></param>
                /// <returns></returns>
                internal bool TryComplete(ITry<U> value)
                {
                    if (Interlocked.CompareExchange(ref this.value, value, null) == null)
                    {
                        var callbacks = Interlocked.Exchange(ref this.callbacks, null);
                        if (callbacks != null)
                        {
                            callbacks(this.value);
                        }

                        try
                        {
                            this.asyncWaitHandle.Set();
                        }
                        catch (Exception)
                        {
                        }

                        return true;
                    }

                    return false;
                }
            }
        }
    }
}