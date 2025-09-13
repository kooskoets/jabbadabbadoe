Patch: Space-model toevoegen + AdminBasic TimeProvider-fix

Wat is dit?
- Models/Space.cs  -> definieert het 'Space' model in namespace JabbadabbadoeBooking.Models
- Security/AdminBasicAuthentication.cs en Services/AdminBasicAuthentication.cs
  -> Basic-auth handler zonder ISystemClock; gebruikt .NET 8 TimeProvider (in de base-class).

Waarom?
- Je buildfouten geven aan dat 'Space' ontbreekt. Deze patch levert het model.
- Obsolete waarschuwingen over ISystemClock verdwijnen met deze handler.

Wat nog nodig is in je project:
1) Zorg dat Data/ApplicationDbContext.cs de DbSets bevat:
   public DbSet<Space> Spaces => Set<Space>();
   public DbSet<SpacePhoto> SpacePhotos => Set<SpacePhoto>();

2) Program.cs DI (als nog niet gedaan):
   builder.Services.AddAuthentication()
       .AddScheme<AdminBasicAuthenticationOptions, AdminBasicAuthenticationHandler>("AdminBasic", null);

3) Authorize-policy voor AdminOnly (zoals je al had).

4) Clean + Rebuild. Daarna migraties:
   Add-Migration AddSpacesModel
   Update-Database
