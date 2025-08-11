Basic Auth Patch — hardcoded admin credentials
==============================================
Credentials:
- Username: admin
- Password: admin123

Plaats het bestand:
- Security/AdminBasicAuthentication.cs  (overschrijf bestaande)

Program.cs (referentie):
builder.Services.AddAuthentication()
    .AddScheme<JabbadabbadoeBooking.Security.AdminBasicAuthenticationOptions,
               JabbadabbadoeBooking.Security.AdminBasicAuthenticationHandler>("AdminBasic", _ => { });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireAuthenticatedUser().AddAuthenticationSchemes("AdminBasic"));

Gebruik:
- Herstart de app
- Open incognito: https://localhost:xxxx/Bookings/AdminList
- Log in met admin / admin123