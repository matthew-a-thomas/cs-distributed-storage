namespace Server.Models.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;

    public sealed class IsOwnerRequirement : IAuthorizationRequirement
    { }
}
