namespace DistributedStorage.Networking.Protocol.Model.Bucket
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Common;
    using DistributedStorage.Model;
    using Encoding;
    using Protocol;
    using Serialization;

    /// <summary>
    /// Implements the client side of a networked <see cref="IBucket{TIdentity}"/> by registering the properties and methods of an <see cref="IBucket{TIdentity}"/> to be exposed over an <see cref="IProtocol"/> to a corresponding <see cref="RemoteBucket{TIdentity}"/>
    /// </summary>
    public sealed class LocalBucketProtocolInitializer<TIdentity> : IProtocolInitializer<IBucket<TIdentity>>
        where TIdentity : IIdentity
    {
        #region Private fields

        private readonly IConverter<byte[], Nothing> _bytesToNothingConverter;
        private readonly IConverter<long, byte[]> _longToBytesConverter;
        private readonly IConverter<byte[], Manifest> _bytesToManifestConverter;
        private readonly IConverter<Hash[], byte[]> _hashArrayToBytesConverter;
        private readonly IConverter<Manifest[], byte[]> _manifestArrayToBytesConverter;
        private readonly IConverter<byte[], Tuple<Manifest, Hash[]>> _bytesToManifestAndHashArrayTupleConverter;
        private readonly IConverter<Slice[], byte[]> _sliceArrayToBytesConverter;
        private readonly IConverter<TIdentity, byte[]> _tIdentityToBytesConverter;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="LocalBucketProtocolInitializer{TIdentity}"/> that will use the given <see cref="IConverter{TFrom, TTo}"/>s to expose an <see cref="IBucket{TIdentity}"/>'s properties and methods to an <see cref="IProtocol"/>
        /// </summary>
        public LocalBucketProtocolInitializer(
            IConverter<byte[], Nothing> bytesToNothingConverter,
            IConverter<long, byte[]> longToBytesConverter,
            IConverter<byte[], Manifest> bytesToManifestConverter,
            IConverter<Hash[], byte[]> hashArrayToBytesConverter,
            IConverter<Manifest[], byte[]> manifestArrayToBytesConverter,
            IConverter<byte[], Tuple<Manifest, Hash[]>> bytesToManifestAndHashArrayTupleConverter,
            IConverter<Slice[], byte[]> sliceArrayToBytesConverter,
            IConverter<TIdentity, byte[]> tIdentityToBytesConverter
            )
        {
            _bytesToNothingConverter = bytesToNothingConverter;
            _longToBytesConverter = longToBytesConverter;
            _bytesToManifestConverter = bytesToManifestConverter;
            _hashArrayToBytesConverter = hashArrayToBytesConverter;
            _manifestArrayToBytesConverter = manifestArrayToBytesConverter;
            _bytesToManifestAndHashArrayTupleConverter = bytesToManifestAndHashArrayTupleConverter;
            _sliceArrayToBytesConverter = sliceArrayToBytesConverter;
            _tIdentityToBytesConverter = tIdentityToBytesConverter;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Implements the client side of a networked <see cref="IBucket{TIdentity}"/> by registering the properties and methods of an <see cref="IBucket{TIdentity}"/> to be exposed over an <see cref="IProtocol"/> to a corresponding <see cref="RemoteBucket{TIdentity}"/>
        /// </summary>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public bool TrySetup(
            IProtocol protocol,
            IBucket<TIdentity> bucket
            )
        {
            // Register methods
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.GetCurrentSize), Handler.CreateFrom(_bytesToNothingConverter, _longToBytesConverter, nothing => bucket.GetCurrentSize())))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.GetHashes), Handler.CreateFrom(_bytesToManifestConverter, _hashArrayToBytesConverter, forManifest => bucket.GetHashes(forManifest).ToArray())))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.GetManifests), Handler.CreateFrom(_bytesToNothingConverter, _manifestArrayToBytesConverter, nothing => bucket.GetManifests().ToArray())))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.GetSlices), Handler.CreateFrom(_bytesToManifestAndHashArrayTupleConverter, _sliceArrayToBytesConverter, tuple => bucket.GetSlices(tuple.Item1, tuple.Item2).ToArray())))
                return false;

            // Register properties
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.MaxSize), Handler.CreateFrom(_bytesToNothingConverter, _longToBytesConverter, nothing => bucket.MaxSize)))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.OwnerIdentity), Handler.CreateFrom(_bytesToNothingConverter, _tIdentityToBytesConverter, nothing => bucket.OwnerIdentity)))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.PoolIdentity), Handler.CreateFrom(_bytesToNothingConverter, _tIdentityToBytesConverter, nothing => bucket.PoolIdentity)))
                return false;
            if (!protocol.TryRegister(nameof(IBucket<TIdentity>.SelfIdentity), Handler.CreateFrom(_bytesToNothingConverter, _tIdentityToBytesConverter, nothing => bucket.SelfIdentity)))
                return false;

            return true;
        }

        #endregion
    }
}
