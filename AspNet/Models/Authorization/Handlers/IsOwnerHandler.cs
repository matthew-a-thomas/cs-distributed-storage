namespace AspNet.Models.Authorization.Handlers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Requirements;

    public sealed class IsOwnerHandler : AuthorizationHandler<IsOwnerRequirement>
    {
        private readonly OwnerRepository _ownerRepository;

        public IsOwnerHandler(OwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement)
        {
            if (_ownerRepository.TryGetOwner(out var currentOwner) && context.User.ClaimsToBeThisOwner(currentOwner))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
