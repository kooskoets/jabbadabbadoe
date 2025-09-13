FIX: dubbele ApplicationDbContext en dubbele SpacePhoto

Je huidige fouten (CS0263/CS0311/CS0101/CS0111/CS0229) komen door twee dingen:

1) Er staat een EXTRA bestand in je project: Data/ApplicationDbContext_example.cs
   - Dit voorbeeldbestand uit de vorige patch MOET NIET meecompileren.
   - Oplossing: verwijder het uit de Solution of zet Build Action op 'None', of wis het bestand.
   - Alleen je eigen Data/ApplicationDbContext.cs mag blijven bestaan.

2) Er zijn twee versies van SpacePhoto in de Models-map (of een 2e property 'Spaceld').
   - Verwijder de oude/extra SpacePhoto-definitie.
   - Laat alléén Models/SpacePhoto.cs bestaan, met één foreign key: SpaceId (int).

Wat staat er in deze zip:
- Models/SpacePhoto.cs  (de enige geldige definitie, met SpaceId)

Stappenplan
-----------
A. Verwijder uit je project (of zet op Build Action = None):
   - Data/ApplicationDbContext_example.cs

B. Verwijder de dubbele SpacePhoto:
   - Zoek in de Solution naar 'class SpacePhoto' en zorg dat er maar één file overblijft.
   - Gebruik de SpacePhoto.cs uit deze zip als definitieve versie.

C. Build opnieuw.

D. Als je al migrations hebt die naar 'Spaceld' (met kleine L) verwijzen:
   - Verwijder die migration en maak een nieuwe, of maak een nieuwe migration die de kolom hernoemt.
   - Voor nieuwe databases: gewoon nieuwe 'Add-Migration' en 'Update-Database' draaien.

Commandos (indien nodig)
------------------------
Add-Migration FixSpaces
Update-Database
