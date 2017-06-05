namespace DistributedStorage.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A stream that reads from and writes to different <see cref="Queue{T}"/>s
    /// </summary>
    public class Pipe : Stream
    {
        #region Private fields

        /// <summary>
        /// The queue to read from
        /// </summary>
        private readonly Queue<byte> _readFrom;

        /// <summary>
        /// The queue to write to
        /// </summary>
        private readonly Queue<byte> _writeTo;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="Stream"/> that will read from and write to the given <see cref="Queue{T}"/>s
        /// </summary>
        public Pipe(Queue<byte> readFrom, Queue<byte> writeTo)
        {
            _readFrom = readFrom;
            _writeTo = writeTo;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a pair of <see cref="Pipe"/>s that are linked such that writing to one will make bytes available to read from the other
        /// </summary>
        public static (Pipe one, Pipe two) CreateLinkedPair()
        {
            Queue<byte>
                q1 = new Queue<byte>(),
                q2 = new Queue<byte>();
            Pipe
                one = new Pipe(q1, q2),
                two = new Pipe(q2, q1);
            return (one, two);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// Reads up to <paramref name="count"/> bytes from the read queue
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                if (_readFrom.Count == 0)
                    return i;
                buffer[i + offset] = _readFrom.Dequeue();
            }
            return count;
        }

        /// <summary>
        /// Throws an exception
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        /// <summary>
        /// Throws an exception
        /// </summary>
        public override void SetLength(long value) => throw new NotImplementedException();

        /// <summary>
        /// Writes <paramref name="count"/> bytes from the given <paramref name="buffer"/> (starting at the given <paramref name="offset"/>) into the write queue
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                _writeTo.Enqueue(buffer[i + offset]);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true
        /// </summary>
        public override bool CanRead { get; } = true;

        /// <summary>
        /// Returns false
        /// </summary>
        public override bool CanSeek { get; } = false;

        /// <summary>
        /// Returns true
        /// </summary>
        public override bool CanWrite { get; } = true;

        /// <summary>
        /// Gets the number of bytes currently in the read queue
        /// </summary>
        public override long Length => _readFrom.Count;

        /// <summary>
        /// Throws an exception
        /// </summary>
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion
    }
}
