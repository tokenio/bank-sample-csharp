using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Tokenio.Proto.Common.SecurityProtos;
using Tokenio.Security;
using Tokenio.Security.Keystore;

namespace Tokenio.BankSample.Utils
{
    internal static class TestKeyUtils
    {
        public static TrustedKey ToTrustedKey(string publicKey, string type)
        {
            var pubKey = Base64UrlEncoder.DecodeBytes(publicKey);
            return
                TrustedKey.Create(
                    type == "EDDSA"
                        ? Key.Types.Algorithm.Ed25519
                        : Key.Types.Algorithm.InvalidAlgorithm,
                    pubKey);
        }

        public static SecretKeyPair ToSecretKeyPair(string publicKey,
            string privateKey,
            string type)
        {
            var pubKey = Base64UrlEncoder.DecodeBytes(publicKey);
            var priKey = Base64UrlEncoder.DecodeBytes(privateKey);

            var id = Base64UrlEncoder.Encode(
                SHA256.Create()
                    .ComputeHash(pubKey))
                .Substring(0, 16);
            var key = new KeyPair(
                id,
                Key.Types.Level.Standard,
                type == "EDDSA"
                    ? Key.Types.Algorithm.Ed25519
                    : Key.Types.Algorithm.InvalidAlgorithm,
                priKey,
                pubKey);
            return SecretKeyPair.Create(
                true,
                type == "EDDSA"
                    ? Key.Types.Algorithm.Ed25519
                    : Key.Types.Algorithm.InvalidAlgorithm,
                key);
        }
    }
}
