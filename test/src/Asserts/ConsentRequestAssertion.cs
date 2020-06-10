using System;
using Tokenio.Proto.Common.ConsentProtos;
using Tokenio.Proto.Common.TokenProtos;
using Xunit;
using TokenRequest = Tokenio.TokenRequests.TokenRequest;

namespace Tokenio.BankSample.Asserts
{
    public sealed class ConsentRequestAssertion : Assert
    {
        private readonly ConsentRequest actual;

        public ConsentRequestAssertion(ConsentRequest actual)
        {
            this.actual = actual;
        }

        public static ConsentRequestAssertion AssertConsentRequest(
            ConsentRequest consentRequest)
        {
            return new ConsentRequestAssertion(consentRequest);
        }

        public ConsentRequestAssertion MatchesTokenRequest(
            TokenRequest tokenRequest)
        {
            Equal(
                tokenRequest.GetTokenRequestPayload()
                    .RedirectUrl,
                actual.TppCallbackUrl);
            Equal(
                tokenRequest.GetTokenRequestPayload()
                    .RefId,
                actual.TppRefId);
            Equal(
                tokenRequest.GetTokenRequestPayload()
                    .Description,
                actual.Description);

            switch (actual.BodyCase)
            {
            case ConsentRequest.BodyOneofCase.ResourceTypeList:
                Equal(
                    TokenRequestPayload.RequestBodyOneofCase.AccessBody,
                    tokenRequest.GetTokenRequestPayload()
                        .RequestBodyCase);
                Equal(
                    tokenRequest.GetTokenRequestPayload()
                        .AccessBody.ResourceTypeList,
                    actual.ResourceTypeList);
                break;
            case ConsentRequest.BodyOneofCase.AccountResourceList:
                Equal(
                    TokenRequestPayload.RequestBodyOneofCase.AccessBody,
                    tokenRequest.GetTokenRequestPayload()
                        .RequestBodyCase);
                Equal(
                    tokenRequest.GetTokenRequestPayload()
                        .AccessBody.AccountResourceList,
                    actual.AccountResourceList);
                break;
            case ConsentRequest.BodyOneofCase.TransferBody:
                Equal(
                    TokenRequestPayload.RequestBodyOneofCase.TransferBody,
                    tokenRequest.GetTokenRequestPayload()
                        .RequestBodyCase);
                Equal(
                    tokenRequest.GetTokenRequestPayload()
                        .TransferBody,
                    actual.TransferBody);
                break;
            case ConsentRequest.BodyOneofCase.StandingOrderBody:
                Equal(
                    TokenRequestPayload.RequestBodyOneofCase.StandingOrderBody,
                    tokenRequest.GetTokenRequestPayload()
                        .RequestBodyCase);
                Equal(
                    tokenRequest.GetTokenRequestPayload()
                        .StandingOrderBody,
                    actual.StandingOrderBody);
                break;
            case ConsentRequest.BodyOneofCase.BulkTransferBody:
                Equal(
                    TokenRequestPayload.RequestBodyOneofCase.BulkTransferBody,
                    tokenRequest.GetTokenRequestPayload()
                        .RequestBodyCase);
                Equal(
                    tokenRequest.GetTokenRequestPayload()
                        .BulkTransferBody,
                    actual.BulkTransferBody);
                break;
            default:
                throw new ArgumentException(
                    "Unexpected Body Case: {0}",
                    actual.BodyCase.ToString());
            }
            return this;
        }
    }
}
