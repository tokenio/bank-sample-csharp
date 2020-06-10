using System;
using System.Security.Cryptography;

namespace Tokenio.BankSample.Utils
{
    public class TestUtil
    {
        /// <summary>
        ///     Randoms the Alphabet
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string RandomAlphabetic(int size)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bit_count = size * 6;
                var byte_count = (bit_count + 7) / 8; // rounded up
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
