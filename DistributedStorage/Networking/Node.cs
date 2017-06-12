namespace DistributedStorage.Networking
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Encoding;
    using Protocol;
    using Protocol.Methods;
    using Serialization;
    using Storage;
    using Storage.Containers;

    /// <summary>
    /// Something that links a <see cref="Storage"/> with a <see cref="IProtocol"/>,
    /// satisfying remote requests for all the <see cref="Node"/> methods through the <see cref="Storage"/>,
    /// and satisfying local requests for all the <see cref="Node"/> methods through the <see cref="IProtocol"/>.
    /// 
    /// Thus every method you find in <see cref="Node"/> can be invoked by the remote party
    /// </summary>
    public sealed class Node : IDisposable
    {
        /// <summary>
        /// Creates new <see cref="Node"/>s
        /// </summary>
        public sealed class Factory
        {
            /// <summary>
            /// A factory for creating a method that takes in nothing and returns an array of <see cref="Manifest"/>s
            /// </summary>
            private readonly ProtocolMethodFactory<Nothing, Manifest[]> _nothingToManifestsFactory;

            /// <summary>
            /// A factory for creating a method that takes in a <see cref="Manifest"/> and returns an <see cref="int"/>
            /// </summary>
            private readonly ProtocolMethodFactory<Manifest, int> _manifestToIntFactory;

            /// <summary>
            /// A factory for creating a method that takes in a <see cref="Manifest"/> and returns an array of <see cref="Slice"/>s
            /// </summary>
            private readonly ProtocolMethodFactory<Manifest, Slice[]> _manifestToSlicesFactory;

            /// <summary>
            /// Creates a new <see cref="Factory"/> which creates <see cref="Node"/>s
            /// </summary>
            public Factory(
                ProtocolMethodFactory<Nothing, Manifest[]> nothingToManifestsFactory,
                ProtocolMethodFactory<Manifest, int> manifestToIntFactory,
                ProtocolMethodFactory<Manifest, Slice[]> manifestToSlicesFactory)
            {
                _nothingToManifestsFactory = nothingToManifestsFactory;
                _manifestToIntFactory = manifestToIntFactory;
                _manifestToSlicesFactory = manifestToSlicesFactory;
            }

            /// <summary>
            /// Attempts to create a new <see cref="Node"/> that will link the given <paramref name="storage"/> and <paramref name="protocol"/> together
            /// </summary>
            public bool TryCreate(Storage storage, IProtocol protocol, out Node node)
            {
                node = null;

                if (!_nothingToManifestsFactory.TryCreate(protocol, "Get manifests", new Handler<Nothing, Manifest[]>(_ => storage.ContainersForManifests.GetKeys().ToArray()), out var getManifestsMethod))
                    return false;
                if (!_manifestToIntFactory.TryCreate(protocol, "Get slice hashes for manifest", new Handler<Manifest, int>(manifest => !storage.ContainersForManifests.TryGet(manifest, out var container) ? 0 : container.GetKeys().Count()), out var getSliceHashesForMethod))
                    return false;
                if (!_manifestToSlicesFactory.TryCreate(protocol, "Get slices for manifest", new Handler<Manifest, Slice[]>(manifest => !storage.ContainersForManifests.TryCreate(manifest, out var container) ? null : container.GetValues().ToArray()), out var getSlicesForManifestMethod))
                    return false;

                node = new Node(
                    getManifestsMethod,
                    getSliceHashesForMethod,
                    getSlicesForManifestMethod
                );
                return true;
            }
        }

        private readonly IMethod<Nothing, Manifest[]> _getManifestsMethod;
        private readonly IMethod<Manifest, int> _getSliceCountForMethod;
        private readonly IMethod<Manifest, Slice[]> _getSlicesForMethod;

        internal Node(IMethod<Nothing, Manifest[]> getManifestsMethod, IMethod<Manifest, int> getSliceCountForMethod, IMethod<Manifest, Slice[]> getSlicesForMethod)
        {
            _getManifestsMethod = getManifestsMethod;
            _getSliceCountForMethod = getSliceCountForMethod;
            _getSlicesForMethod = getSlicesForMethod;
        }
        
        /// <summary>
        /// Retrieves all <see cref="Manifest"/>s from the other party
        /// </summary>
        public Task<Manifest[]> GetManifestsAsync()
        {
            var tcs = new TaskCompletionSource<Manifest[]>();
            _getManifestsMethod.Invoke(Nothing.Instance, tcs.SetResult);
            return tcs.Task;
        }

        /// <summary>
        /// Gets a count of all the <see cref="Slice"/>s for the given <paramref name="manifest"/>
        /// </summary>
        public Task<int> GetSliceCountFor(Manifest manifest)
        {
            var tcs = new TaskCompletionSource<int>();
            _getSliceCountForMethod.Invoke(manifest, tcs.SetResult);
            return tcs.Task;
        }

        /// <summary>
        /// Returns all the slices currently available for the given <paramref name="manifest"/>
        /// </summary>
        public Task<Slice[]> GetSlicesFor(Manifest manifest)
        {
            var tcs = new TaskCompletionSource<Slice[]>();
            _getSlicesForMethod.Invoke(manifest, tcs.SetResult);
            return tcs.Task;
        }

        /// <summary>
        /// Releases internal resources
        /// </summary>
        public void Dispose()
        {
            foreach (var disposable in new IDisposable[] { _getManifestsMethod, _getSliceCountForMethod, _getSlicesForMethod })
                disposable.Dispose();
        }
    }
}
