namespace DistributedStorage.Networking.Security
{
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    /// <summary>
    /// Something that can be used for a protocol which communicates public keys between two parties along with proof that both parties own their corresponding private key
    /// </summary>
    public sealed class RsaKeySwapper
    {
        /// <summary>
        /// Performs RSA crypto stuff for us
        /// </summary>
        private readonly ICryptoRsa _cryptoRsa;

        /// <summary>
        /// Creates a new <see cref="RsaKeySwapper"/> that uses the given RSA crypto
        /// </summary>
        public RsaKeySwapper(ICryptoRsa cryptoRsa)
        {
            _cryptoRsa = cryptoRsa;
        }

        /// <summary>
        /// Sends our public key and our challenge
        /// </summary>
        public void SendChallenge(Stream stream, RSAParameters ours, byte[] ourChallenge)
        {
            stream.Write(ours, false);
            stream.Write(ourChallenge);
        }

        /// <summary>
        /// Receives their public key and their challenge
        /// </summary>
        public bool TryReceiveChallenge(Stream stream, out RSAParameters theirs, out byte[] theirChallenge)
        {
            theirChallenge = null;
            return
                stream.TryRead(out theirs)
                &&
                stream.TryRead(out theirChallenge);
        }

        /// <summary>
        /// Sends our proof of owning our private key by signing the combination of both challenges
        /// </summary>
        public void SendChallengeResponse(Stream stream, RSAParameters ours, byte[] theirChallenge, byte[] ourChallenge)
        {
            var mixed = (byte[])theirChallenge.Clone();
            mixed.Xor(ourChallenge);
            var proof = _cryptoRsa.Sign(mixed, ours);
            
            stream.Write(proof);
        }

        /// <summary>
        /// Receives their signature of the combination of both challenges, returning true if it is valid
        /// </summary>
        public bool TryReceiveChallengeResponse(Stream stream, byte[] ourChallenge, byte[] theirChallenge, RSAParameters theirs)
        {
            if (!stream.TryRead(out byte[] theirProof))
                return false;

            var mixed = (byte[])ourChallenge.Clone();
            mixed.Xor(theirChallenge);

            return _cryptoRsa.Verify(mixed, theirProof, theirs);
        }
    }
}
