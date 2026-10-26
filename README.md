# Air Cleaning Machine

An interactive 3D simulator of industrial gas-cleaning and CO₂ capture equipment, built in Unity. Users assemble an installation in a constructor (furnace, electrofilter, catalyst, emulsifier, CO₂ capture column, pumps, valves and pipes), then run a simulation that calculates the process parameters, visualises flows and particles, and raises alerts when values go out of range. The simulator can also read live measurements from an MRU Optima gas analyser.

The interface is available in Kazakh, Russian and English.

## Requirements

| | |
|---|---|
| Unity | **6000.6.5f1** (Unity 6) |
| Render pipeline | Universal Render Pipeline 17.6 |
| Platform | Windows (the gas-analyser integration is Windows-only) |
| Input | Legacy Input Manager |

Key packages (see [Packages/manifest.json](Packages/manifest.json)):

- [InstantPipes](https://github.com/leth4/InstantPipes): procedural pipes between equipment
- Alembic: baked fluid animation (`Assets/Resources/Fluid/lll.abc`)
- Timeline: scripted equipment sequences
- TextMeshPro / uGUI: user interface
- Newtonsoft JSON
- [MCP for Unity](https://github.com/CoplayDev/unity-mcp): editor automation (development only)

Plugins: `Assets/Plugins/HidSharp.dll` (USB HID access).

## Getting started

1. Clone the repository. It contains large binary assets (about 1 GB), so the first clone takes a while.
2. Open the folder in Unity Hub with Unity **6000.6.5f1**.
3. Open `Assets/Scenes/SignIn.unity` and press **Play**.
4. Sign in with an account from `Assets/Resources/loginData.json`.

## Scenes

The scenes in Build Settings, in order:

| # | Scene | Purpose |
|---|---|---|
| 0 | `Tutorial` | Guided introduction to the simulator |
| 1 | `SignIn` | Login screen |
| 2 | `MainMenu` | Choose a construction or simulation |
| 3 | `Constructor` | Assemble an installation: place equipment and connect pipes |
| 4 | `Simulation` | Run the assembled installation |
| 5 | `Simulation2` | Alternative simulation layout |
| 6 | `CHP` | Combined heat and power plant scene |

> `SignInSystem` loads the main menu by **index** (`LoadScene(2)`). If you reorder scenes in Build Settings, update it.

## Controls

**Camera** (mouse over the 3D view):

| Input | Action |
|---|---|
| Left mouse drag | Orbit around the pivot |
| Right mouse drag | Look around (the cursor is hidden while held) |
| Middle mouse / Shift + left drag | Pan |
| Mouse wheel | Zoom towards the cursor |
| Double-click | Fly to the object |
| WASD / arrows, Q / E | Fly; down / up (hold Shift to move faster) |
| F | Focus on the object under the cursor |
| Home | Reset the view |
| 1 / 2 / 3 | Front / top / isometric view |
| 4–9 | Preset view points |

**Constructor:** drag equipment with the mouse. R / T and A / D rotate the selected equipment, and Ctrl modifies these actions.

## Project structure

```
Assets/
├── Scenes/            Game scenes
├── Scripts/
│   ├── Facilities/        One folder per piece of equipment: manager, math module, effects
│   │   ├── Common/            Base classes, Facility ID pairing, ParameterRowUI
│   │   ├── Electrofilter/, Emulsifier/, Catalyst/, Oven/
│   │   └── CO2Capture/        CO₂ capture (New/ is the state-machine controller)
│   ├── Constructor/       Placing equipment and connecting pipes
│   ├── Simulation/        Equipment chain (LoadSmoke), molecules, CHP, tutorial
│   ├── Monitoring/        Parameter limits, alerts and recommendations
│   ├── Saving/            Save and load constructions as JSON
│   ├── Camera/            Camera controller and camera sequences
│   ├── Localization/      Translator (reads Data/Translations.csv)
│   ├── Hardware/          MRU Optima gas analyser (USB HID)
│   ├── UI/                Menus, sign-in, tables, process diagrams (Scheme/), right panel
│   ├── Core/              Tags and small shared helpers
│   └── Editor/            Editor tools
├── Prefabs/, Models/, Materials/, Shaders/, Animation/, Audio/, UI/
├── Data/              Translations, default simulation, certificate templates
├── Resources/         Process limits, login data, fluid animation
├── Settings/          URP assets, lighting, HDRI
└── ThirdParty/        DataRenderer2D, Vefects
```

### How a simulation fits together

- Each piece of equipment in the scene has a **manager** (`ModuleManagerBase` and its subclasses) and a **math module** (`MathModuleBase` and its subclasses). The base classes are in `Facilities/Common/`, and each equipment folder holds its own pair.
- `FacilityPairingRegistry` links a manager to its math module by a shared **Facility ID**.
- `ProcessMonitor` checks the values against the limits in `Resources/ProcessLimits.asset`. `ProcessAlert`, `ProcessAdvisor` and `AlertPanelUI` show the warnings and recommendations to the user.

### Editor tools

| Menu | What it does |
|---|---|
| `GameObject > Facility ID > Link Selected As New Pair` | Gives the selected manager and math module a shared Facility ID |
| `Tools > Facility ID > Validate Scene` | Reports missing or duplicate Facility IDs |
| `Tools > Process Monitor > Select Limits` | Selects (or creates) the `ProcessLimits` asset |

## Saving

Constructions are saved as JSON, with camera screenshots, to:

```
%USERPROFILE%\AppData\LocalLow\<Company>\<Product>\SavedConstructions\
```

(`Application.persistentDataPath`). The default simulation in `Assets/Data/Default Simulation` is copied there on first launch.

## Gas analyser (MRU Optima)

`Hardware/OptimaHidPoller` reads live measurements from an MRU Optima analyser over USB HID. It polls the device on a background thread and delivers the data to the main thread.

- It supports these models: SN317718 (O₂, CO, NO, NO₂, NOx, SO₂) and SN355211 (O₂, CO₂, CO, NO, NO₂, NOx).
- **Close MRU4Win** before starting, because only one program can use the device at a time.
- Other scripts read values with `OptimaHidPoller.Instance.TryGet("CO_ppm", out float v)` or subscribe to `OnData`.
- Turn on `writeCsv` in the Inspector to log raw data to `persistentDataPath`.

## Localisation

All UI strings are stored in [Assets/Data/Translations.csv](Assets/Data/Translations.csv). It has the columns `OriginalName;Kazakh;English;Russian`, separated by semicolons. To translate a new string, add a row to this file.

## Building

1. **File > Build Profiles**, then choose **Windows**.
2. Check that the scene list and order match the table above.
3. Click **Build**.
