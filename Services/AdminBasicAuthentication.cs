using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JabbadabbadoeBooking.Services;

public class AdminBasicAuthenticationOptions : AuthenticationSchemeOptions {}

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
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail("Invalid scheme"));
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

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
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
    }
}