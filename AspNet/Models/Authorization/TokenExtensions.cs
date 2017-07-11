namespace AspNet.Models.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class TokenExtensions
    {
        /// <summary>
        /// Determines if this <see cref="Token"/> is valid
        /// by verifying its HMAC and then performing the given <paramref name="finalCheck"/> on it
        /// </summary>
        public static bool IsValid(this Token token, byte[] key, Func<Token, bool> finalCheck)
        {
            // Compute the HMAC by serializing out this token
            byte[] hmac;
            using (var stream = new MemoryStream())
            {
                // Serialize the claims
                foreach (var claim in token.Claims.SelectMany(x => new[] { x.Key, x.Value }))
                {
                    var claimBytes = Encoding.UTF8.GetBytes(claim);
                    stream.Write(claimBytes, 0, claimBytes.Length);
                }

                // Serialize the token's time
                var timeBytes = BitConverter.GetBytes(token.UnixTimestamp);
                stream.Write(timeBytes, 0, timeBytes.Length);

                // Serialize the token's nonce
                stream.Write(token.Nonce, 0, token.Nonce.Length);

                // Compute the HMAC
                stream.Position = 0;
                using (var hmacer = new HMACSHA256(key))
                    hmac = hmacer.ComputeHash(stream);
            }

            // Check if the HMAC matches
            if (!hmac.SequenceEqual(token.Hmac))
                return false;

            // Otherwise, run the final check
            var resultOfFinalCheck = finalCheck(token);

            return resultOfFinalCheck;
        }

        /// <summary>
        /// Creates a new <see cref="Token"/> from these given <paramref name="claims"/> using the given HMAC <paramref name="key"/>
        /// </summary>
        public static Token ToToken(this IReadOnlyDictionary<string, string> claims, byte[] key)
        {
            // Serialize out the stuff that will go into the generated token
            byte[] nonce;
            long unixTimestamp;
            byte[] hmac;
            using (var stream = new MemoryStream())
            {
                // Serialize the claims
                foreach (var claim in claims.SelectMany(x => new[] { x.Key, x.Value }))
                {
                    var claimBytes = Encoding.UTF8.GetBytes(claim);
                    stream.Write(claimBytes, 0, claimBytes.Length);
                }

                // Serialize the current time
                unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var nowBytes = BitConverter.GetBytes(unixTimestamp);
                stream.Write(nowBytes, 0, nowBytes.Length);

                // Serialize the nonce
                nonce = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(nonce);
                stream.Write(nonce, 0, nonce.Length);

                // Compute the HMAC
                stream.Position = 0;
                using (var hmacer = new HMACSHA256(key))
                    hmac = hmacer.ComputeHash(stream);
            }

            // Now create a token
            return new Token(claims, hmac, nonce, unixTimestamp);
        }
    }
}
