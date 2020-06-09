using System;
using System.Text;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.AliasProtos;
using Tokenio.Proto.Common.SecurityProtos;
using Tokenio.Proto.Common.TokenProtos;
using Tokenio.Proto.Common.TransferInstructionsProtos;
using TokenRequest = Tokenio.TokenRequests.TokenRequest;

namespace Tokenio.BankSample.Common
{
    public abstract class Sample
    {
        private static readonly string TOKEN_REALM = "token";
        private static readonly double AMOUNT = 30.1;
        private static readonly string CURRENCY = "EUR";

        private static readonly string TPP_CALLBACK =
            "https://merchant-demo.dev.token.io/callback";

        public static string RandomAlphaNumeric(int size)
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty)
                .Substring(0, size);
        }

        private static string RandomAlphabetic(int size, bool lowerCase)
        {
            var randomBuilder = new StringBuilder();
            var random = new Random();
            char ch;
            for (var i = 0; i < size; i++)
            {
                ch =
                    Convert.ToChar(
                        Convert.ToInt32(
                            Math.Floor(26 * random.NextDouble() + 65)));
                randomBuilder.Append(ch);
            }
            if (lowerCase)
                return randomBuilder.ToString()
                    .ToLower();
            return randomBuilder.ToString();
        }

        public static TransferEndpoint TransferEndpoint()
        {
            return new TransferEndpoint
            {
                Account = BankAccount()
            };
        }

        public static BankAccount BankAccount()
        {
            return new BankAccount
            {
                Token = new BankAccount.Types.Token
                {
                    AccountId = RandomAlphaNumeric(15),
                    MemberId = RandomAlphaNumeric(15)
                }
            };
        }

        public static Alias alias()
        {
            return alias(true);
        }

        public static Alias alias(bool noVerify)
        {
            var suffix = noVerify
                ? "+noverify@example.com"
                : "@example.com";
            return new Alias
            {
                Value = RandomAlphaNumeric(15)
                    .ToLower() + suffix,
                Type = Alias.Types.Type.Email,
                Realm = TOKEN_REALM
            };
        }

        public static Alias DomainAlias()
        {
            return new Alias
            {
                Value = RandomAlphaNumeric(15)
                    .ToLower() + ".noverify",
                Type = Alias.Types.Type.Domain,
                Realm = TOKEN_REALM
            };
        }

        public static TokenMember TokenMember()
        {
            return new TokenMember
            {
                Id = RandomAlphaNumeric(15)
            };
        }

        public static TransferInstructions TransferInstructions()
        {
            var instructions = new TransferInstructions
            {
                Source = TransferEndpoint()
            };
            instructions.Destinations.Add(TransferEndpoint());
            return instructions;
        }

        public static Signature Signature()
        {
            return new Signature
            {
                MemberId = RandomAlphaNumeric(15),
                Signature_ = RandomAlphaNumeric(15),
                KeyId = RandomAlphaNumeric(15)
            };
        }

        public static TokenPayload BankTransfer()
        {
            var now = new DateTime();
            var span = TimeSpan.FromMinutes(1);
            var redeemer = TokenMember();
            return new TokenPayload
            {
                Version = "1.0",
                RefId = RandomAlphaNumeric(15),
                Issuer = TokenMember(),
                From = TokenMember(),
                EffectiveAtMs = now.Add(span)
                    .Millisecond,
                ExpiresAtMs = now.Subtract(span)
                    .Millisecond,
                Description = RandomAlphaNumeric(10),
                Transfer = new TransferBody
                {
                    Redeemer = redeemer,
                    Instructions = TransferInstructions(),
                    Currency = "USD",
                    LifetimeAmount = "100.50",
                    Amount = "100.50"
                }
            };
        }

        public static TokenRequest ResourceTypeAccessTokenRequest(
            string tppMemberId,
            string bankId)
        {
            return TokenRequest.
                AccessTokenRequestBuilder(
                    TokenRequestPayload.Types.AccessBody.Types.ResourceType
                        .Accounts,
                    TokenRequestPayload.Types.AccessBody.Types.ResourceType
                        .Balances)
                .SetRefId(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetUserRefId(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetRedirectUrl(TPP_CALLBACK)
                .SetToMemberId(tppMemberId)
                .SetDescription(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetCsrfToken(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetBankId(bankId)
                .SetReceiptRequested(false)
                .SetState(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .Build();
        }

        public static TransferDestination SwiftDestination()
        {
            return new TransferDestination
            {
                Swift = new TransferDestination.Types.Swift
                {
                    Bic = RandomAlphabetic(15, true),
                    Account = RandomAlphabetic(15, true)
                }
            };
        }

        public static TokenRequest TransferTokenRequest(string tppMemberId,
            string bankId)
        {
            return TokenRequest.TransferTokenRequestBuilder(AMOUNT, CURRENCY)
                .SetRefId(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetUserRefId(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetRedirectUrl(TPP_CALLBACK)
                .SetToMemberId(tppMemberId)
                .SetDescription(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetCsrfToken(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .SetBankId(bankId)
                .SetReceiptRequested(false)
                .AddDestination(SwiftDestination())
                .SetState(
                    RandomAlphaNumeric(15)
                        .ToLower())
                .Build();
        }
    }
}
