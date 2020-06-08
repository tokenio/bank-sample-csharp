using System;
using Tokenio.Security.Keystore;
using Xunit;

namespace Tokenio.BankSample
{
    public class GenerateKeyTest
    {
        [Fact]
        public void Generate()
        {
            SecretKeyPair keyPair = SecretKeyPair.Create();
            Console.WriteLine("crypto: EDDSA");
            Console.WriteLine("private-key: "
                    + keyPair.PrivateKeyString()
                    + " // Used for signing bank auth payloads");
            Console.WriteLine("public-key: "
                    + keyPair.PublicKeyString()
                    + "  // Give to Token so that Token can verify bank auth payloads");
        }
    }
}