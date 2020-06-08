using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Tokenio.Security;

namespace Tokenio.BankSample.Restful
{
    public class BearerAuthorizationInterceptor : DelegatingHandler
    {
        private ILog logger = LogManager.GetLogger(
            MethodBase.GetCurrentMethod()
                .DeclaringType);

        private readonly string alg;
        private readonly string keyId;
        private readonly string memberId;
        private readonly ISigner signer;

        public BearerAuthorizationInterceptor(string memberId,
            string alg,
            string keyId,
            ISigner signer,
            HttpMessageHandler innerHandler) : base(innerHandler)
        {
            this.memberId = memberId;
            this.alg = alg;
            this.keyId = keyId;
            this.signer = signer;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // to unescape the path parameters, by default Refit url encode path
            //You could also simply unescape the whole uri.OriginalString
            //but i don´t recommend that, i.e only fix what´s broken
            var uri = request.RequestUri;
            var unescapedPath = Uri.UnescapeDataString(uri.AbsolutePath);
            var userInfo = string.IsNullOrWhiteSpace(uri.UserInfo)
                ? ""
                : $"{uri.UserInfo}@";
            var scheme = string.IsNullOrWhiteSpace(uri.Scheme)
                ? ""
                : $"{uri.Scheme}://";

            request.RequestUri =
                new Uri(
                    $"{scheme}{userInfo}{uri.Authority}{unescapedPath}{uri.Query}{uri.Fragment}");

            string body = null;
            if (request.Content != null)
                body = await request.Content.ReadAsStringAsync();
            var authorization = Authorization(
                request.Method.ToString(),
                request.RequestUri.Host,
                request.RequestUri.AbsolutePath,
                body,
                request.RequestUri.Query);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", authorization);

            logger.Debug(string.Format("Request: {0}", request));
            if (request.Content != null)
            {
                logger.Debug(await request.Content.ReadAsStringAsync());
                logger.Debug(
                    string.Format(
                        "Authorization {0}",
                        request.Headers.Authorization));
            }
            var response = await base.SendAsync(request, cancellationToken);
            logger.Debug("Response: {0}");
            logger.Debug(response);
            if (response.Content != null)
                logger.Debug(await response.Content.ReadAsStringAsync());
            return response;
        }


        private string Authorization(string method,
            string host,
            string path,
            string body = "",
            string query = "")
        {
            var header = new JObject();
            header.Add("alg", alg);
            header.Add("kid", keyId);
            header.Add("mid", memberId);
            header.Add("host", host);
            header.Add("method", method);
            header.Add("path", path);
            if (query.Length > 0)
                header.Add("query", query);

            header.Add(
                "exp",
                (int)
                    (DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1))
                        .TotalSeconds);

            var encodedHeader = Base64UrlEncoder.Encode(header.ToString());
            var encodedBody = Base64UrlEncoder.Encode(body ?? "");
            var signature = signer.Sign(encodedHeader + "." + encodedBody);

            var jwt = encodedHeader + "." + encodedBody + "." + signature;
            return jwt;
        }
    }
}
