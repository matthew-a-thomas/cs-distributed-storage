namespace DistributedStorage.Model
{
    public interface IAuthenticator<in TIdentity>
        where TIdentity : IIdentity
    {
        AuthenticationResult Authenticate(TIdentity identity);
    }
}
