namespace CSharpPromise.Concurrent
{
    using System;
    using System.Diagnostics;
    using CSharpPromise.Utils;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractFuture<T> : IFuture<T>
    {
        /// <summary>
        ///
        /// </summary>
        public abstract bool IsCompleted
        {
            get;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnComplete(Action<ITry<T>> f)
        {
            Debug.Assert(f != null);

            this.OnComplete(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        public abstract void OnComplete(Action<ITry<T>> f, IExecutor executor);

        /// <summary>
        ///
        /// </summary>
        public abstract void Await();

        /// <summary>
        ///
        /// </summary>
        /// <param name="atMost"></param>
        public abstract void Await(int millisecondsTimeout);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public abstract T Result();

        /// <summary>
        ///
        /// </summary>
        /// <param name="atMost"></param>
        /// <returns></returns>
        public abstract T Result(int millisecondsTimeout);

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFuture<T> AndThen(Action<ITry<T>> f)
        {
            Debug.Assert(f != null);

            return this.AndThen(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<T> AndThen(Action<ITry<T>> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<T>();

            Action<ITry<T>> callback = (t) =>
            {
                try
                {
                    executor.Execute(() => { f(t); });
                }
                finally
                {
                    p.Complete(t);
                }
            };

            this.OnComplete(callback);

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFuture<S> Collect<S>(Func<T, S> f)
        {
            Debug.Assert(f != null);

            return this.Collect(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<S> Collect<S>(Func<T, S> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            return this.Map(f, executor);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IFuture<Exception> Failed()
        {
            var p = PromiseFactory.Create<Exception>();

            this.OnComplete((t) =>
            {
                if (t.IsSuccess)
                {
                    p.Failure(new IndexOutOfRangeException("Future.Failed not completed with an exception"));
                }
                else
                {
                    p.Success(t.Failed().Get());
                }
            });

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IFuture<T> Filter(Func<T, bool> p)
        {
            Debug.Assert(p != null);

            return this.Filter(p, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<T> Filter(Func<T, bool> p, IExecutor executor)
        {
            Debug.Assert(p != null && executor != null);

            return this.Map((v) =>
                {
                    if (p(v))
                    {
                        return v;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("Future.Filter predicate is not satisfied");
                    }
                },
                executor);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFuture<S> FlatMap<S>(Func<T, IFuture<S>> f)
        {
            Debug.Assert(f != null);

            return this.FlatMap(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<S> FlatMap<S>(Func<T, IFuture<S>> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<S>();

            this.OnFailure((e) => { p.Failure(e); });

            this.OnSuccess((v) =>
                {
                    try
                    {
                        f(v).OnComplete((t) => { p.Complete(t); });
                    }
                    catch (Exception ex)
                    {
                        p.Failure(ex);
                    }
                },
                executor);

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void Foreach(Action<T> f)
        {
            Debug.Assert(f != null);

            this.Foreach(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        public void Foreach(Action<T> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            this.OnComplete((t) => { t.Foreach(f); }, executor);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFuture<S> Map<S>(Func<T, S> f)
        {
            Debug.Assert(f != null);

            return this.Map(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<S> Map<S>(Func<T, S> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            var p = PromiseFactory.Create<S>();

            this.OnComplete((t) => { p.Complete(t.Map(f)); }, executor);

            return p.Future;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnFailure(Action<Exception> f)
        {
            Debug.Assert(f != null);

            this.OnFailure(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        public void OnFailure(Action<Exception> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            this.OnComplete((t) =>
            {
                if (t.IsFailure)
                {
                    t.Failed().Foreach((ex) => { executor.Execute(() => { f(ex); }); });
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        public void OnSuccess(Action<T> f)
        {
            Debug.Assert(f != null);

            this.OnSuccess(f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        public void OnSuccess(Action<T> f, IExecutor executor)
        {
            Debug.Assert(f != null && executor != null);

            this.OnComplete((t) =>
                {
                    if (t.IsSuccess)
                    {
                        t.Foreach((v) => { executor.Execute(() => { f(v); }); });
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFuture<S> Transform<S>(Func<T, S> s, Func<Exception, Exception> f)
        {
            Debug.Assert(s != null && f != null);

            return this.Transform(s, f, DirectExecutor.Instance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        public IFuture<S> Transform<S>(Func<T, S> s, Func<Exception, Exception> f, IExecutor executor)
        {
            Debug.Assert(s != null && f != null && executor != null);

            var p = PromiseFactory.Create<S>();

            this.OnSuccess((v) =>
                {
                    p.Complete(TryFactory.Create(() => s(v)));
                },
                executor);

            this.OnFailure((e) =>
                {
                    p.Complete(TryFactory.Create<S>(() => { throw f(e); }));
                },
                executor);

            return p.Future;
        }
    }
}