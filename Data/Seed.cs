using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JabbadabbadoeBooking.Data;

public static class Seed
{
    public static void SeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        // Voeg hier desgewenst testdata toe.
    }

    // Alias zodat een bestaande 'Seed(app)' call blijft werken
    public static void Seed(WebApplication app) => SeedData(app);
}
