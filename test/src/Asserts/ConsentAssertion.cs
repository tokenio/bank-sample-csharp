using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Tokenio.BankSample.Common;
using Tokenio.BankSample.Utils;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.ConsentProtos;
using Tokenio.Proto.Common.TokenProtos;
using Xunit;
using BodyCase = Tokenio.Proto.Common.TokenProtos.TokenPayload.BodyOneofCase;
using TypeCase = Tokenio.Proto.Common.ConsentProtos.Consent.TypeOneofCase;

namespace Tokenio.BankSample.Asserts
{
    public sealed class ConsentAssertion : Assert
    {
        private readonly Consent actual;

        private ConsentAssertion(Consent actual)
        {
            this.actual = actual;
        }

        public static ConsentAssertion AssertConsent(Consent consent)
        {
            return new ConsentAssertion(consent);
        }

        public ConsentAssertion IsFromUser(string userMemberId)
        {
            Equal(userMemberId, actual.MemberId);
            return this;
        }

        public void AssertToken(Token token)
        {
            Equal(actual.Id, token.Id);
            Equal(actual.Beneficiary.Member.Id, token.Payload.To.Id);
            Equal(actual.MemberId, token.Payload.From.Id);
            if (actual.TypeCase == TypeCase.InformationAccess)
                Equal(BodyCase.Access, token.Payload.BodyCase);
            else if (actual.TypeCase == TypeCase.Payment)
                Equal(BodyCase.Transfer, token.Payload.BodyCase);
            else
                throw new ArgumentException(
                    "Consent type \"" + actual.TypeCase + "\" is invalid.");
        }

        public ConsentAssertion HasInformationAccess(string bankId,
            IList<string> accountIdentifiers,
            IList<string> displayNames,
            TokenRequestPayload.Types.AccessBody.Types.ResourceTypeList
                resourceTypeList)
        {
            IList<NamedAccount> accounts =
                Enumerable.Range(0, accountIdentifiers.Count)
                    .Select(
                        index =>
                            new NamedAccount(
                                bankId,
                                accountIdentifiers[index],
                                displayNames[index]))
                    .ToList();
            return HasInformationAccess(accounts, resourceTypeList);
        }

        public ConsentAssertion HasInformationAccess(
            IList<NamedAccount> accounts,
            TokenRequestPayload.Types.AccessBody.Types.ResourceTypeList
                resourceTypeList)
        {
            var expectedTypes =
                resourceTypeList.Resources.Select(
                    resource => Transformer.ToConsentResourceType(resource))
                    .ToHashSet();
            var resourceAccounts = new HashSet<BankAccount>();
            foreach (var access in actual.InformationAccess.ResourceAccess)
            {
                resourceAccounts.Add(access.Account);
                expectedTypes.Should()
                    .BeEquivalentTo(access.Resources);
            }
            var expectedAccounts =
                accounts.Select(account => account.GetBankAccount())
                    .ToHashSet();
            expectedAccounts.Should()
                .BeEquivalentTo(resourceAccounts);
            return this;
        }

        private bool IsEqual(BankAccount bankAccount1, BankAccount bankAccount2)
        {
            return Normalize(bankAccount1)
                .Equals(Normalize(bankAccount2));
        }

        private BankAccount Normalize(BankAccount bankAccount)
        {
            bankAccount.AccountFeatures = null;
            bankAccount.Metadata.Clear();
            return bankAccount;
        }
    }
}
