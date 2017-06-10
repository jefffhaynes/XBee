using System;

namespace XBee.Observable
{
    internal class Select<TSource, TResult> : Producer<TResult>
    {
        private readonly Func<TSource, TResult> _selector;
        private readonly IObservable<TSource> _source;

        public Select(IObservable<TSource> source, Func<TSource, TResult> selector)
        {
            _source = source;
            _selector = selector;
        }

        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            var sink = new SelectSink(this, observer);
            return _source.Subscribe(sink);
        }

        private class SelectSink : Sink<TResult>, IObserver<TSource>
        {
            private readonly Select<TSource, TResult> _parent;

            public SelectSink(Select<TSource, TResult> parent, IObserver<TResult> observer)
                : base(observer)
            {
                _parent = parent;
            }

            public void OnNext(TSource value)
            {
                Observer.OnNext(_parent._selector(value));
            }

            public void OnError(Exception error)
            {
                Observer.OnError(error);
            }

            public void OnCompleted()
            {
                Observer.OnCompleted();
            }
        }
    }
}