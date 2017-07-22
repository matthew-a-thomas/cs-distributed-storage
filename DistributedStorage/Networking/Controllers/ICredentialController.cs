namespace DistributedStorage.Networking.Controllers
{
    using System.Threading.Tasks;
    using Authentication;

    public interface ICredentialController
    {
        Task<Credential> GenerateCredentialAsync();
    }
}
