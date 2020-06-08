using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using Io.Token.Proto.Bankapi;
using log4net;
using Refit;
using Tokenio.Proto.BankLink;
using Tokenio.Proto.Common.MoneyProtos;
using Tokenio.Utils;
using FankAccount = Io.Token.Proto.Bankapi.Account;

namespace Tokenio.BankSample.Fank
{
    public class FankClient
    {
        private static readonly ILog logger = LogManager
            .GetLogger(
                MethodBase.GetCurrentMethod()
                    .DeclaringType);

        private static readonly TimeSpan HTTP_TIMEOUT = TimeSpan.FromSeconds(90);
        private readonly IFankClientApi fankApi;
        private readonly HttpClient httpClient;

        public FankClient(string hostName, int port, bool useSsl)
        {
            var protocol = useSsl ? "https" : "http";
            var urlFormat = "{0}://{1}:{2}";
            var baseUrl = string.Format(urlFormat, protocol, hostName, port);
            httpClient =
                new HttpClient(new LoggingHandler(new HttpClientHandler()));
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = HTTP_TIMEOUT;
            fankApi = RestService.For<IFankClientApi>(httpClient);
        }

        public Task<Client> AddClient(string bic,
            string firstName,
            string lastName)
        {
            var request = new AddClientRequest
            {
                FirstName = firstName,
                LastName = lastName
            };
            var api = fankApi.AddClient(
                bic,
                Util.ToJson(request));
            var response = Wrap(
                api,
                new AddClientResponse());
            return Task.Run(() => { return response.Client; });
        }

        public Task<FankAccount> AddAccount(
            Client client,
            string name,
            string bic,
            string number,
            double amount,
            string currency)
        {
            var request = new AddAccountRequest
            {
                ClientId = client.Id,
                Name = name,
                AccountNumber = number,
                Balance = new Money
                {
                    Value = amount.ToString(),
                    Currency = currency
                }
            };
            var api =
                fankApi.AddAccount(
                    bic,
                    client.Id,
                    Util.ToJson(request));
            var response = Wrap(
                api,
                new AddAccountResponse());
            return Task.Run(() => { return response.Account; });
        }

        public Task<BankAuthorization> StartAccountsLinking(
            string alias,
            string clientId,
            string bic,
            IList<string> accountNumbers)
        {
            var request = new AuthorizeLinkAccountsRequest
            {
                MemberId = alias,
                ClientId = clientId
            };
            request.Accounts.Add(accountNumbers);
            var api = fankApi.AuthorizeLinkAccounts(
                bic,
                clientId,
                Util.ToJson(request));
            var response = Wrap(
                api,
                new BankAuthorization());
            return Task.Run(() => { return response; });
        }

        private T Wrap<T>(Task<string> response, T builder) where T : IMessage
        {
            try
            {
                var json = Util.NormalizeJson(response.Result);
                return (T) JsonParser.Default.Parse(json, builder.Descriptor);
            }
            catch (IOException ex)
            {
                throw new SystemException(ex.Message, ex);
            }
        }
    }
}
