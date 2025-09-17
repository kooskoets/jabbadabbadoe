using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Services;   // voor BookingService/PaymentService
using JabbadabbadoeBooking.Security;   // voor de Basic-auth klassen

// Aliassen om ambiguïteit te voorkomen
using AdminBasicOptions = JabbadabbadoeBooking.Security.AdminBasicAuthenticationOptions;
using AdminBasicHandler = JabbadabbadoeBooking.Security.AdminBasicAuthenticationHandler;


var builder = WebApplication.CreateBuilder(args);

// === Services ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Email + domain services
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<PaymentService>(); // laat staan; wordt gebruikt door betalingen

// Admin Basic-auth registreren (zonder default scheme te overschrijven)
builder.Services.AddAuthentication() // Identity blijft default
    .AddScheme<JabbadabbadoeBooking.Security.AdminBasicAuthenticationOptions,
               JabbadabbadoeBooking.Security.AdminBasicAuthenticationHandler>("AdminBasic", _ => { });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .AddAuthenticationSchemes("AdminBasic"));

// Image storage service
builder.Services.AddSingleton<IImageStorageService, ImageStorageService>();

var app = builder.Build();

// Migrate + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    Seed.SeedData(db, app.Configuration);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
