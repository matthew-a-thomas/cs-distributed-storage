namespace AspNet.Models
{
    /// <summary>
    /// The owner that controls this instance
    /// </summary>
    public sealed class Owner
    {
        public Owner(string identity)
        {
            Identity = identity;
        }

        public string Identity { get; }
    }
}
