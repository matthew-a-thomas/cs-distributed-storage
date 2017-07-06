namespace AspNet.Models
{
    public sealed class Manifest
    {
        public Manifest(string id, int length, int numSlices)
        {
            Id = id;
            Length = length;
            NumSlices = numSlices;
        }

        public string Id { get; }
        public int Length { get; }
        public int NumSlices { get; }
    }
}
