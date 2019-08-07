using Tokenio.Proto.BankLink;

namespace Tokenio.BankSample.Model
{
    public interface IAccountLinking
    {
        BankAuthorization GetBankAuthorization(string accessToken);
    }
}
