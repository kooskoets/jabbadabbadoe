Installatie:
1) Kopieer alle mappen/bestanden naar je project.
2) Program.cs: builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
3) ApplicationDbContext.cs: voeg DbSet<Space> en DbSet<SpacePhoto> + relatie (zie example).
4) Maak wwwroot/uploads aan (schrijfbaar).
5) Migraties: Add-Migration AddSpaces && Update-Database.
6) Ga naar /AdminSpaces.
