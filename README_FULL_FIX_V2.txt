# JabbadabbadoeBooking – Full Fix v2

Bestanden die je 1-op-1 kunt vervangen/toevoegen:
- Models/SpacePhoto.cs  (bevat ook FilePath eigenschap)
- Data/ApplicationDbContext.cs (voegt DbSet<SpacePhoto> toe + relatie)
- Services/ImageStorageService.cs (voegt DeleteImage + validatie toe)
- Security/SecurityHeadersExtensions.cs (heeft ook WebApplication-overload)
- Data/Seed.cs (zorgt dat 'Seed(app)' compileert)

## Visual Studio stappen
1. Vervang/toevoegen via Solution Explorer (Add ▸ Existing Item…).
2. Zorg in Program.cs dat je `using JabbadabbadoeBooking.Security;` bovenaan hebt staan
   en dat `app.UseSecurityHeaders();` in de pipeline staat.
3. Voer (eenmalig) een migratie uit omdat SpacePhoto nieuw is:
   - Tools ▸ NuGet Package Manager ▸ Package Manager Console:
     ```
     Add-Migration AddSpacePhoto
     Update-Database
     ```
4. Build ▸ Rebuild Solution.
