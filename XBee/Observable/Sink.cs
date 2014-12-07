using System;

namespace XBee.Observable
{
    internal abstract class Sink<TSource>
    {
        protected readonly IObserver<TSource> _observer;

        protected Sink(IObserver<TSource> observer)
        {
            _observer = observer;
        }
    }
}