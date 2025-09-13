using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Services;   // voor BookingService/PaymentService
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Aliassen om ambiguïteit te voorkomen


var builder = WebApplication.CreateBuilder(args);

// MVC + Identity UI
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// EF Core + Identity
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

// Moderne AuthorizationBuilder (voorkomt ASP0025)
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .AddAuthenticationSchemes("AdminBasic"));

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
