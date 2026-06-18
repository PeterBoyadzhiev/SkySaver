# SkySaver Desktop

A WPF desktop application for tracking flight deals and price alerts, built with C# / .NET 8 and SQLite.

## Features

- **Flight Search** — pick origin and destination from dropdowns, select a date, and search
- **Round-trip support** — enable the Round Trip checkbox to search both outbound and return flights in one go
- **Results View** — flight cards showing airline, departure/arrival times, duration, stops, and price; displays whether results are from the Live API or Mock data
- **Price Alerts** — save a target price on any flight; the app checks every 30 minutes and sends a Windows toast notification if the price drops to or below your target
- **Alerts management** — pause/resume individual alerts, inline delete confirmation (no pop-up dialogs), duplicate alert prevention
- **Dual data source** — works fully offline with 50+ built-in mock flights, or live via the Aviationstack API
- **Automatic fallback** — if the live API fails or returns no results, the app silently falls back to mock data so you always see results
- **Settings page** — toggle between Mock and Live API mode, enter your API key (masked for security)
- **App icon** — custom icon in the window title bar, taskbar, and exe

## Tech Stack

| Layer | Technology |
|---|---|
| UI | WPF (.NET 8) |
| Architecture | MVVM with `INotifyPropertyChanged` throughout |
| Local storage | SQLite via `Microsoft.Data.Sqlite` |
| Flight data | Mock data (default) / Aviationstack API |
| Notifications | `Microsoft.Toolkit.Uwp.Notifications` |
| DI | `Microsoft.Extensions.DependencyInjection` |

## Project Structure

```
SkySaver/
├── Assets/          # plane.ico (window, taskbar, exe icon)
├── Config/          # AppConfig (runtime config), DataSourceMode enum
├── Data/            # DatabaseInitializer (SQLite schema creation on first run)
├── Helpers/         # ViewModelBase, RelayCommand, AsyncRelayCommand, value converters
├── Models/          # Flight, PriceAlert, SearchQuery, AviationStack DTOs
├── Repositories/    # IPriceAlertRepository, PriceAlertRepository (SQLite CRUD)
├── Services/        # IFlightService, IFlightServiceFactory, MockFlightService,
│                    # AviationStackFlightService, FallbackFlightService,
│                    # FlightServiceFactory, AlertMonitorService,
│                    # INotificationService, WindowsNotificationService,
│                    # IDialogService, WpfDialogService
├── ViewModels/      # MainViewModel, SearchViewModel, ResultsViewModel,
│                    # AlertsViewModel, SettingsViewModel
├── Views/           # SearchView, ResultsView, AlertsView, SettingsView
└── App.xaml.cs      # DI composition root and startup
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
2. Open the app, go to **Settings**, select **Live API (Aviationstack)**, and paste your key — it is masked for security.
   Alternatively, create an `appsettings.json` file next to the executable:
   ```json
   { "AviationStackApiKey": "your_key_here" }
   ```
3. The mode indicator in the sidebar and on the Results page confirms which source is active.

> **Note:** The Aviationstack free tier uses HTTP (not HTTPS) and is limited to 100 requests/month. Prices are not available on the free tier and are generated as estimates.

## Usage

### Searching for flights

1. Select origin and destination airports from the dropdowns (same airport cannot be selected for both).
2. Pick a departure date.
3. Optionally tick **Round trip** and pick a return date — the app searches both legs and shows all results together.
4. Click **Search Flights**.

The results page shows a small badge indicating whether results came from the **Live API** or **Mock data**.

### Saving a price alert

- Click **Save Alert** on any flight card. The button changes to **✓ Alert Saved** and is disabled — you cannot save the same alert twice.
- If you navigate back and search the same route again, already-saved flights are shown as saved.

### Managing alerts

- Navigate to **Price Alerts** to see all saved alerts.
- **Pause** (amber) — suspends price checking for that alert. The card dims to indicate it is inactive. The flight still shows as "Alert Saved" in search results.
- **Resume** (green) — reactivates the alert.
- **Delete** — clicking Delete replaces the button with **Yes** / **No** inline. Yes removes the alert permanently, No cancels without any changes.

The app checks all active alerts every 30 minutes. If a price drops to or below your target, a Windows desktop notification is shown.

## Supported routes in Mock mode

All 7 airports work as both origin and destination in any combination:

| Airport | City |
|---|---|
| SOF | Sofia |
| LHR | London Heathrow |
| TXL | Berlin |
| CDG | Paris Charles de Gaulle |
| FCO | Rome Fiumicino |
| AMS | Amsterdam Schiphol |
| MXP | Milan Malpensa |

## Data Source Modes

| Mode | How to activate | Internet required |
|---|---|---|
| **Mock** (default) | Launch the app — no config needed | No |
| **Live API** | Settings page + valid Aviationstack key | Yes |

If Live API is selected but the request fails or returns no results, the app automatically falls back to mock data so you always see results.
