namespace Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;

    /// <summary>
    /// Something which remembers instances of <typeparamref name="T"/> for a certain amount of time
    /// </summary>
    public sealed class ReplayDetector<T> : IDisposable
    {
        /// <summary>
        /// Our subscription, which causes us to forget about items in the future
        /// </summary>
        private IDisposable _disposable;

        /// <summary>
        /// Something through which new items are published so that they can be forgotten about a certain amount of time later
        /// </summary>
        private readonly IObserver<T> _itemObserver;

        /// <summary>
        /// Our recollection of items
        /// </summary>
        private readonly HashSet<T> _seenItems = new HashSet<T>();

        /// <summary>
        /// Creates a new <see cref="ReplayDetector{T}"/>, which remembers instances of <see cref="T"/> for the given <paramref name="attentionSpan"/>
        /// </summary>
        public ReplayDetector(TimeSpan attentionSpan)
        {
            var subject = new Subject<T>();
            _itemObserver = subject;
            _disposable = subject.Delay(attentionSpan).Subscribe(ForgetAboutItem);
        }

        /// <summary>
        /// Stops an internal subscription
        /// </summary>
        public void Dispose() => Interlocked.Exchange(ref _disposable, null)?.Dispose();

        /// <summary>
        /// Causes us to forget about the given <paramref name="item"/> so that passing it to <see cref="IsUnique(T)"/> again would return true
        /// </summary>
        private void ForgetAboutItem(T item)
        {
            lock (_seenItems)
            {
                _seenItems.Remove(item);
            }
        }

        /// <summary>
        /// Remembers the given <paramref name="item"/> for the amount of time given to the constructor, after which it will be considered unique again.
        /// Returns false if the given <paramref name="item"/> has already been seen before within that amount of time.
        /// </summary>
        public bool IsUnique(T item)
        {
            lock (_seenItems)
            {
                if (!_seenItems.Add(item))
                    return false;
                _itemObserver.OnNext(item);
                return true;
            }
        }
    }
}
