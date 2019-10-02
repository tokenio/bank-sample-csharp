using System.Collections.Generic;
using System.Collections.Immutable;
using Tokenio.Integration.Api.Service;
using Tokenio.Proto.Common.ConsentProtos;

namespace Tokenio.BankSample.Services
{
    public class ConsentManagementServiceImpl : IConsentManagementService
    {
        private readonly IDictionary<string, Consent> consentById = new Dictionary<string, Consent>();

        public void OnConsentCreated(Consent consent)
        {
            consentById.Add(consent.Id, consent);
        }

        public void OnConsentRevoked(string tokenId)
        {
            consentById.Remove(tokenId);
        }

        public IDictionary<string, Consent> GetConsent()
        {
            return consentById.ToImmutableDictionary();
        }
    }
}
