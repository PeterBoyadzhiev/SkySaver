# SkySaver Desktop — AI-Assisted Development Exam

---

## 1. Project Topic & System Requirements

**Project: SkySaver Desktop**

SkySaver Desktop is a Windows desktop application for tracking flight deals and managing price alerts. The user selects an origin and destination airport, picks a travel date (with optional return date for round trips), and searches for available flights. Results are displayed as cards showing airline, departure/arrival times, duration, stops, and price. The user can save a price alert on any flight — the app then monitors that route in the background and sends a Windows toast notification if the price drops to or below their target.

**System Requirements:**

- Modern WPF UI with a sidebar navigation model and four pages: Search, Results, Alerts, Settings
- Airport selection via dropdown (7 supported airports; same airport cannot be selected for both origin and destination)
- Round-trip search: optional return date that triggers a second search leg automatically
- Dual data source: a built-in mock dataset (50+ flights, works offline with no setup) and the Aviationstack REST API for live data
- Automatic fallback: if the live API fails or returns empty results, the app transparently switches to mock data
- Price alerts persisted locally using SQLite; alerts survive app restarts
- Background price monitor that checks all active alerts every 30 minutes
- Windows desktop toast notification when a tracked price drops below the target
- Alert management: pause/resume individual alerts, inline delete confirmation (no pop-up dialogs), duplicate alert prevention
- Settings page with masked API key input and live mode indicator
- Clean MVVM architecture, async/await throughout, dependency injection via `Microsoft.Extensions.DependencyInjection`
- Custom app icon in the window title bar, taskbar, and executable

**AI tools used across the project:**

| Tool | Role |
|---|---|
| **ChatGPT** | Initial project planning, architecture ideation, and drafting the structured prompts used with Claude Code |
| **Claude Code** | Primary implementation tool — all code generation, refactoring, multi-agent orchestration, and build verification |
| **GitHub Copilot** | Small UI adjustments and inline suggestions during manual XAML editing |

---

## 2. System Architecture — Modules

The system was broken into six modules, each with a clear responsibility boundary:

| Module | Layer | Key Classes |
|---|---|---|
| UI / MVVM | Views + ViewModels | `SearchView`, `ResultsView`, `AlertsView`, `SettingsView` + corresponding ViewModels |
| Flight Service | Services | `IFlightService`, `MockFlightService`, `AviationStackFlightService`, `FallbackFlightService`, `FlightServiceFactory` |
| Data / Persistence | Models + Repositories | `Flight`, `PriceAlert`, `PriceAlertRepository`, `DatabaseInitializer` |
| Background Monitor | Services | `AlertMonitorService`, `INotificationService`, `WindowsNotificationService` |
| Configuration | Config | `AppConfig`, `DataSourceMode` |
| Multi-Agent Dev System | `.claude/agents/` | `architect`, `backend`, `frontend`, `data`, `qa` agents |

---

## 3. Development Process per Module

---

### Module 1 — UI / MVVM Layer

**Approach & reasoning**

The UI was designed around a fixed 220px sidebar for navigation and a `ContentControl` in the main area that swaps views dynamically using WPF `DataTemplate` routing. Each view is a `UserControl` bound to its ViewModel through the DI container — no business logic in code-behind. All state lives in ViewModels extending a shared `ViewModelBase` implementing `INotifyPropertyChanged`.

**Step-by-step workflow**

1. ChatGPT was used to draft a structured specification prompt describing the full project, which was then given to Claude Code to generate the initial MVVM skeleton (folder structure, `ViewModelBase`, `RelayCommand`, `AsyncRelayCommand`, converters, and all four pages).
2. Claude Code generated all XAML views. Airport selection was initially free-text input and was later switched to ComboBox dropdowns after user feedback.
3. GitHub Copilot was used during manual XAML editing sessions to suggest inline fixes to button sizing, margin adjustments, and DataTrigger patterns on the alert cards.
4. Round-trip support was added by extending `SearchViewModel` with a nullable `ReturnDate` and a checkbox in the XAML.
5. The delete confirmation was redesigned from a `MessageBox` pop-up to an inline Yes/No button swap after user feedback.

**Testing strategy**

Validated each page visually by running the app on a test machine. Checked that bindings updated correctly — e.g. the Search button disabling when the same airport is selected for both dropdowns, and `IsAlertSaved` toggling the button state after saving.

**AI tool choice**

- **ChatGPT** — used to plan the initial UI structure and produce the detailed project specification prompt
- **Claude Code** — generated all XAML and ViewModel code, maintained context across the full project
- **GitHub Copilot** — used for small inline UI adjustments during manual editing

**Key prompts (actual)**

From the initial ChatGPT-drafted specification sent to Claude Code:
> *"Create a desktop application called 'SkySaver Desktop' using C#, .NET 8, WPF, SQLite. Purpose: A simple flight deal tracker that searches flights using the Amadeus API and allows users to save price alerts locally... Use clean MVVM architecture. Use async/await correctly. Include models, services, repositories, viewmodels. Add loading indicators and error handling. Keep the project beginner-friendly for a university course project. Generate the project step-by-step, starting with folder structure and architecture."*

After testing the app:
> *"First of all, you should not be able to select the same FROM and TO airport, as that yields no results. Then, if you get no results and go back to the search, you get a notice below the Search button about no flights found and it suggests the only available routes are from SOF, which remained from the time when that was true but I've since added the other combinations."*

After seeing the MessageBox delete confirmation:
> *"I don't like that the confirmation for the Delete prompts in a new sub-window with a different style. Can we think of something else, for example, pressing on the delete button would replace it with yes and no buttons that would do the respective function — yes removes the entry, no returns you to the previous state."*

---

### Module 2 — Flight Service Layer

**Approach & reasoning**

The service layer is built around a single `IFlightService` interface. A `FlightServiceFactory` reads `AppConfig.DataSourceMode` on every call and returns the appropriate implementation — either `MockFlightService` directly, or a `FallbackFlightService` wrapping `AviationStackFlightService` with `MockFlightService` as fallback. The rest of the app never needs to know which source is active.

The project originally targeted the Amadeus API. After discovering it no longer offers a free tier, a full migration to Aviationstack was planned using ChatGPT to structure the migration requirements, then handed to Claude Code for implementation.

**Step-by-step workflow**

1. ChatGPT was used to draft the detailed Aviationstack migration prompt, including the architecture requirements, fallback behaviour, and UI toggle specification.
2. Claude Code implemented `AviationStackFlightService` (HTTP client, JSON parsing, random price generation since the free tier has no price data), `FallbackFlightService` as a decorator, and the factory.
3. The mock dataset was expanded to 50+ flights covering 7 airports bidirectionally with always-future departure times.
4. `LastUsedSource` was added to `AppConfig` so the Results page could display which source was used.

**Testing strategy**

Tested mock mode without any API key. Tested fallback by using an intentionally invalid API key and confirming mock results still appeared. Confirmed the data source badge on the Results page updated correctly.

**AI tool choice**

- **ChatGPT** — drafted the structured migration specification prompt
- **Claude Code (Backend Agent)** — implemented all service classes after Architect Agent approval

**Key prompts (actual)**

ChatGPT-drafted prompt delivered to Claude Code:
> *"Turns out Amadeus no longer provides free API plans so we need to switch to a different service. Update the existing SkySaver Desktop WPF application to support BOTH: Aviationstack API mode (online) and Offline mock data mode (no API key required). Add a configurable setting using an enum `DataSourceMode { AviationStack, Mock }` and a singleton `AppConfig` class. Default the mode to `Mock` so the application works immediately after cloning with no setup required. Create an `IFlightService` interface with `SearchFlightsAsync(string origin, string destination, DateTime date)`. Implement `MockFlightService` with at least 30 hardcoded realistic flights and a simulated network delay. Implement `AviationStackFlightService` using `HttpClient`. Create a `FlightServiceFactory` that returns the correct implementation based on `AppConfig.DataSourceMode`. If the Aviationstack API fails, times out, or returns empty results — automatically fall back to `MockFlightService`. The app must ALWAYS return results. Add a UI toggle in Settings and show a label on the results page indicating which data source was used."*

---

### Module 3 — Data / Persistence Layer

**Approach & reasoning**

Price alerts are persisted in a local SQLite database in `%APPDATA%\SkySaver\`, created automatically on first run. The repository uses raw parameterised SQL — no ORM — keeping the code explicit. Both `Flight` and `PriceAlert` implement `INotifyPropertyChanged` so UI-state properties update bound controls live without reloading lists.

**Step-by-step workflow**

1. Claude Code's Data Agent created the schema and full CRUD repository.
2. Added `ExistsAsync` for duplicate alert prevention — intentionally with no `IsActive` filter so paused alerts still count as saved.
3. Both model classes were updated to implement `INotifyPropertyChanged` after the QA Agent identified that `IsActive`, `IsAlertSaved`, and `IsConfirmingDelete` changes would not update the UI on plain POCOs.

**Testing strategy**

Saved an alert, restarted the app, confirmed it persisted. Attempted to save a duplicate — confirmed the button stayed as "✓ Alert Saved". Verified paused alerts still showed as saved in search results.

**AI tool choice — Claude Code (Data Agent)**

**Key prompts (actual)**

> *"When you save an alert, the button should change to something like 'Alert Saved' and not allow you to click again, you don't need multiples of the same alert saved. Think of some good UI representation for that, maybe also showing a message that the user already has a saved alert for this flight."*

> *"Just to make sure, pausing the alert should still show the flight in the search results as alert saved."*

---

### Module 4 — Background Monitor & Notifications

**Approach & reasoning**

`AlertMonitorService` runs a `PeriodicTimer` loop (30-minute interval) on a background task. On each tick it loads all active alerts, calls `GetLowestPriceAsync` via the factory for each, and fires a Windows toast notification if the price is at or below the target. Each individual check is wrapped in a try/catch to prevent one failing alert from stopping the loop.

**Step-by-step workflow**

1. Initial implementation injected `IFlightSearchService` directly.
2. After the service layer refactor, Claude Code updated the monitor to inject `IFlightServiceFactory` instead — ensuring mode changes in Settings affect background checks too.

**Testing strategy**

Set a target price above the mock flight price, confirmed the toast notification fired on the next monitor tick.

**AI tool choice — Claude Code (Backend Agent)**

**Key prompts (actual)**

> *"`AlertMonitorService` currently holds a direct `IFlightService` reference injected at startup. Refactor it to depend on `IFlightServiceFactory` instead and call `_factory.Create()` on each check cycle. This ensures that if the user switches between Mock and Live API in Settings, the background alert checks immediately respect the new mode without requiring an app restart."*

---

### Module 5 — Configuration & Settings

**Approach & reasoning**

All runtime configuration lives in a single `AppConfig` singleton extending `ViewModelBase`. The Settings page can bind directly to its properties and changes propagate immediately. The API key is masked using a `PasswordBox` (WPF's `PasswordBox.Password` cannot be data-bound — a code-behind bridge pattern was used instead).

**Step-by-step workflow**

1. Created `AppConfig` with `DataSourceMode` and `AviationStackApiKey` properties.
2. Replaced `TextBox` with `PasswordBox` for the API key field after user feedback about security.
3. Added `LastUsedSource` to track and display which data source served the last results.

**Testing strategy**

Switched modes and confirmed the sidebar label and results badge updated immediately. Entered a key and restarted — confirmed it was preserved via `appsettings.json`.

**AI tool choice — Claude Code (Frontend + Backend Agents)**

**Key prompts (actual)**

> *"On the settings page, where you can input a key for the live API, the key should be hidden for security."*

> *"When selecting the Live API and doing a search, we should show somewhere that the results are from the Live API and not from the mock since it can fallback to the mock and you would not know."*

---

### Module 6 — Multi-Agent Development System

**Approach & reasoning**

Five custom agents were defined in `.claude/agents/`: `architect`, `backend`, `frontend`, `data`, and `qa`. Each has a specialised system prompt defining responsibilities, rules, and output format. The Architect Agent must approve all changes before others implement them. This mirrors a real engineering team structure and kept concerns cleanly separated. The agent prompts themselves were partially planned and worded with help from ChatGPT.

**Step-by-step workflow**

1. ChatGPT was consulted to design the agent role structure and responsibilities.
2. Claude Code created the five agent definition files in the project's `.claude/agents/` directory.
3. Each agent's prompt was refined individually to match the project's specific architecture.
4. For each feature batch: Architect Agent analysed and distributed tasks → Backend/Frontend/Data agents ran in parallel → QA Agent verified before commit.

**Testing strategy**

Build verification (`dotnet build`, 0 errors, 0 warnings) after every agent run. The QA Agent caught: a duplicate `AsyncRelayCommand<T>` class, `PriceAlert` missing `INotifyPropertyChanged`, an inverted visibility converter on the status message, and dead Amadeus code left in the project.

**AI tool choice — Claude Code + ChatGPT**

**Key prompts (actual)**

Introducing the multi-agent system (ChatGPT-assisted specification):
> *"I want to create some agents for this project for future work, can you help with that? You are an AI software engineering team working on a C# .NET 8 WPF project called SkySaver Desktop. You will operate as a multi-agent system with the following roles: Architect Agent – defines structure and design. Backend Agent – implements services, APIs, and data logic. Frontend Agent – implements WPF UI and MVVM. Data Agent – handles models, SQLite, and mock data. QA Agent – checks errors, improves code, and ensures consistency. Each response must clearly label which agent is speaking. Agents must not overlap responsibilities. Architect Agent must approve changes before implementation. Keep all code consistent with MVVM pattern. Prefer simplicity over overengineering."*

Triggering the first architecture review:
> *"Let's run the /architect agent and see what can be improved. If needed, give the tasks to other agents."*

After the Architect produced its improvement list:
> *"Yes, run everything needed."*

---

## 4. Challenges & Tool Comparison

**Biggest challenges**

1. **Amadeus API no longer free** — The original plan used the Amadeus Flight Offers API, which turned out to no longer have a free tier. This required a full mid-project migration to Aviationstack. ChatGPT was used to re-plan the migration requirements and draft the detailed specification prompt before handing it to Claude Code.

2. **SDK not pre-installed** — The project was developed on a machine without .NET 8. Claude Code had to download and install the SDK to a local temp directory before any builds were possible.

3. **Multi-agent sync interruptions** — When the project-scoped agents were introduced, every spawned subagent tried to run a global session setup script on every message (the instruction said "before responding to ANY message"). This caused repeated permission prompts mid-task. The fix was rewording the instruction to explicitly apply only to the main session, not subagents.

4. **WPF-specific quirks** — Several issues needed the QA Agent: `PasswordBox.Password` not bindable in MVVM (code-behind bridge), `Style` not switchable via a converter (inline `DataTrigger` required), and `INotifyPropertyChanged` missing on model classes that needed live UI updates.

**Which tool helped the most and why**

**Claude Code** was the most impactful tool by a significant margin. It maintained full codebase context across dozens of files and hundreds of changes, generated compilable WPF/XAML code reliably, and the multi-agent system allowed parallelising independent tasks with a clear architectural approval gate. The QA Agent repeatedly caught correctness bugs that would have been hard to spot manually.

**ChatGPT** played a valuable supporting role — used to think through architecture decisions before committing, and to produce well-structured, detailed prompts that Claude Code could act on precisely. The initial project specification and the Aviationstack migration spec were both produced this way.

**GitHub Copilot** was useful for small, localised UI edits — autocompleting XAML attribute values and suggesting minor layout fixes inline — but was not well-suited to the multi-file, cross-layer changes that made up most of the work.

**What would be improved with more time**

- Add unit tests for the service layer — `IFlightService` and `IFlightServiceFactory` are well-positioned for this
- Extend mock data to cover more routes and simulate price changes over time so the background monitor has something realistic to trigger on
- Add a price history chart on the alert detail view
- Proper settings persistence (currently the API key is only saved if `appsettings.json` is manually created)
- Replace the reserved `IDialogService` with a proper in-app modal overlay for future confirmation flows

---

## 5. Working System Evidence

*(Add at least two screenshots here — suggested:)*
1. *The Search page with airport dropdowns and the Round Trip checkbox enabled*
2. *The Results page showing flight cards with the "Mock data (offline)" or "Live API" badge*
3. *The Alerts page showing an active and a paused (dimmed) alert with amber Pause / green Resume buttons*
4. *The inline Yes/No delete confirmation on an alert card*

---

## 6. Repository

**GitHub:** https://github.com/PeterBoyadzhiev/SkySaver
