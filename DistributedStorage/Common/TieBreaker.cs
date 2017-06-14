namespace DistributedStorage.Common
{
    using System;
    using Networking.Security;

    /// <summary>
    /// Something that knows how to deal with ties
    /// </summary>
    public sealed class TieBreaker
    {
        /// <summary>
        /// The result of trying to break a tie
        /// </summary>
        public enum Result
        {
            /// <summary>
            /// You won the tie break
            /// </summary>
            YouWon,

            /// <summary>
            /// They won the tie break
            /// </summary>
            TheyWon,

            /// <summary>
            /// You both tied
            /// </summary>
            Tie
        }

        private readonly IEntropy _entropy;
        private readonly Action<int> _sendOurValue;
        private readonly Func<int> _getTheirValue;

        /// <summary>
        /// Creates a new <see cref="TieBreaker"/>, which knows how to deal with ties
        /// </summary>
        /// <param name="entropy">Your source of entropy, from which random numbers will be drawn</param>
        /// <param name="sendOurValue">Your way of communicating your number to the other party</param>
        /// <param name="getTheirValue">Your way of receiving a number from the other party</param>
        public TieBreaker(IEntropy entropy, Action<int> sendOurValue, Func<int> getTheirValue)
        {
            _entropy = entropy;
            _sendOurValue = sendOurValue;
            _getTheirValue = getTheirValue;
        }

        /// <summary>
        /// Breaks a tie by generating a random number, swapping it with the other party (through the methods given to the constructor), and comparing your number with theirs.
        /// The party with the larger number wins, and it's a tie if both numbers are the same size
        /// </summary>
        public Result Test()
        {
            var ourValue = _entropy.NextInteger();
            _sendOurValue(ourValue);
            var theirValue = _getTheirValue();
            return
                ourValue > theirValue
                ? Result.YouWon
                : (theirValue > ourValue
                ? Result.TheyWon
                : Result.Tie);
        }
    }
}
