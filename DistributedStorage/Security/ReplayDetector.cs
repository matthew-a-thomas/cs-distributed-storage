namespace DistributedStorage.Security
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Something that detects replays
    /// </summary>
    public sealed class ReplayDetector<T>
    {
        private long _smallestAllowedTag = long.MinValue;
        private readonly HashSet<T> _seenHashes = new HashSet<T>();
        private readonly LinkedList<Tuple<long, T>> _timeline = new LinkedList<Tuple<long, T>>();

        /// <summary>
        /// Removes any history of objects that have tags lower than or equal to the given <paramref name="throughTag"/>
        /// </summary>
        public void Clean(long throughTag)
        {
            _smallestAllowedTag = Math.Max(throughTag, _smallestAllowedTag);
            while (_timeline.Count > 0 && _timeline.First.Value.Item1 <= throughTag)
            {
                _seenHashes.Remove(_timeline.First.Value.Item2);
                _timeline.RemoveFirst();
            }
        }

        /// <summary>
        /// Returns false if the given <paramref name="obj"/> has been seen recently, or if the given <paramref name="tag"/> is not larger than the last given <paramref name="tag"/>.
        /// Otherwise, the given <paramref name="obj"/> is added to an internal list and will not be removed until <see cref="Clean"/> is called with a value at least as large as this <paramref name="tag"/>
        /// </summary>
        public bool TryAdd(T obj, long tag)
        {
            if (tag <= _smallestAllowedTag)
                return false;
            if (!_seenHashes.Add(obj))
                return false;
            _timeline.AddLast(Tuple.Create(tag, obj));
            return true;
        }
    }
}
