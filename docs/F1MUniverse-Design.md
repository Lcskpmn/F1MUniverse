# F1MUniverse – Design Document

> Desktop toolkit for editing / inspecting **F1 Manager** saves (F1/F2/F3) with a clean, modern UI.

---

## 1. Project Overview

**Name:** `F1MUniverse`  
**Type:** Desktop app (no web / hosting required)  
**Goal:** A single tool that combines the best parts of:

1. **Mr Only standings app** – visual quality, clean modern layout, easy to read standings.
2. **Colour/Name editor** – simple, direct editing of names/colours.
3. **Save viewer** – deep access to *everything* in the save/database (drivers, staff, teams, contracts, seasons, etc.).
4. **f1dbeditor.com** – overall UX, navigation, and features (records, contracts, performance, etc.).

The app should eventually allow full editing of **F1, F2, and F3** data (drivers, teams, contracts, standings, performance, etc.), for both the **current** and **upcoming** seasons.

For now we focus on:

- **Standings viewer/editor** (first working screen).
- **Contracts viewer/editor** (next major screen).
- Basic **navigation shell** that can be extended later.

---

## 2. Tech Stack

- **Language:** C#
- **Framework:** **.NET 8.0**
- **UI:** WPF (Windows Presentation Foundation)
- **Project type:** `WPF Application` (not `.NET Framework`, not MAUI, not WinUI)
- **IDE:** Visual Studio 2026 Community

Target is **Windows desktop only**. No web technologies, no Electron.

---

## 3. Overall UX / Visual Style

### 3.1 General Style

- Dark theme inspired by:
  - **Mr Only standings app** (clean, focused, racing-like).
  - **f1dbeditor.com** (panels, typography).
- Colors:
  - Window background: `#121212`
  - Left sidebar: `#181818`
  - Top bar: `#1A1A1A`
  - Content panels: `#252525`
  - Text (primary): `#FFFFFF`
  - Text (secondary): `#9E9E9E` / `#A0A0A0`
  - Section labels: `#777777`
- Rounded corners on content panels (`CornerRadius ≈ 8–10`).
- Flat buttons with subtle hover/press states.

### 3.2 Layout

**Two-column layout:**

- **Left column (220px):** Navigation sidebar.
- **Right column:** Top bar + main content.

#### Left Sidebar

- Background: `#181818`
- Top logo area:
  - App title: `F1MUniverse` (bold, white).
  - Subtitle: `Save & DB editor` (smaller, grey).
- Navigation sections:

  ```text
  SEASONS
  - Standings
  - Contracts
  - Calendar (later)

  OTHER
  - Settings (later)

Navigation buttons use a reusable style NavButtonStyle:

Left-aligned content.

Transparent background by default.

Hover: background #232323.

Pressed: background #3A3A3A.

Rounded corners: CornerRadius="6".

Top Bar (Right Side)

Height: ~52px.

Shows current section title and short description.

Example for standings:

Left text: Standings

Right text: • F1 / F2 / F3 save editor (small, grey).

Content Area

Margin: 16 from edges.

Main content inside a <Border>:

Background="#252525"

CornerRadius="10"

Padding="16"

Initially shows a welcome message:

Title: Welcome!

Body: “Select a section on the left to start (Standings or Contracts).”

4. Navigation & Screens
4.1 Implemented Navigation

Left sidebar buttons with click handlers in MainWindow.xaml.cs:

StandingsButton_Click

ContractsButton_Click

On click:

Update top bar text (TopTitleText).

Replace content panel with the corresponding screen (currently a simple placeholder for Contracts).

4.2 Planned Screens

Standings (current focus)

Contracts

Calendar (later)

Settings (later)

Future: Drivers, Teams, Records, Performance, etc.

5. Standings Screen (Current Implementation)
5.1 UI Elements

Inside the content panel when Standings is active:

Title & description

ContentTitleText = "Standings"

Short description text, e.g.:
“Example data – later this will come from your F1/F2/F3 save/database.”

Filters row

Label: Series:

ComboBox SeriesComboBox:

Items: F1, F2, F3

Default: F3

Label: Season:

ComboBox SeasonComboBox:

Example values: 2023, 2024, 2025 (expand later)

Default: 2025

(Optional future) Radio buttons for view type:

DriversRadio (default, checked)

TeamsRadio (disabled for now, planned for later)

Standings table

WPF DataGrid with x:Name="StandingsGrid".

Properties:

AutoGenerateColumns="False"

IsReadOnly="True"

CanUserAddRows="False"

CanUserResizeRows="False"

GridLinesVisibility="None"

Backgrounds to match dark theme.

Columns:

Column	Binding property	Width	Notes
#	Position	40	Position in table
Driver	Driver	*	Shortened driver name
Team	Team	2*	Team name
Points	Points	80	Integer
5.2 Example Data (F3 2025)

Current example driver standings (UI test only):

T. Naël – Prema – 210 pts
M. Boya – Campos – 195 pts
N. León – MP – 186 pts
U. Ugochukwu – Prema – 175 pts
C. Stenshorne – Hitech – 160 pts


C# model:

public class StandingRow
{
    public int Position { get; set; }
    public string Driver { get; set; }  // or Team, depending on view
    public string Team { get; set; }
    public int Points { get; set; }
}

A list of StandingRow is bound to StandingsGrid.ItemsSource.

5.3 Current Code Behaviour

On StandingsButton_Click:

Top bar title changes to "Standings".

Content panel is rebuilt to show:

Title + description.

Series & Season comboboxes.

DataGrid with example F3 2025 data.

When SeriesComboBox or SeasonComboBox changes:

Event handler(s) call UpdateStandingsView() (planned/being implemented) to reload data for:

Selected series (F1, F2, F3).

Selected season (2023/2024/2025/...).

Currently the logic is hard-coded example data. Later it will query the actual save/database.

6. Contracts Screen (Planned)

Goal: Similar style as f1dbeditor Contracts tab.

6.1 Planned Features

Ability to inspect and edit:

Driver contracts (team, start/end season, salary, bonuses, position clauses).

Staff contracts (team, role, salary).

Dual view:

Current grid (per team – drivers and staff).

Contract detail popup/panel (like f1dbeditor).

When a driver is clicked, show a contract dialog:

Current contract (team, salary, valid until, bonus).

Next contract / pre-contract (optional, e.g. “Contract for 2030”).

Later: add Next season view to see which drivers are already signed for which teams in F1/F2/F3.

For now, Contracts screen can be a placeholder with the layout scaffolded but no real data yet.

7. Data & Save Integration (Roadmap)

Not yet implemented – planned direction only.

7.1 File Types

F1 Manager saves are SQLite databases (*.sav → underlying DB).

We already use an external DB viewer to inspect tables:

Staff_BasicData, Staff_Contracts, Drivers, Teams, Races, etc.

7.2 Planned Data Layer

Use SQLite via System.Data.SQLite or Microsoft.Data.Sqlite.

Provide:

File open dialog: select .sav file.

Read-only mode first (just viewing data).

Later: editing with transactions and backup.

Possible structure:

public class SaveDatabase
{
    public string FilePath { get; }
    public SqliteConnection Connection { get; }

    public Task<List<StandingRow>> GetDriverStandingsAsync(string series, int season);
    public Task<List<Contract>> GetContractsAsync(string series, int season);
    // ...
}

Mapping from DB tables:

Standings: use race results and points rules, or dedicated standings tables if present.

Contracts: Staff_Contracts & similar tables.

Fields: StaffID, TeamID, StartDay, EndSeason, Salary, RaceBonus, etc.

7.3 Series Support

Series = F1, F2, F3

Each with:

Drivers, teams, races, points, contracts.

UI should always know which series is active (bound to SeriesComboBox).

8. Future Features / Nice-to-haves

Next Season View:

Show where drivers/staff are signed for next season.

Vacant seats per team in F1/F2/F3.

Calendar editor:

List of races per season, drag-and-drop reordering, sprint toggles.

Records / Stats:

Inspired by f1dbeditor’s Records Hub: all-time wins, points, poles, etc.

Performance / Car Development editor:

Team performance charts per race.

Car part stats (front wing, floor, etc.).

Engine performance editor (Ferrari/Mercedes/Red Bull/Renault/Audi/Honda, etc.).

Colour / Name editor:

Quick editing of team/driver names & colors (like the separate app we use now).

Theme / Settings:

Adjust primary accent colour (e.g. purple/teal).

Toggle animations, fonts, etc.

9. Coding Guidelines

Keep UI text in English.

Separate UI layout (XAML) and logic (code-behind):

Keep event handlers small.

Move data loading & transformations to separate classes/services where possible.

Always escape & in XAML as &amp;.

For controls accessed from code-behind, ensure they have x:Name="..." in XAML.

Use simple C# POCO classes for data rows:

StandingRow, ContractRow, DriverRow, etc.

10. Current File Overview
MainWindow.xaml

Defines:

Window layout (sidebar + top bar + content panel).

Window.Resources (including NavButtonStyle).

Buttons:

StandingsButton

ContractsButton

Named elements:

TopTitleText

ContentTitleText

ContentBodyText

StandingsGrid (inside standings view)

SeriesComboBox, SeasonComboBox (when standings active)

MainWindow.xaml.cs

Handles:

Window constructor.

Navigation button click handlers.

Standings view building & example data population.

(Optionally) handlers for series/season changed and radio button checked.

This document should be kept up to date as new screens and features are added.

::contentReference[oaicite:0]{index=0}
