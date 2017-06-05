namespace DistributedStorage.Networking.Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;

    public static class RsaKeySwapperExtensions
    {
        /// <summary>
        /// Uses the given stream to write out the public part of our RSA key, and read in and return the public part of their RSA key.
        /// This method also verifies that the sending party owns the private key for the public key they're sending.
        /// It does this by also swapping nonces, and signing/verifying them
        /// </summary>
        public static bool TrySwapPublicRsaKeys(this RsaKeySwapper @this, Stream underlyingStream, RSAParameters ours, TimeSpan timeout, IEntropy entropy, out RSAParameters theirs)
        {
            // Send our challenge
            var ourChallenge = entropy.CreateNonce(ours.Modulus.Length);
            @this.SendChallenge(underlyingStream, ours, ourChallenge);

            // Receive their challenge
            var start = Stopwatch.StartNew();
            if (!@this.TryReceiveChallenge(underlyingStream, timeout, out theirs, out var theirChallenge))
                return false;

            // Send our challenge response
            @this.SendChallengeResponse(underlyingStream, ours, theirChallenge, ourChallenge);

            // Receive and validate their challenge response
            return @this.TryReceiveChallengeResponse(underlyingStream, ourChallenge, theirChallenge, theirs, timeout - start.Elapsed);
        }
    }
}
