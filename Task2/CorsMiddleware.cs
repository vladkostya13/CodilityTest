using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net.Http;

namespace CodilityTest.Task2
{
    public class CorsMiddleware
    {
        private readonly string OriginRequestHeader = HeaderNames.Origin;
        private readonly string MethodRequestHeader = HeaderNames.AccessControlRequestMethod;
        private readonly string HeadersRequestHeader = HeaderNames.AccessControlRequestHeaders;

        private readonly string OriginResponseHeader = HeaderNames.AccessControlAllowOrigin;
        private readonly string MethodsResponseHeader = HeaderNames.AccessControlAllowMethods;
        private readonly string HeadersResponseHeader = HeaderNames.AccessControlAllowHeaders;

        private readonly CorsPolicy policy;

        public CorsMiddleware(RequestDelegate next, CorsPolicy policy)
        {
            this.policy = policy;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsRequestHttpOptions(context.Request))
            {
                var message = "{\"message\": \"Ok\"}";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(message);
                return;
            }

            var origin = GetOrigin(context.Request);
            var policy = GetPolicyForOrigin(origin);
            if (policy == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }

            if (RequestHasAccessControlRequestMethod(context.Request))
            {
                var header = context.Request.Headers[MethodRequestHeader].ToString();
                var headerList = header.Split(',').Select(x => x.Trim()).ToList();
                var intersectedCollection = policy.AllowedMethods.Intersect(headerList, StringComparer.OrdinalIgnoreCase);
                if (intersectedCollection.Any())
                {
                    context.Response.Headers[MethodsResponseHeader] = string.Join(",", policy.AllowedMethods);
                }
            }

            if (RequestHasAccessControlRequestHeaders(context.Request))
            {
                var header = context.Request.Headers[HeadersRequestHeader].ToString();
                var headerList = header.Split(',').Select(x => x.Trim()).ToList();
                var intersectedCollection = policy.AllowedHeaders.Intersect(headerList, StringComparer.OrdinalIgnoreCase);
                if (intersectedCollection.Count() == headerList.Count)
                {
                    context.Response.Headers[HeadersResponseHeader] = string.Join(",", policy.AllowedHeaders);
                }
            }

            context.Response.Headers[OriginResponseHeader] = origin;
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        private bool IsRequestHttpOptions(HttpRequest request)
        {
            return string.Equals(request.Method, HttpMethod.Options.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private string GetOrigin(HttpRequest request)
        {
            return request.Headers.TryGetValue(OriginRequestHeader, out var value) ? value.ToString() : string.Empty;
        }

        private Policy GetPolicyForOrigin(string origin)
        {
            return policy.OriginsConfig.TryGetValue(origin, out var value) ? value : null;
        }

        private bool RequestHasAccessControlRequestMethod(HttpRequest request)
        {
            return request.Headers.ContainsKey(MethodRequestHeader);
        }

        private bool RequestHasAccessControlRequestHeaders(HttpRequest request)
        {
            return request.Headers.ContainsKey(HeadersRequestHeader);
        }
    }
}