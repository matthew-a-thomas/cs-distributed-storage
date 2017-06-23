namespace DistributedStorage.Networking
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Encoding;
    using Model;
    using Protocol;
    using Protocol.Methods;
    using Serialization;

    /// <summary>
    /// Implements the remote side of a networked <see cref="IBucket{TIdentity}"/>
    /// </summary>
    public sealed class RemoteBucket<TIdentity> : IBucket<TIdentity>
        where TIdentity : IIdentity
    {
        #region Public properties

        public long MaxSize => _getMaxSizePropertyMethod.InvokeAndWait(Nothing.Instance);
        public TIdentity OwnerIdentity => _getOwnerIdentityPropertyMethod.InvokeAndWait(Nothing.Instance);
        public TIdentity PoolIdentity => _getPoolIdentityPropertyMethod.InvokeAndWait(Nothing.Instance);
        public TIdentity SelfIdentity => _getSelfIdentityPropertyMethod.InvokeAndWait(Nothing.Instance);

        #endregion

        #region Private fields

        private readonly IMethod<Nothing, long> _getCurrentSizeMethod;
        private readonly IMethod<Manifest, Hash[]> _getHashesMethod;
        private readonly IMethod<Nothing, Manifest[]> _getManifestsMethod;
        private readonly IMethod<Tuple<Manifest, Hash[]>, Slice[]> _getSlicesMethod;
        private readonly IMethod<Nothing, long> _getMaxSizePropertyMethod;
        private readonly IMethod<Nothing, TIdentity> _getOwnerIdentityPropertyMethod;
        private readonly IMethod<Nothing, TIdentity> _getPoolIdentityPropertyMethod;
        private readonly IMethod<Nothing, TIdentity> _getSelfIdentityPropertyMethod;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="RemoteBucket{TIdentity}"/>, which implements the remote side of a networked <see cref="IBucket{TIdentity}"/>
        /// </summary>
        public RemoteBucket(
            IProtocol protocol,
            IConverter<Nothing, byte[]> nothingToBytesConverter,
            IConverter<byte[], long> bytesToLongConverter,
            IConverter<Manifest, byte[]> manifestToBytesConverter,
            IConverter<byte[], Hash[]> bytesToHashArrayConverter,
            IConverter<byte[], Manifest[]> bytesToManifestArrayConverter,
            IConverter<Tuple<Manifest, Hash[]>, byte[]> manifestAndHashArrayTupleToBytesConverter,
            IConverter<byte[], Slice[]> bytesToSliceArrayConverter,
            IConverter<byte[], TIdentity> bytesToTIdentityConverter
            )
        {
            _getCurrentSizeMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetCurrentSize), nothingToBytesConverter, bytesToLongConverter, () => { });
            _getHashesMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetHashes), manifestToBytesConverter, bytesToHashArrayConverter, () => { });
            _getManifestsMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetManifests), nothingToBytesConverter, bytesToManifestArrayConverter, () => { });
            _getMaxSizePropertyMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.MaxSize), nothingToBytesConverter, bytesToLongConverter, () => { });
            _getOwnerIdentityPropertyMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.OwnerIdentity), nothingToBytesConverter, bytesToTIdentityConverter, () => { });
            _getPoolIdentityPropertyMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.PoolIdentity), nothingToBytesConverter, bytesToTIdentityConverter, () => { });
            _getSlicesMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetSlices), manifestAndHashArrayTupleToBytesConverter, bytesToSliceArrayConverter, () => { });
            _getSelfIdentityPropertyMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.SelfIdentity), nothingToBytesConverter, bytesToTIdentityConverter, () => { });
        }

        #endregion

        #region Public methods

        public long GetCurrentSize() => _getCurrentSizeMethod.InvokeAndWait(Nothing.Instance);
        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _getHashesMethod.InvokeAndWait(forManifest);
        public IEnumerable<Manifest> GetManifests() => _getManifestsMethod.InvokeAndWait(Nothing.Instance);
        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes) => _getSlicesMethod.InvokeAndWait(Tuple.Create(forManifest, hashes));

        #endregion
    }
}
