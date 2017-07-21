namespace DistributedStorage.Networking.Http.Exceptions
{
    public sealed class ConflictException : HttpException
    {
        public ConflictException() : base(409, "This action cannot be performed due to the current state of the resource")
        {
        }
    }
}
