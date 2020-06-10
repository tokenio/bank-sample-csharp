using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using log4net;
using Refit;
using Tokenio.BankSample.Common;
using Tokenio.Proto.Common.ConsentProtos;
using Tokenio.Proto.Gateway;
using Tokenio.Utils;

namespace Tokenio.BankSample.Restful
{
    public class BankClient
    {
        private static readonly int DEFAULT_SSL_PORT = 443;

        private static readonly TimeSpan HTTP_TIMEOUT = TimeSpan.FromSeconds(90);

        private readonly IBankClientApi bankClientApi;
        private readonly string bankId;
        private readonly string baseUrl;
        private HttpClient httpClient;


        public BankClient(string bankId,
            BankConfig bankConfig,
            DnsEndPoint gateway,
            AuthorizationType authorizationType)
        {
            this.bankId = bankId;
            var useSsl = gateway.Port == DEFAULT_SSL_PORT;
            var host = useSsl
                ? gateway.Host
                : string.Format("{0}:{1}", gateway.Host, gateway.Port);
            var protocol = useSsl ? "https" : "http";
            baseUrl = protocol + "://" + host;
            var bankMemberId = GetBankMemberId(bankId);
            HttpMessageHandler authInterceptor;
            switch (authorizationType)
            {
            case AuthorizationType.Token:
                //TODO: add token Token Authorization Interceptor 
                authInterceptor = new HttpClientHandler();
                break;

            case AuthorizationType.Bearer:
                authInterceptor =
                    new BearerAuthorizationInterceptor(
                        bankMemberId,
                        "EdDSA",
                        bankConfig.GetSecretKeyId(),
                        bankConfig.GetSecretKeyStore()
                            .CreateSigner(),
                        new HttpClientHandler());
                break;

            default:
                throw new ArgumentException("Unsupported authorization type");
            }

            bankClientApi = BankClientApi(baseUrl, authInterceptor);
        }


        private string GetBankMemberId(string bankId)
        {
            var response = Wrap(
                BankClientApi(baseUrl)
                    .ResolveAlias(bankId, "BANK"),
                new ResolveAliasResponse());
            return response.Member.Id;
        }

        public string GetBankId()
        {
            return bankId;
        }

        public string CreateUser()
        {
            var response = Wrap(
                bankClientApi.CreateUser(bankId),
                new CreateBankUserResponse());
            return response.UserId;
        }

        public void DeleteUser(string userId)
        {
            Wrap(
                bankClientApi.DeleteUser(bankId, userId),
                new DeleteBankUserResponse());
        }

        public ConsentRequest GetConsentRequest(string requestId)
        {
            var response =
                Wrap(
                    bankClientApi.GetConsentRequest(bankId, requestId),
                    new RetrieveConsentRequestResponse());
            return response.ConsentRequest;
        }

        public Consent CreateConsent(string body)
        {
            var response = Wrap(
                bankClientApi.CreateConsent(bankId, body),
                new CreateConsentResponse());
            return response.Consent;
        }

        public void CancelConsent(string consentId)
        {
            Wrap(
                bankClientApi.CancelConsent(bankId, consentId),
                new CancelConsentResponse());
        }

        private IBankClientApi BankClientApi(string baseUrl,
            HttpMessageHandler authInterceptor = null)
        {
            if (authInterceptor == null)
                httpClient = new HttpClient();
            else
                httpClient = new HttpClient(authInterceptor);

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = HTTP_TIMEOUT;
            return RestService.For<IBankClientApi>(httpClient);
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
