namespace AspNet.Models
{
    public sealed class Slice
    {
        public Slice(bool[] coefficients, string id, byte[] data)
        {
            Coefficients = coefficients;
            Id = id;
            Data = data;
        }

        public bool[] Coefficients { get; }
        public string Id { get; }
        public byte[] Data { get; }
    }
}
