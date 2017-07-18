namespace Server.Models.Authentication
{
    using System.Security.Cryptography;
    using DistributedStorage.Authentication;

    public sealed class CredentialFactory
    {
        private readonly SecretRepository _secretRepository;

        public CredentialFactory(SecretRepository secretRepository)
        {
            _secretRepository = secretRepository;
        }

        /// <summary>
        /// Creates a new <see cref="Credential"/> by randomly generating some public bytes, then signing them with this server's secret to create the private bytes
        /// </summary>
        public Credential CreateNewCredential()
        {
            var serverSecret = _secretRepository.GetCachedSecret();
            if (serverSecret == null)
                return null;
            using (var rng = RandomNumberGenerator.Create())
            {
                var publicBytes = new byte[256 / 8];
                rng.GetBytes(publicBytes);
                byte[] privateBytes;
                using (var hmacer = new HMACSHA256(serverSecret))
                {
                    privateBytes = hmacer.ComputeHash(publicBytes);
                }
                return new Credential(publicBytes, privateBytes);
            }
        }
    }
}
