namespace DistributedStorage.Model
{
    public sealed class Authenticator<TIdentity> : IAuthenticator<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly AuthenticateDelegate _authenticateDelegate;

        public delegate AuthenticationResult AuthenticateDelegate(TIdentity identity);

        public Authenticator(AuthenticateDelegate authenticateDelegate)
        {
            _authenticateDelegate = authenticateDelegate;
        }

        public AuthenticationResult Authenticate(TIdentity identity) => _authenticateDelegate(identity);
    }
}
