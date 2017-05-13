using System;
using System.Threading;

namespace XBee
{
    internal sealed class AnonymousDisposable : IDisposable
    {
        private volatile Action _dispose;

        /// <summary>
        /// Constructs a new disposable with the given action used for disposal.
        /// </summary>
        /// <param name="dispose">Disposal action which will be run upon calling Dispose.</param>
        public AnonymousDisposable(Action dispose)
        {
            _dispose = dispose;
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => _dispose == null;

        /// <summary>
        /// Calls the disposal action if and only if the current instance hasn't been disposed yet.
        /// </summary>
        public void Dispose()
        {
#pragma warning disable 0420
            var dispose = Interlocked.Exchange(ref _dispose, null);
#pragma warning restore 0420
            dispose?.Invoke();
        }
    }
}
