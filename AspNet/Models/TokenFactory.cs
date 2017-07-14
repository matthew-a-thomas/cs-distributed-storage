namespace AspNet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Authorization;

    public class TokenFactory
    {
        private readonly SecretRepository _secretRepository;

        public TokenFactory(SecretRepository secretRepository)
        {
            _secretRepository = secretRepository;
        }

        public Token GenerateNewToken()
        {
            var serverSecret = _secretRepository.GetCachedSecret();
            if (serverSecret == null)
                return null;
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenPublicBytes = new byte[256 / 8];
                rng.GetBytes(tokenPublicBytes);
                byte[] tokenSecretBytes;
                using (var hmacer = new HMACSHA256(serverSecret))
                {
                    tokenSecretBytes = hmacer.ComputeHash(tokenPublicBytes);
                }

                var tokenSecret = Convert.ToBase64String(tokenSecretBytes);
                var tokenPublic = Convert.ToBase64String(tokenPublicBytes);

                var claims = new Dictionary<string, string>
                {
                    { "public", tokenPublic },
                    { "secret", tokenSecret }
                };
                var token = claims.ToToken(serverSecret);
                return token;
            }
        }
    }
}
