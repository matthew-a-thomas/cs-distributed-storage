namespace DistributedStorage.Networking.Controllers
{
    using System.Threading.Tasks;
    using Http;

    public interface IOwnerController
    {
        Task<StatusResponse<string>> GetOwnerAsync();
        Task<StatusResponse<bool>> PutOwnerAsync(string owner);
    }
}
