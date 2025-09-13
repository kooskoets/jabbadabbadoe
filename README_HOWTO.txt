# VS instructies om compile errors te fixen

## Space.PhotoList toevoegen (zonder je bestaande properties kwijt te raken)
Open **Models/Space.cs** en voeg **binnen de class Space { ... }**, dus NIET op namespace-niveau, de volgende property toe:

```csharp
using System.Collections.Generic;
// ... (laat je bestaande using-regels staan)

public ICollection<JabbadabbadoeBooking.Models.SpacePhoto> PhotoList { get; set; } = new List<JabbadabbadoeBooking.Models.SpacePhoto>();
```

> Let op: de curly braces moeten zo zijn:
>
> ```csharp
> namespace JabbadabbadoeBooking.Models
> {
>     public class Space
>     {
>         // ... je bestaande velden/properties
>         public ICollection<SpacePhoto> PhotoList { get; set; } = new List<SpacePhoto>();
>     }
> }
> ```
> Als je `namespace JabbadabbadoeBooking.Models;` (file-scoped) gebruikt, staat er **geen** extra `{ }` achter de namespace en moet de class direct daarna beginnen.
> Zorg dat de `PhotoList` **binnen** de `class Space { ... }` staat.

## DbContext uitbreiden
Open **ApplicationDbContext.cs** en zorg dat bovenin staat:
```csharp
using Microsoft.EntityFrameworkCore;
using JabbadabbadoeBooking.Models;
```

Voeg in de class toe:
```csharp
public DbSet<SpacePhoto> SpacePhotos { get; set; } = null!;
```

(Optioneel in `OnModelCreating`)
```csharp
modelBuilder.Entity<SpacePhoto>()
    .HasOne(p => p.Space)
    .WithMany(s => s.PhotoList)
    .HasForeignKey(p => p.SpaceId)
    .OnDelete(DeleteBehavior.Cascade);
```

## Migrations
In Visual Studio: **Tools ▸ NuGet Package Manager ▸ Package Manager Console**
```
Add-Migration AddSpacePhoto
Update-Database
```

Dat is alles. Daarna **Build ▸ Rebuild Solution**.
