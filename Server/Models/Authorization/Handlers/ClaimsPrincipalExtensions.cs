namespace Server.Models.Authorization.Handlers
{
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {
        public static bool ClaimsToBeThisOwner(this ClaimsPrincipal claims, string owner) => claims.HasClaim(c => c.Type == ClaimTypes.Sid && c.Value == owner);
    }
}
