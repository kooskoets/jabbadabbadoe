using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace JabbadabbadoeBooking.Security;

public class AdminBasicAuthenticationOptions : AuthenticationSchemeOptions { }

public class AdminBasicAuthenticationHandler : AuthenticationHandler<AdminBasicAuthenticationOptions>
{
    public AdminBasicAuthenticationHandler(
        IOptionsMonitor<AdminBasicAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.NoResult());

        try
        {
            var header = Request.Headers["Authorization"].ToString();
            if (!header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.NoResult());

            var token = header.Substring("Basic ".Length).Trim();
            var raw = Convert.FromBase64String(token);
            var creds = Encoding.UTF8.GetString(raw).Split(':', 2);
            if (creds.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid basic header"));

            const string USER = "admin";
            const string PASS = "admin123";

            if (creds[0] != USER || creds[1] != PASS)
                return Task.FromResult(AuthenticateResult.Fail("Bad credentials"));

            var claims = new[] { new Claim(ClaimTypes.Name, USER), new Claim(ClaimTypes.Role, "Admin") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Auth error"));
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"Jabbadabbadoe Admin\"";
        return Task.CompletedTask;
    }
}