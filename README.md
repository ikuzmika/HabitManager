# HabitManager

Ieradumu pārvaldības lietotne, kas izstrādāta ar .NET MAUI,
ASP.NET Core Web API un SQL Server.

## Funkcionalitāte

- Ieradumu saraksta un detaļu apskate.
- Ieradumu izveidošana un rediģēšana.
- Izpildes biežuma izvēle: katru dienu vai konkrētās nedēļas dienās.
- Ieradumu arhivēšana un atjaunošana.
- Izpildes atzīmēšana izvēlētā datumā un atzīmes atcelšana.
- Datu saglabāšana SQL Server datubāzē.
- Kļūdu paziņojumi, ja API nav sasniedzams vai darbība nav atļauta.

## Risinājuma struktūra

- HabitManager.Domain — domēna modeļi un biznesa noteikumi.
- HabitManager.DataModels — API pieprasījumu un atbilžu modeļi.
- HabitManager.Application — aplikācijas servisi un glabātuves abstrakcija.
- HabitManager.Infrastructure — Entity Framework Core, datu glabātuve un migrācijas.
- HabitManager.Api — API kontrolieri un kļūdu apstrāde.
- HabitManager.App — .NET MAUI lietotne un API klients.
- HabitManager.TestData — testa datu ģenerēšana.

Pašreizējā lietotne datus iegūst no API.
Iepriekš izmantotie lokālie JSON dati netiek automātiski pārnesti uz datubāzi.

## Nepieciešamā programmatūra

Aprakstītā palaišana paredzēta Windows datoram.

- .NET 10 SDK.
- .NET MAUI izstrādes rīki.
- JetBrains Rider vai Visual Studio ar .NET MAUI atbalstu.
- SQL Server.
- Android pārbaudei: Android SDK Platform 36 un Android emulators.
- Entity Framework Core komandrindas rīks `dotnet-ef`, 10. versija.

## API endpointi

| Metode | Ceļš | Darbība |
|---|---|---|
| GET | /api/habits | Ieradumu saraksts |
| GET | /api/habits/{id} | Viena ieraduma detaļas |
| POST | /api/habits | Izveidot ieradumu |
| PUT | /api/habits/{id} | Rediģēt ieradumu |
| PUT | /api/habits/{id}/archive | Arhivēt |
| DELETE | /api/habits/{id}/archive | Atjaunot no arhīva |
| GET | /api/habits/{id}/completions | Izpildes ieraksti |
| POST | /api/habits/{id}/completions | Atzīmēt izpildi |
| DELETE | /api/habits/{id}/completions/{date} | Atcelt izpildi |

Datums URL jānorāda formātā `yyyy-MM-dd`.

## Datu integritāte un kļūdas

- Neeksistējošs ieradums: HTTP 404.
- Nederīgi ievaddati: HTTP 400.
- Neatļauta darbība, piemēram, arhivēta ieraduma izpilde: HTTP 409.
- Datubāzes unikālais indekss nepieļauj vairākus izpildes ierakstus
  vienam ieradumam vienā datumā.
- Dati saglabājas pēc API pārstartēšanas.

## Lietotāja identifikācija

Pašlaik tiek izmantots pagaidu lietotājs `demo-user-1`.
Pilna autentifikācija un lietotāju reģistrācija nav ieviesta.
Visi lietotnes klienti strādā ar šī paša demonstrācijas lietotāja datiem.

## Ierobežojumi

- Lietotnei nepieciešams sasniedzams API.
- Offline režīms un sinhronizācija vēl nav ieviesti.
