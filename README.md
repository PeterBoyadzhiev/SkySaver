# SkySaver Desktop

A WPF desktop application for tracking flight deals and price alerts, built with C# / .NET 8 and SQLite.

## Features

- **Flight Search** — search routes by IATA code and date via the Amadeus API
- **Results View** — flight cards showing airline, times, duration, stops, and price
- **Price Alerts** — save a target price; the app checks it every 30 minutes and sends a Windows toast notification on a price drop

## Tech Stack

| Layer | Technology |
|---|---|
| UI | WPF (.NET 8) |
| Architecture | MVVM |
| Local storage | SQLite via `Microsoft.Data.Sqlite` |
| Flight data | Amadeus Flight Offers API |
| Notifications | `Microsoft.Toolkit.Uwp.Notifications` |
| DI | `Microsoft.Extensions.DependencyInjection` |

## Project Structure

```
SkySaver/
├── Models/          # Plain data classes (Flight, PriceAlert, SearchQuery, Amadeus DTOs)
├── Services/        # Amadeus API client, notification service, background monitor
├── Repositories/    # SQLite CRUD for PriceAlerts
├── ViewModels/      # SearchViewModel, ResultsViewModel, AlertsViewModel, MainViewModel
├── Views/           # SearchView, ResultsView, AlertsView (UserControls)
├── Helpers/         # ViewModelBase, RelayCommand, AsyncRelayCommand, value converters
├── Data/            # DatabaseInitializer (schema creation)
└── App.xaml.cs      # DI container wiring and startup
```

## Getting Started

1. **Register** a free account at [Amadeus for Developers](https://developers.amadeus.com) and create an app to obtain `Client ID` and `Client Secret`.
2. Open `App.xaml.cs` and replace the placeholder strings:
   ```csharp
   const string clientId     = "YOUR_AMADEUS_CLIENT_ID";
   const string clientSecret = "YOUR_AMADEUS_CLIENT_SECRET";
   ```
3. Build and run:
   ```
   dotnet run
   ```
   Requires .NET 8 SDK and Windows 10 (1803+) for toast notifications.

## Usage

1. Enter origin and destination as 3-letter IATA codes (e.g. `SOF` → `LHR`).
2. Pick a departure date and click **Search Flights**.
3. Click **Save Alert** on any result to save a price target.
4. The app checks prices every 30 minutes and sends a desktop notification if a price drops to or below your target.
