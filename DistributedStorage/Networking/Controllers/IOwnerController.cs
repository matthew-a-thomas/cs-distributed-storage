namespace DistributedStorage.Networking.Controllers
{
    using System.Threading.Tasks;

    public interface IOwnerController
    {
        Task<string> GetOwnerAsync();
        Task<bool> PutOwnerAsync(string owner);
    }
}
