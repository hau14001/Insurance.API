using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.Infrastructure.CorsConfigure
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GemCorsOptions _corsOptions;

        public CorsMiddleware(RequestDelegate next, IOptions<GemCorsOptions> options)
        {
            _next = next;
            _corsOptions = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Request.Headers.TryGetValue("origin", out var origin);
                context.Response.Headers.Remove("Access-Control-Allow-Origin");
                context.Response.Headers.Add("Access-Control-Allow-Origin", _corsOptions.AllowedOrigins?.Any() != true ? "*" : _corsOptions.AllowedOrigins.FirstOrDefault(o => o == origin));

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public class GemCorsOptions
    {
        public IEnumerable<string> AllowedOrigins { get; set; }
    }
}