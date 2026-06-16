# SkySaver Desktop

A WPF desktop application for tracking flight deals and price alerts, built with C# / .NET 8 and SQLite.

## Features

- **Flight Search** — search routes by IATA code and date
- **Results View** — flight cards showing airline, times, duration, stops, and price
- **Price Alerts** — save a target price; the app checks it every 30 minutes and sends a Windows toast notification on a price drop
- **Dual data source** — works fully offline with built-in mock data, or live via the Aviationstack API
- **Automatic fallback** — if the live API fails or returns no results, the app silently falls back to mock data

## Tech Stack

| Layer | Technology |
|---|---|
| UI | WPF (.NET 8) |
| Architecture | MVVM |
| Local storage | SQLite via `Microsoft.Data.Sqlite` |
| Flight data | Mock data (default) / Aviationstack API |
| Notifications | `Microsoft.Toolkit.Uwp.Notifications` |
| DI | `Microsoft.Extensions.DependencyInjection` |

## Project Structure

```
SkySaver/
├── Config/          # AppConfig, DataSourceMode enum
├── Models/          # Flight, PriceAlert, SearchQuery, AviationStack DTOs
├── Services/        # IFlightService, MockFlightService, AviationStackFlightService,
│                    # FallbackFlightService, FlightServiceFactory, AlertMonitorService
├── Repositories/    # SQLite CRUD for PriceAlerts
├── ViewModels/      # MainViewModel, SearchViewModel, ResultsViewModel,
│                    # AlertsViewModel, SettingsViewModel
├── Views/           # SearchView, ResultsView, AlertsView, SettingsView
├── Helpers/         # ViewModelBase, RelayCommand, AsyncRelayCommand, value converters
├── Data/            # DatabaseInitializer (schema creation)
└── App.xaml.cs      # DI container wiring and startup
```

## Getting Started

### Run immediately (no setup required)

The app defaults to **Mock mode** — no API key or internet connection needed.

```
dotnet run
```

Requires .NET 8 SDK and Windows 10 (1803+) for toast notifications.

### Enable live flight data (optional)

1. Register a free account at [aviationstack.com](https://aviationstack.com) to get an API key (no credit card required).
2. Either paste it in the app under **Settings → Live API (Aviationstack)**, or create an `appsettings.json` file next to the executable:
   ```json
   { "AviationStackApiKey": "your_key_here" }
   ```
3. Switch the toggle in **Settings** from *Mock Data* to *Live API*.

> **Note:** The Aviationstack free tier uses HTTP (not HTTPS) and is limited to 100 requests/month. Prices are not available on the free tier and are generated as estimates.

## Usage

1. Enter origin and destination as 3-letter IATA codes (e.g. `SOF` → `LHR`).
2. Pick a departure date and click **Search Flights**.
3. Click **Save Alert** on any result to save a price target.
4. The app checks prices every 30 minutes and sends a desktop notification if a price drops to or below your target.

### Supported routes in Mock mode

| Origin | Destinations |
|---|---|
| SOF (Sofia) | LHR, TXL, CDG, FCO, AMS, MXP |
| LHR, TXL, CDG, FCO, AMS, MXP | SOF |

## Data Source Modes

| Mode | How to activate | Internet required |
|---|---|---|
| **Mock** (default) | Launch the app — no config needed | No |
| **Live API** | Settings page + valid Aviationstack key | Yes |

If Live API is selected but the request fails or returns no results, the app automatically falls back to mock data so you always see results.
