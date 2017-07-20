namespace DistributedStorage.Authorization
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Something which knows how to create <see cref="AuthorizationToken"/>s from strings
    /// </summary>
    public sealed class StringAndAuthorizationTokenAdapter
    {
        private readonly Regex _regex = new Regex(@"^([^:]+):([^:]+):([^:]+):(.+)$");
        /*                                           ^ ID    ^ nonce ^ time  ^ hmac */

        /// <summary>
        /// Tries to create a new <see cref="AuthorizationToken"/> from the given string
        /// </summary>
        public bool TryCreateFromString(string s, out AuthorizationToken token)
        {
            token = null;

            // Make sure the value matches our regular expression
            var match = _regex.Match(s);
            if (!match.Success)
                return false;

            // Grab bits from the string
            string
                idBase64 = match.Groups[1].Value,
                nonceBase64 = match.Groups[2].Value,
                unixTimeString = match.Groups[3].Value,
                hmacBase64 = match.Groups[4].Value;
            long unixTime; // The client's reported unix time
            byte[]
                id, // The client's reported ID
                nonce, // A random value sent from the client
                hmac; // The client's reported request HMAC
            try
            {
                id = Convert.FromBase64String(idBase64);
                nonce = Convert.FromBase64String(nonceBase64);
                unixTime = long.Parse(unixTimeString);
                hmac = Convert.FromBase64String(hmacBase64);
            }
            catch
            {
                return false;
            }

            // Since the conversions happened successfully, create a new token
            token = new AuthorizationToken(id, hmac, nonce, unixTime);
            return true;
        }

        /// <summary>
        /// Creates a string that can be parsed by <see cref="TryCreateFromString"/>
        /// </summary>
        public string CreateFromToken(AuthorizationToken token) =>
            $@"{
                Convert.ToBase64String(token.Id)
            }:{
                Convert.ToBase64String(token.Nonce)
            }:{
                token.UnixTime
            }:{
                Convert.ToBase64String(token.Hmac)
            }";
    }
}
