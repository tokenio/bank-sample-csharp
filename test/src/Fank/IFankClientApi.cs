using System.Threading.Tasks;
using Refit;

namespace Tokenio.BankSample.Fank
{
    public interface IFankClientApi
    {
        [Put("/banks/{bic}/clients")]
        Task<string> AddClient(
            [AliasAs("bic")] string bic,
            [Body] string request);

        [Put("/banks/{bic}/clients/{client_id}/accounts")]
        Task<string> AddAccount(
            [AliasAs("bic")] string bic,
            [AliasAs("client_id")] string clientId,
            [Body] string request);

        [Put("/banks/{bic}/clients/{client_id}/link-accounts")]
        Task<string> AuthorizeLinkAccounts(
            [AliasAs("bic")] string bic,
            [AliasAs("client_id")] string clientId,
            [Body] string request);
    }
}
