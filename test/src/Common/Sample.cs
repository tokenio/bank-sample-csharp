using System;
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

        private static readonly string TPP_CALLBACK =
            "https://merchant-demo.dev.token.io/callback";

        private static string RandomAlphabetic(int size)
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty)
                .Substring(0, size);
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
                    AccountId = RandomAlphabetic(15),
                    MemberId = RandomAlphabetic(15)
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
                Value = RandomAlphabetic(15)
                    .ToLower() + suffix,
                Type = Alias.Types.Type.Email,
                Realm = TOKEN_REALM
            };
        }

        public static Alias DomainAlias()
        {
            return new Alias
            {
                Value = RandomAlphabetic(15)
                    .ToLower() + ".noverify",
                Type = Alias.Types.Type.Domain,
                Realm = TOKEN_REALM
            };
        }

        public static TokenMember TokenMember()
        {
            return new TokenMember
            {
                Id = RandomAlphabetic(15)
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
                MemberId = RandomAlphabetic(15),
                Signature_ = RandomAlphabetic(15),
                KeyId = RandomAlphabetic(15)
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
                RefId = RandomAlphabetic(15),
                Issuer = TokenMember(),
                From = TokenMember(),
                EffectiveAtMs = now.Add(span)
                    .Millisecond,
                ExpiresAtMs = now.Subtract(span)
                    .Millisecond,
                Description = RandomAlphabetic(10),
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
                    RandomAlphabetic(15)
                        .ToLower())
                .SetUserRefId(
                    RandomAlphabetic(15)
                        .ToLower())
                .SetRedirectUrl(TPP_CALLBACK)
                .SetToMemberId(tppMemberId)
                .SetDescription(
                    RandomAlphabetic(15)
                        .ToLower())
                .SetCsrfToken(
                    RandomAlphabetic(15)
                        .ToLower())
                .SetBankId(bankId)
                .SetReceiptRequested(false)
                .SetState(
                    RandomAlphabetic(15)
                        .ToLower())
                .Build();
        }
    }
}
