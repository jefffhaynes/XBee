using System;

namespace XBee.Observable
{
    public abstract class Producer<TSource> : IObservable<TSource>
    {
        public abstract IDisposable Subscribe(IObserver<TSource> observer);
    }
}
