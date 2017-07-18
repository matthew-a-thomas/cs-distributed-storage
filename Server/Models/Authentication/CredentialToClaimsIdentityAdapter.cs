namespace Server.Models.Authentication
{
    using System;
    using System.Security.Claims;
    using DistributedStorage.Authentication;

    public sealed class CredentialToClaimsIdentityAdapter
    {
        private readonly IdentityClaimFactory _identityClaimFactory;

        public CredentialToClaimsIdentityAdapter(IdentityClaimFactory identityClaimFactory)
        {
            _identityClaimFactory = identityClaimFactory;
        }

        /// <summary>
        /// Creates a new <see cref="ClaimsIdentity"/> for the given <paramref name="credential"/>
        /// </summary>
        public ClaimsIdentity CreateIdentityFor(Credential credential, string authenticationType)
        {
            // Create their claims.
            var claims = new[]
            {
                _identityClaimFactory.CreateClaim(Convert.ToBase64String(credential.Public))
            };
            // Create an identity for those claims and this middleware's authentication type.
            var identity = new ClaimsIdentity(claims, authenticationType);
            return identity;
        }
    }
}
