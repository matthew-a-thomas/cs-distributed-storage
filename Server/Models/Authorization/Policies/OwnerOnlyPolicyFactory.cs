namespace Server.Models.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;
    using Requirements;

    /// <summary>
    /// Creates policies having an <see cref="IsOwnerRequirement"/> requirement
    /// </summary>
    public sealed class OwnerOnlyPolicyFactory
    {
        /// <summary>
        /// The name of the policies created by the <see cref="OwnerOnlyPolicyFactory"/>
        /// </summary>
        public const string PolicyName = "OwnerOnly";

        private readonly IsOwnerRequirement _requirement;

        public OwnerOnlyPolicyFactory() => _requirement = new IsOwnerRequirement();

        public AuthorizationPolicy CreatePolicy()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.AddRequirements(_requirement);
            var policy = policyBuilder.Build();
            return policy;
        }
    }
}
