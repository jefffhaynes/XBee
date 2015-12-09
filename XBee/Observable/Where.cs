using System;

namespace XBee.Observable
{
    internal class Where<TSource> : Producer<TSource>
    {
        private readonly IObservable<TSource> _source;
        private readonly Func<TSource, bool> _predicate;

        public Where(IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public override IDisposable Subscribe(IObserver<TSource> observer)
        {
            var sink = new WhereSink(this, observer);
            return _source.Subscribe(sink);
        }

        class WhereSink : Sink<TSource>, IObserver<TSource>
        {
            private readonly Where<TSource> _parent;

            public WhereSink(Where<TSource> parent, IObserver<TSource> observer) : base(observer)
            {
                _parent = parent;
            }

            public void OnNext(TSource value)
            {
                if(_parent._predicate(value))
                    Observer.OnNext(value);
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
