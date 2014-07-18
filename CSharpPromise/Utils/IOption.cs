namespace CSharpPromise.Utils
{
    using System;
    using System.Diagnostics;

    public interface IOption<out T> 
    {
        bool IsEmpty
        {
            get;
        }

        T Get();
    }

    public sealed class Some<T> : IOption<T>
    {
        private readonly T x;

        public Some(T x)
        {
            this.x = x;
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public T Get()
        {
            return this.x;
        }
    }

    public sealed class None<T> : IOption<T>
    {
        public static readonly IOption<T> Instance = new None<T>();

        private None()
        {
        }
        
        public bool IsEmpty
        {
            get { return true; }
        }

        public T Get()
        {
            throw new InvalidOperationException("None.Get");
        }
    }

    public static class OptionExtension
    {
        public static IOption<U> FlatMap<T, U>(this IOption<T> o, Func<T, IOption<U>> f)
        {
            Debug.Assert(o != null && f != null);

            if (o.IsEmpty)
            {
                return None<U>.Instance;
            }

            return f(o.Get());
        }

        public static IOption<U> Map<T, U>(this IOption<T> o, Func<T, U> f)
        {
            Debug.Assert(o != null && f != null);

            if (o.IsEmpty)
            {
                return None<U>.Instance;
            }

            return new Some<U>(f(o.Get()));
        }
    }
}