using System;

namespace XBee.Observable
{
    internal abstract class Producer<TSource> : IObservable<TSource>
    {
        public abstract IDisposable Subscribe(IObserver<TSource> observer);
    }
}
