using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JabbadabbadoeBooking.Security
{
    public class AdminBasicAuthenticationOptions : AuthenticationSchemeOptions { }

    /// <summary>
    /// Eenvoudige Basic Auth voor het admin-gedeelte.
    /// In .NET 8 gebruiken we de ingebouwde TimeProvider (base.TimeProvider) i.p.v. ISystemClock.
    /// </summary>
    public class AdminBasicAuthenticationHandler
        : AuthenticationHandler<AdminBasicAuthenticationOptions>
    {
        private readonly IConfiguration _config;

        public AdminBasicAuthenticationHandler(
            IOptionsMonitor<AdminBasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock obsoleteClock, // <-- blijft aanwezig als je DI al zo had geregistreerd; wordt NIET gebruikt
            IConfiguration config
        ) : base(options, logger, encoder)
        {
            _config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Verwacht "Authorization: Basic base64(user:pass)"
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());

            var header = Request.Headers["Authorization"].ToString();
            if (!header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.NoResult());

            try
            {
                var token = header.Substring("Basic ".Length).Trim();
                var credentialBytes = Convert.FromBase64String(token);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                var user = credentials.ElementAtOrDefault(0) ?? string.Empty;
                var pass = credentials.ElementAtOrDefault(1) ?? string.Empty;

                var adminUser = _config["Admin:User"] ?? "admin";
                var adminPass = _config["Admin:Pass"] ?? "admin";

                if (user == adminUser && pass == adminPass)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user),
                        new Claim(ClaimTypes.Role, "Admin")
                    };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }

                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
            }
        }
    }
}
