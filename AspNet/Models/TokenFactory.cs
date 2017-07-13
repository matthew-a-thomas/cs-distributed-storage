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
            var secret = _secretRepository.GetCachedSecret();
            if (secret == null)
                return null;
            using (var rng = RandomNumberGenerator.Create())
            {
                var idBytes = new byte[8];
                rng.GetBytes(idBytes);
                var id = Convert.ToBase64String(idBytes);
                var claims = new Dictionary<string, string>
                {
                    { "id", id}
                };
                var token = claims.ToToken(secret);
                return token;
            }
        }
    }
}
