# Spaces CRUD Module

## Installatiestappen
1. Kopieer bestanden naar de juiste mappen in je project.
2. Voeg in Program.cs toe:
```csharp
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
```
3. In ApplicationDbContext.cs:
```csharp
public DbSet<Space> Spaces => Set<Space>();
public DbSet<SpacePhoto> SpacePhotos => Set<SpacePhoto>();
```
4. Voer EF migraties uit:
```powershell
Add-Migration AddSpaces
Update-Database
```
5. Zorg dat `wwwroot/uploads` bestaat en schrijfbaar is.
6. Ga naar `/AdminSpaces` (alleen admin).

