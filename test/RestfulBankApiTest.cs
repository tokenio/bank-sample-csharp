using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Tokenio.BankSample.Asserts;
using Tokenio.BankSample.Common;
using Tokenio.BankSample.Restful;
using Tokenio.BankSample.Utils;
using Tokenio.Proto.Common.ConsentProtos;
using Tokenio.Proto.Common.MemberProtos;
using Tokenio.Proto.Common.SecurityProtos;
using Tokenio.Proto.Gateway;
using Tokenio.Utils;
using Xunit;
using Xunit.Abstractions;
using static Tokenio.Proto.Common.ConsentProtos.CreateConsent.Types;

namespace Tokenio.BankSample
{
    public class RestfulBankApiTest
    {
        public RestfulBankApiTest(ITestOutputHelper helper)
        {
            testOutHelper = helper;
            tppMember = tppRule.Member();
            tppMember.SetProfile(
                new Profile
                {
                    DisplayNameFirst = "South",
                    DisplayNameLast = "Side"
                });
            accounts = bankBearerAuthRule.CreateAccount(2);
            accountIdentifiers =
                accounts.Select(
                    account => Transformer.ToAccountIndentifier(account))
                    .ToList();
            displayNames = accounts.Select(account => account.GetDisplayName())
                .ToList();
        }

        public TokenRestfulBankRule bankBearerAuthRule =
            new TokenRestfulBankRule(AuthorizationType.Bearer);

        public TokenTppRule tppRule = new TokenTppRule();
        private readonly Tpp.Member tppMember;
        private readonly ITestOutputHelper testOutHelper;
        private readonly IList<NamedAccount> accounts;
        private readonly IList<string> accountIdentifiers;
        private readonly IList<string> displayNames;

        private void TestResourceTypeAccessConsent(BankClient bankClient)
        {
            var userId = bankClient.CreateUser();
            var tokenRequest =
                Sample.ResourceTypeAccessTokenRequest(
                    tppMember.MemberId(),
                    bankClient.GetBankId());
            var requestId = tppMember.StoreTokenRequestBlocking(tokenRequest);

            // Bank receives the request Id.
            var consentRequest = bankClient.GetConsentRequest(requestId);

            testOutHelper.WriteLine(consentRequest.ToString());

            ConsentRequestAssertion.AssertConsentRequest(consentRequest)
                .MatchesTokenRequest(tokenRequest);

            // Bank creates consents with selected accounts.
            var request = new CreateConsentRequest
            {
                Params = new CreateConsent
                {
                    RequestId = requestId,
                    UserId = userId,
                    ResourceTypeAccess = new ResourceTypeAccess
                    {
                        AccountIdentifiers = {accountIdentifiers}
                    }
                }
            };
            testOutHelper.WriteLine(request.ToString());

            var consent = bankClient.CreateConsent(Util.ToJson(request));
            testOutHelper.WriteLine(consent.ToString());

            ConsentAssertion.AssertConsent(consent)
                .IsFromUser(userId)
                .HasInformationAccess(
                    bankClient.GetBankId(),
                    accountIdentifiers,
                    displayNames,
                    consentRequest.ResourceTypeList);

            // TPP redeems the token.
            var result = tppRule.Token()
                .GetTokenRequestResult(requestId)
                .Result;
            Assert.Equal(consent.Id, result.TokenId);
            Assert.NotNull(result.Signature);
            var representable = tppMember.ForAccessToken(result.TokenId);
            var userAccounts = representable.GetAccountsBlocking();
            var userNames =
                userAccounts.Select(userAccount => userAccount.Name())
                    .ToList();
            userNames.Should()
                .BeEquivalentTo(displayNames);
            var money =
                userAccounts[0].GetBalanceBlocking(Key.Types.Level.Standard)
                    .Current;
            Assert.NotNull(money.Currency);
            Assert.NotNull(money.Value);
            bankClient.CancelConsent(consent.Id);
        }

        private void TestDeleteBankUser(BankClient bankClient)
        {
            var userId = bankClient.CreateUser();
            bankClient.DeleteUser(userId);
        }

        [Fact]
        public void DeleteBankUser()
        {
            TestDeleteBankUser(bankBearerAuthRule.GetBankClient());
        }

        [Fact]
        public void ResourceTypeAccessConsent()
        {
            TestResourceTypeAccessConsent(bankBearerAuthRule.GetBankClient());
        }
    }
}
