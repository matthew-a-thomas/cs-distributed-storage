namespace AspNet.Models.Authorization.Handlers
{
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {
        public static bool ClaimsToBeThisOwner(this ClaimsPrincipal claims, Owner owner) => claims.HasClaim(c => c.Type == ClaimTypes.Email && c.Value == owner.Identity);
    }
}
