using System;
using Tokenio.BankSample.Utils;
using ResourceType =
    Tokenio.Proto.Common.TokenProtos.TokenRequestPayload.Types.AccessBody.Types.
        ResourceType;
using ConsentResourceType =
    Tokenio.Proto.Common.ConsentProtos.Consent.Types.InformationAccess.Types.
        ResourceAccess.Types.ResourceType;
using AccountResourceType =
    Tokenio.Proto.Common.TokenProtos.TokenRequestPayload.Types.AccessBody.Types.
        AccountResourceType;

namespace Tokenio.BankSample.Common
{
    public class Transformer
    {
        public static ConsentResourceType ToConsentResourceType(
            ResourceType type)
        {
            switch (type)
            {
            case ResourceType.Accounts:
                return ConsentResourceType.Account;
            case ResourceType.Balances:
                return ConsentResourceType.Balance;
            case ResourceType.Transactions:
                return ConsentResourceType.Transactions;
            case ResourceType.StandingOrders:
                return ConsentResourceType.StandingOrders;
            case ResourceType.TransferDestinations:
                return ConsentResourceType.TransferDestinations;
            default:
                throw new ArgumentException(
                    "Unexpected Resource Type:{0}",
                    type.ToString());
            }
        }

        public static ConsentResourceType ToConsentResourceType(
            AccountResourceType type)
        {
            switch (type)
            {
            case AccountResourceType.AccountInfo:
                return ConsentResourceType.Account;
            case AccountResourceType.AccountBalance:
                return ConsentResourceType.Balance;
            case AccountResourceType.AccountTransactions:
                return ConsentResourceType.Transactions;
            case AccountResourceType.AccountStandingOrders:
                return ConsentResourceType.StandingOrders;
            case AccountResourceType.AccountTransferDestinations:
                return ConsentResourceType.TransferDestinations;
            case AccountResourceType.AccountFundsConfirmation:
                return ConsentResourceType.FundsConfirmations;
            default:
                throw new ArgumentException(
                    "Unexpected acount resource type:{0}",
                    type.ToString());
            }
        }

        public static string ToAccountIndentifier(NamedAccount namedAccount)
        {
            return namedAccount.GetBankAccount()
                .Swift.Bic + "|" + namedAccount.GetBankAccount()
                    .Swift.Account;
        }
    }
}
