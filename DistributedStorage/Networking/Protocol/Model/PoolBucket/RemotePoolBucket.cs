namespace DistributedStorage.Networking.Protocol.Model.PoolBucket
{
    using System;
    using System.Reflection;
    using Common;
    using DistributedStorage.Model;
    using Encoding;
    using Methods;
    using Serialization;

    public sealed class RemotePoolBucket : IPoolBucket
    {
        public sealed class Factory
        {
            private readonly IConverter<Tuple<Manifest, Slice[]>, byte[]> _manifestAndSliceArrayTupleToBytesConverter;
            private readonly IConverter<byte[], Nothing> _bytesToNothingConverter;
            private readonly IConverter<Tuple<Manifest, Hash[]>, byte[]> _manifestAndHashArrayTupleToBytesConverter;

            public Factory(
                IConverter<Tuple<Manifest, Slice[]>, byte[]> manifestAndSliceArrayTupleToBytesConverter,
                IConverter<byte[], Nothing> bytesToNothingConverter,
                IConverter<Tuple<Manifest, Hash[]>, byte[]> manifestAndHashArrayTupleToBytesConverter)
            {
                _manifestAndSliceArrayTupleToBytesConverter = manifestAndSliceArrayTupleToBytesConverter;
                _bytesToNothingConverter = bytesToNothingConverter;
                _manifestAndHashArrayTupleToBytesConverter = manifestAndHashArrayTupleToBytesConverter;
            }

            public RemotePoolBucket CreateFrom(IProtocol protocol) => new RemotePoolBucket(
                protocol,
                _manifestAndSliceArrayTupleToBytesConverter,
                _bytesToNothingConverter,
                _manifestAndHashArrayTupleToBytesConverter
            );
        }

        #region Private fields

        private readonly IMethod<Tuple<Manifest, Slice[]>, Nothing> _addSlicesMethod;
        private readonly IMethod<Tuple<Manifest, Hash[]>, Nothing> _deleteSlicesMethod;

        #endregion

        #region Constructor

        public RemotePoolBucket(
            IProtocol protocol,
            IConverter<Tuple<Manifest, Slice[]>, byte[]> manifestAndSliceArrayTupleToBytesConverter,
            IConverter<byte[], Nothing> bytesToNothingConverter,
            IConverter<Tuple<Manifest, Hash[]>, byte[]> manifestAndHashArrayTupleToBytesConverter
        )
        {
            var type = typeof(IPoolBucket);

            _addSlicesMethod = ProtocolMethod.Create(protocol, type.GetMethod(nameof(IPoolBucket.AddSlices)).GetNameForProtocolRegistration(), manifestAndSliceArrayTupleToBytesConverter, bytesToNothingConverter);
            _deleteSlicesMethod = ProtocolMethod.Create(protocol, type.GetMethod(nameof(IPoolBucket.DeleteSlices)).GetNameForProtocolRegistration(), manifestAndHashArrayTupleToBytesConverter, bytesToNothingConverter);
        }

        #endregion

        #region Public methods

        public void AddSlices(Manifest forManifest, Slice[] slices) => _addSlicesMethod.InvokeAsync(Tuple.Create(forManifest, slices)).Wait();

        public void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete) => _deleteSlicesMethod.InvokeAsync(Tuple.Create(forManifest, hashesToDelete)).Wait();

        #endregion
    }
}
