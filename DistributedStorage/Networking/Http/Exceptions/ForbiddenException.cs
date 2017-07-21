namespace DistributedStorage.Networking.Http.Exceptions
{
    public sealed class ForbiddenException : HttpException
    {
        public ForbiddenException() : base(403, "This action is not allowed right now")
        {
        }
    }
}
