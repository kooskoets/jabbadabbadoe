using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JabbadabbadoeBooking.Security
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
                // CSP relaxed to allow Bootstrap via jsDelivr and inline scripts used in _Layout
                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "img-src 'self' data: blob:; " +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                    "font-src 'self' https://fonts.gstatic.com; " +
                    "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                    "connect-src 'self';";
                await next();
            });
        }

        // Overload for WebApplication
        public static WebApplication UseSecurityHeaders(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "img-src 'self' data: blob:; " +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                    "font-src 'self' https://fonts.gstatic.com; " +
                    "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                    "connect-src 'self';";
                await next();
            });
            return app;
        }
    }
}
