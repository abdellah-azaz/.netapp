using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MonAppMultiplateforme.Services;

public static class AuthTokenProvider
{
    public static string? Token { get; set; }
}

public class TokenInterceptorHandler : DelegatingHandler
{
    public TokenInterceptorHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(AuthTokenProvider.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthTokenProvider.Token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
