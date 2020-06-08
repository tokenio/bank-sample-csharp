using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Tokenio.BankSample.Fank
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var logger = LogManager
                .GetLogger(
                    MethodBase.GetCurrentMethod()
                        .DeclaringType);
            logger.Debug(string.Format("Request: {0}", request));
            if (request.Content != null)
                logger.Debug(await request.Content.ReadAsStringAsync());
            var response = await base.SendAsync(request, cancellationToken);
            logger.Debug("Response: {0}");
            logger.Debug(response);
            if (response.Content != null)
                logger.Debug(await response.Content.ReadAsStringAsync());
            return response;
        }
    }
}
