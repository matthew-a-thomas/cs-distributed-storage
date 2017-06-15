namespace DistributedStorage.Actors
{
    using System;
    using System.Threading;

    /// <summary>
    /// A thread-safe singly-linked list which notifies when something has been enqueued
    /// </summary>
    public sealed class WorkQueue<T>
    {
        /// <summary>
        /// A node in a singly-linked list.
        /// Encapsulates a value of <see cref="T"/>, and points forward to another node
        /// </summary>
        public sealed class Node
        {
            /// <summary>
            /// The next <see cref="Node"/> after this one, if there is one
            /// </summary>
            public Node Next { get; internal set; }

            /// <summary>
            /// The value contained in this <see cref="Node"/>
            /// </summary>
            public readonly T Value;

            /// <summary>
            /// Creates a new <see cref="Node"/> having the given <paramref name="value"/>
            /// </summary>
            internal Node(T value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// The callback to execute when a new item is enqueued in this <see cref="WorkQueue{T}"/>
        /// </summary>
        private readonly Action<Node> _callback;

        /// <summary>
        /// The last created <see cref="Node"/>
        /// </summary>
        private Node _tail;

        /// <summary>
        /// Creates a new <see cref="WorkQueue{T}"/>, which invokes the given <paramref name="callback"/> when a new item is enqueued
        /// </summary>
        public WorkQueue(Action<Node> callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Enqueues an item, invoking the callback with the new <see cref="Node"/> that was created to encapsulate it
        /// </summary>
        public void Enqueue(T item)
        {
            var node = new Node(item);
            var oldTail = Interlocked.Exchange(ref _tail, node);
            if (oldTail != null)
                oldTail.Next = node;
            _callback(node);
        }
    }
}
