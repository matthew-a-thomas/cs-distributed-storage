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

    public sealed class RemoteBucket<TIdentity> : IBucket<TIdentity>
        where TIdentity : IIdentity
    {
        #region Public properties

        public long MaxSize { get; }
        public TIdentity OwnerIdentity { get; }
        public TIdentity PoolIdentity { get; }
        public TIdentity SelfIdentity { get; }

        #endregion

        #region Private fields

        private readonly IMethod<Nothing, long> _getCurrentSizeMethod;
        private readonly IMethod<Manifest, Hash[]> _getHashesMethod;
        private readonly IMethod<Nothing, Manifest[]> _getManifestsMethod;
        private readonly IMethod<Tuple<Manifest, Hash[]>, Slice[]> _getSlicesMethod;

        #endregion

        #region Constructor

        public RemoteBucket(
            IProtocol protocol,
            IConverter<Nothing, byte[]> nothingToBytesConverter,
            IConverter<byte[], long> bytesToLongConverter,
            IConverter<Manifest, byte[]> manifestToBytesConverter,
            IConverter<byte[], Hash[]> bytesToHashArrayConverter,
            IConverter<byte[], Manifest[]> bytesToManifestArrayConverter,
            IConverter<Tuple<Manifest, Hash[]>, byte[]> manifestAndHashArrayTupleToBytesConverter,
            IConverter<byte[], Slice[]> bytesToSliceArrayConverter
            )
        {
            _getCurrentSizeMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetCurrentSize), nothingToBytesConverter, bytesToLongConverter, () => { });
            _getHashesMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetHashes), manifestToBytesConverter, bytesToHashArrayConverter, () => { });
            _getManifestsMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetManifests), nothingToBytesConverter, bytesToManifestArrayConverter, () => { });
            _getSlicesMethod = ProtocolMethod.Create(protocol, nameof(IBucket<TIdentity>.GetSlices), manifestAndHashArrayTupleToBytesConverter, bytesToSliceArrayConverter, () => { });
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
