using System;

namespace XBee.Observable
{
    internal abstract class Sink<TSource>
    {
        protected readonly IObserver<TSource> Observer;

        protected Sink(IObserver<TSource> observer)
        {
            Observer = observer;
        }
    }
}