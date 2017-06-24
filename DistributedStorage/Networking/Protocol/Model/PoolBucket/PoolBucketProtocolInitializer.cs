namespace DistributedStorage.Networking.Protocol.Model.PoolBucket
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common;
    using DistributedStorage.Model;
    using Encoding;
    using Serialization;

    public sealed class PoolBucketProtocolInitializer : IProtocolInitializer<IPoolBucket>
    {
        private readonly IConverter<byte[], Tuple<Manifest, Slice[]>> _bytesToManifestAndSliceArrayTupleConverter;
        private readonly IConverter<Nothing, byte[]> _nothingToBytesConverter;
        private readonly IConverter<byte[], Tuple<Manifest, Hash[]>> _bytesToManifestAndHashArrayTupleConverter;

        public PoolBucketProtocolInitializer(
            IConverter<byte[], Tuple<Manifest, Slice[]>> bytesToManifestAndSliceArrayTupleConverter,
            IConverter<Nothing, byte[]> nothingToBytesConverter,
            IConverter<byte[], Tuple<Manifest, Hash[]>> bytesToManifestAndHashArrayTupleConverter
            )
        {
            _bytesToManifestAndSliceArrayTupleConverter = bytesToManifestAndSliceArrayTupleConverter;
            _nothingToBytesConverter = nothingToBytesConverter;
            _bytesToManifestAndHashArrayTupleConverter = bytesToManifestAndHashArrayTupleConverter;
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public bool TrySetup(IProtocol protocol, IPoolBucket with)
        {
            if (!protocol.TryRegister(nameof(IPoolBucket.AddSlices), Handler.CreateFrom(_bytesToManifestAndSliceArrayTupleConverter, _nothingToBytesConverter, tuple =>
            {
                with.AddSlices(tuple.Item1, tuple.Item2);
                return Nothing.Instance;
            })))
                return false;
            if (!protocol.TryRegister(nameof(IPoolBucket.DeleteSlices), Handler.CreateFrom(_bytesToManifestAndHashArrayTupleConverter, _nothingToBytesConverter, tuple =>
            {
                with.DeleteSlices(tuple.Item1, tuple.Item2);
                return Nothing.Instance;
            })))
                return false;

            return true;
        }
    }
}
