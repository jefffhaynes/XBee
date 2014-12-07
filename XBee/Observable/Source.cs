using System;
using System.Collections.Generic;

namespace XBee.Observable
{
    public class Source<TSource> : IObservable<TSource>, IDisposable
    {
        private readonly List<IObserver<TSource>> _observers;
        private readonly object _observersLock = new object();

        internal Source()
        {
            _observers = new List<IObserver<TSource>>();
        }

        public IDisposable Subscribe(IObserver<TSource> observer)
        {
            lock (_observersLock)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
            }

            return new AnonymousDisposable(() =>
            {
                lock (_observersLock)
                    _observers.Remove(observer);
            });
        }

        public void Push(TSource value)
        {
            lock (_observersLock)
            {
                foreach (var observer in _observers)
                    observer.OnNext(value);
            }
        }

        public void Dispose()
        {
            lock (_observersLock)
            {
                foreach (var observer in _observers)
                    observer.OnCompleted();

                _observers.Clear();
            }
        }
    }
}