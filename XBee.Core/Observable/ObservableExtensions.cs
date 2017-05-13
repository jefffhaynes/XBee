using System;

namespace XBee.Observable
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> Where<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new Where<TSource>(source, predicate);
        }

        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return new Select<TSource, TResult>(source, selector);
        }
    }
}