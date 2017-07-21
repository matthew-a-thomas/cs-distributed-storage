namespace DistributedStorage.Networking.Http.Exceptions
{
    public sealed class NotFoundException : HttpException
    {
        public NotFoundException() : base(404, "The resource was not found") { }
    }
}
