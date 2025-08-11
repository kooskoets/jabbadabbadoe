# Jabbadabbadoe Booking — PRO (volledige versie)

**Features**
- ASP.NET Core MVC (net8.0) + Identity (e-mailbevestiging verplicht)
- Mollie-betalingen (redirect + webhook)
- iCal (.ics) export van **Confirmed** boekingen: `/ical/space.ics`
- Kalender met overlap-blokkade, NAW, e-mailbevestigingen
- Bootstrap 5 UI, placeholders in `wwwroot/images/listing`

## Starten
1) Open `JabbadabbadoeBooking.sln` in Visual Studio 2022+  
2) PMC:
```
Add-Migration InitialCreate
Update-Database
```
3) **SMTP** invullen in `appsettings.json` (anders console-logging).  
4) **Mollie**: zet `Mollie:ApiKey` (test_ of live_) en zorg voor publieke HTTPS. Webhook: `/payments/webhook`.  
5) Run (F5).
   - Reserveren: `/Bookings/Create`
   - Identity: `/Identity/Account/Register`, `/Identity/Account/Login`
   - iCal: `/ical/space.ics`
   - Adminlijst (Basic): `/Bookings/AdminList` (gebruikersnaam/wachtwoord in `appsettings.json`)

## Notities
- Overweeg voor productie: Identity-rollen i.p.v. Basic voor admin.
- Voeg cookies/Privacy/AVG en rate limiting/recaptcha toe.