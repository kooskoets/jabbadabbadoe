# Program.cs wijziging — AdminOnly policy
Deze Program.cs bevat de extra Authorization policy "AdminOnly".
Vervang je huidige Program.cs met deze, of kopieer het blok tussen de MARKERS:

// Basic-auth voor admin
builder.Services.AddAuthentication("AdminBasic")
    .AddScheme<AdminBasicAuthenticationOptions, AdminBasicAuthenticationHandler>("AdminBasic", null);

// ➜ Authorization policy die door [Authorize(Policy = "AdminOnly")] wordt gebruikt
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddAuthenticationSchemes("AdminBasic");
        policy.RequireRole("Admin");
    });
});

Vergeet niet: in de middleware pipeline moeten zowel app.UseAuthentication() als app.UseAuthorization() staan.
