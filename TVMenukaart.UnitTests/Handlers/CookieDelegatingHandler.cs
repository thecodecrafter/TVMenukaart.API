using System.Net;

namespace TVMenukaart.UnitTests.Handlers
{
    public class CookieDelegatingHandler : DelegatingHandler
    {
        private readonly CookieContainer _cookieContainer;

        public CookieDelegatingHandler(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null)
            {
                var cookieHeader = _cookieContainer.GetCookieHeader(request.RequestUri);
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    request.Headers.Remove("Cookie");
                    request.Headers.Add("Cookie", cookieHeader);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
