using System.Threading.Tasks;
using Refit;

namespace Tokenio.BankSample.Restful
{
    public interface IBankClientApi
    {
        [Get("/resolve-alias")]
        Task<string> ResolveAlias([Query("alias.value")] string value,
            [Query("alias.type")] string type);

        [Post("/banks/{bank_id}/users")]
        Task<string> CreateUser([AliasAs("bank_id")] string bankId);

        [Delete("/banks/{bank_id}/users/{user_id}")]
        Task<string> DeleteUser([AliasAs("bank_id")] string bankId,
            [AliasAs("user_id")] string userId);

        [Get("/banks/{bank_id}/consent-requests/{request_id}")]
        Task<string> GetConsentRequest([AliasAs("bank_id")] string bankId,
            [AliasAs("request_id")] string requestId);

        [Post("/banks/{bank_id}/consents")]
        Task<string> CreateConsent([AliasAs("bank_id")] string bankId,
            [Body] string body);

        [Delete("/banks/{bank_id}/consents/{consent_id}")]
        Task<string> CancelConsent([AliasAs("bank_id")] string bankId,
            [AliasAs("consent_id")] string consentId);
    }
}
