using System;
using System.Text;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace JabbadabbadoeBooking.Services
{
    public class AdminBasicAuthenticationOptions : AuthenticationSchemeOptions { }

    public class AdminBasicAuthenticationHandler : AuthenticationHandler<AdminBasicAuthenticationOptions>
    {
        private readonly IConfiguration _config;

        public AdminBasicAuthenticationHandler(
            IOptionsMonitor<AdminBasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config)
            : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var header) ||
                !string.Equals(header.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header."));
            }

            string user, pass;
            try
            {
                var bytes = Convert.FromBase64String(header.Parameter ?? "");
                var parts = Encoding.UTF8.GetString(bytes).Split(':', 2);
                if (parts.Length != 2) return Task.FromResult(AuthenticateResult.Fail("Invalid Basic payload."));
                user = parts[0]; pass = parts[1];
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Base64 in Authorization header."));
            }

            var expUser = _config["Admin:Username"];
            var expPass = _config["Admin:Password"];
            if (!string.Equals(user, expUser) || !string.Equals(pass, expPass))
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, "Admin"),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
