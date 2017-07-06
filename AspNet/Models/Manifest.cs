namespace AspNet.Models
{
    public sealed class Manifest
    {
        public Manifest(string id, int length, int numSlices, string[] sliceIds)
        {
            Id = id;
            Length = length;
            NumSlices = numSlices;
            SliceIds = sliceIds;
        }

        public string Id { get; }
        public int Length { get; }
        public int NumSlices { get; }
        public string[] SliceIds { get; }
    }
}
