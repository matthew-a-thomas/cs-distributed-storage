namespace Security
{
    using System;
    using Common;

    /// <summary>
    /// An implementation of <see cref="ICryptoSymmetric"/> that uses AES-256 behind the scenes and is resistant to replay attacks
    /// </summary>
    public sealed class CryptoSymmetric : ICryptoSymmetric
    {
        /// <summary>
        /// Something that can create instances of <see cref="CryptoSymmetric"/>
        /// </summary>
        public sealed class Factory
        {
            /// <summary>
            /// Creates a new <see cref="CryptoSymmetric"/> that has the given <paramref name="replayTolerance"/>
            /// </summary>
            public CryptoSymmetric Create(TimeSpan replayTolerance) => new CryptoSymmetric(replayTolerance);
        }

        /// <summary>
        /// The amount of time we will tolerate between our system's time and the timestamp in a decrypted message
        /// </summary>
        private readonly TimeSpan _replayTolerance;

        /// <summary>
        /// Something that helps us detect replays in plaintexts
        /// </summary>
        private readonly ReplayDetector<Hash> _replayDetector = new ReplayDetector<Hash>();

        /// <summary>
        /// Creates a new <see cref="ICryptoSymmetric"/> which tolerates the given difference between current system time and the timestamp reported in an encrypted message
        /// </summary>
        public CryptoSymmetric(TimeSpan replayTolerance)
        {
            _replayTolerance = replayTolerance;
        }

        /// <summary>
        /// Returns a cryptographically-secure random array of 256/8 bytes
        /// </summary>
        public byte[] CreateAesKey() => Crypto.CreateAesKey();

        /// <summary>
        /// Performs SHA-256 on the given <paramref name="key"/> to turn it into a key that can be used for AES-256
        /// </summary>
        public byte[] ConvertToAesKey(byte[] key) => Crypto.ConvertToAesKey(key);

        /// <summary>
        /// Performs AES encryption and HMAC on the given <paramref name="plaintext"/> using the given <paramref name="key"/>, an AES-256 cipher, and a SHA512 HMAC
        /// </summary>
        public byte[] EncryptAndHmac(byte[] plaintext, byte[] key) => Crypto.EncryptAes(plaintext, key);

        /// <summary>
        /// Performs AES decryption and HMAC validation of the given <paramref name="ciphertext"/> using the given <paramref name="key"/>, an AES-256 algorithm, and a SHA512 HMAC
        /// </summary>
        public bool TryVerifyHmacAndDecrypt(byte[] ciphertext, byte[] key, out byte[] plaintext)
        {
            if (!Crypto.TryDecryptAes(ciphertext, key, out plaintext, out var ticksUtc))
                return false;
            var hash = Hash.Create(plaintext);
            _replayDetector.Clean((DateTime.UtcNow - _replayTolerance).Ticks);
            return _replayDetector.TryAdd(hash, ticksUtc);
        }
    }
}
