namespace DistributedStorage.Networking.Http.Exceptions
{
    public sealed class UnauthorizedException : HttpException
    {
        public UnauthorizedException() : base(401, "You lack the authorization to perform this action") { }
    }
}
