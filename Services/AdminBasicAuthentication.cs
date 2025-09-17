using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JabbadabbadoeBooking.Services;

public class AdminBasicAuthenticationOptions : AuthenticationSchemeOptions { }

public class AdminBasicAuthenticationHandler : AuthenticationHandler<AdminBasicAuthenticationOptions>
{
    private readonly IConfiguration _config;
    public AdminBasicAuthenticationHandler(IOptionsMonitor<AdminBasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration config)
        : base(options, logger, encoder, clock)
    {
        _config = config;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        try
        {
            var headerValue = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(headerValue))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            // Parse and validate scheme
            AuthenticationHeaderValue authHeader;
            try
            {
                authHeader = AuthenticationHeaderValue.Parse(headerValue);
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

            if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail("Invalid scheme"));

            var param = authHeader.Parameter ?? "";
            if (string.IsNullOrEmpty(param))
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

            string credentialString;
            try
            {
                var credentialBytes = Convert.FromBase64String(param);
                credentialString = Encoding.UTF8.GetString(credentialBytes);
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

            var credentials = credentialString.Split(':', 2);
            var username = credentials.Length > 0 ? credentials[0] : string.Empty;
            var password = credentials.Length > 1 ? credentials[1] : string.Empty;

            var adminUser = _config["Admin:Username"] ?? "owner";
            var adminPass = _config["Admin:Password"] ?? "ChangeThisAdminPassword123!";

            if (username == adminUser && password == adminPass)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Auth error"));
        }
    }
}
