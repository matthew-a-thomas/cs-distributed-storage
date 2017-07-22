namespace DistributedStorage.Networking.Controllers
{
    using System.Threading.Tasks;
    using Authentication;
    using Http;

    public interface ICredentialController
    {
        Task<StatusResponse<Credential>> GenerateCredentialAsync();
    }
}
