namespace SecurityTests.Utils
{
    using Security;

    internal class NonsecureEntropy : IEntropy
    {
        /// <summary>
        /// Just returns an empty byte array of the given <paramref name="size"/>
        /// </summary>
        public byte[] CreateNonce(int size) => new byte[size];
    }
}
