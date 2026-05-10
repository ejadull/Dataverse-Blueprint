# Dataverse Blueprint

An [XrmToolBox](https://www.xrmtoolbox.com) plugin that exports your Microsoft Dataverse entity data model to multiple diagram and document formats.

## Features

- **Load entities** from any Dataverse environment — filter by All, Custom Only, or By Solution
- **Select/deselect** individual entities before exporting
- **Five export formats**:
  - **DBML** — [dbdiagram.io](https://dbdiagram.io) compatible schema
  - **Mermaid** — ER diagram syntax for GitHub, GitLab, Notion, and [mermaid.live](https://mermaid.live)
  - **PlantUML** — entity diagram for PlantUML renderers
  - **SVG** — vector graphic rendered from the Mermaid diagram
  - **PNG** — raster image converted from the SVG

## Requirements

| Requirement | Version |
|-------------|---------|
| XrmToolBox | 1.2023.x or later |
| .NET Framework | 4.8 |
| WebView2 Runtime | Latest (for SVG/PNG export — [download](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)) |

> **SVG and PNG export** require the WebView2 Runtime to be installed. If it is absent the plugin falls back to the [mermaid.ink](https://mermaid.ink) public API (requires internet access).

## Installation

### Via XrmToolBox Tool Library (recommended)

1. Open XrmToolBox
2. Go to **Tool Library**
3. Search for **Dataverse Blueprint**
4. Click **Install**

### Manual installation (from a release)

1. Download the latest `.nupkg` from [Releases](https://github.com/ejadull/Dataverse-Blueprint/releases)
2. Copy all DLLs from the package `lib\net48\` folder into your XrmToolBox `Plugins\` directory
3. Restart XrmToolBox

### Local installation (from source)

Use this method while the plugin is not yet in the Tool Library, or to test your own changes.

**Step 1 — Build in Release mode**

```powershell
dotnet build -c Release DataverseBlueprint/DataverseBlueprint.csproj
```

**Step 2 — Locate your XrmToolBox Plugins folder**

The default path is:

```
%APPDATA%\MscrmTools\XrmToolBox\Plugins\
```

Open it directly from Explorer:

```powershell
start %APPDATA%\MscrmTools\XrmToolBox\Plugins
```

> If the folder does not exist, launch XrmToolBox at least once so it creates its directory structure.

**Step 3 — Copy the plugin and its dependencies**

Copy these files from `DataverseBlueprint\bin\Release\net48\` into the Plugins folder:

| File | Purpose |
|------|---------|
| `DataverseBlueprint.dll` | The plugin |
| `Svg.dll` | SVG → PNG conversion |
| `ExCSS.dll` | Dependency of Svg.NET |
| `System.Buffers.dll` | Dependency of Svg.NET |
| `System.Memory.dll` | Dependency of Svg.NET |
| `System.Numerics.Vectors.dll` | Dependency of Svg.NET |
| `System.Runtime.CompilerServices.Unsafe.dll` | Dependency of Svg.NET |

One-liner (PowerShell, run from the solution root after building):

```powershell
$src  = "DataverseBlueprint\bin\Release\net48"
$dest = "$env:APPDATA\MscrmTools\XrmToolBox\Plugins"
$files = @(
    "DataverseBlueprint.dll",
    "Svg.dll", "ExCSS.dll",
    "System.Buffers.dll", "System.Memory.dll",
    "System.Numerics.Vectors.dll",
    "System.Runtime.CompilerServices.Unsafe.dll"
)
$files | ForEach-Object { Copy-Item "$src\$_" -Destination $dest -Force }
Write-Host "Done. Restart XrmToolBox."
```

**Step 4 — Restart XrmToolBox**

Close and reopen XrmToolBox. Search for **Dataverse Blueprint** in the plugin list — it should appear without needing the Tool Library.

## Usage

1. **Connect** to a Dataverse environment using the XrmToolBox connection manager
2. Select a **filter**:
   - *All* — every entity in the environment
   - *Custom Only* — entities where `IsCustomEntity = true`
   - *By Solution* — type the solution unique name in the text box
3. Click **Load Entities** — the list populates with all matching entities, all pre-selected
4. **Check/uncheck** the entities to include in the export (or use Select All / Deselect All)
5. Click one of the **Export as** buttons and choose a save location

## Export Format Details

### DBML
Compatible with [dbdiagram.io](https://dbdiagram.io). Tables map to entities, columns to attributes, and `Ref:` lines to relationships. Many-to-Many relationships are represented as junction tables.

### Mermaid
Produces a fenced ` ```mermaid ``` ` block with `erDiagram` syntax. Paste it directly into GitHub Markdown, Notion, or open it in [mermaid.live](https://mermaid.live). Only relationships between the selected entities are emitted.

### PlantUML
Produces an `@startuml` / `@enduml` entity diagram. Primary key attributes are marked with `*`. Compatible with any [PlantUML](https://www.plantuml.com/plantuml) renderer.

### SVG / PNG
The Mermaid diagram is rendered inside an embedded WebView2 browser using the official [mermaid.js](https://mermaid.js.org/) library. The resulting SVG is captured from the DOM. For PNG, the SVG is rasterized via [Svg.NET](https://github.com/svg-net/SVG). If WebView2 is not installed the [mermaid.ink](https://mermaid.ink) public API is used as a fallback.

## Settings

Open **File → Settings → Dataverse Blueprint** to configure:

| Setting | Default | Description |
|---------|---------|-------------|
| Include Relationships | On | Emit relationship lines in all exports |
| Include System Attributes | Off | Include non-custom attributes per entity |

The last selected filter and solution name are restored automatically on next launch.

## Building from Source

```bash
git clone https://github.com/ejadull/Dataverse-Blueprint.git
cd Dataverse-Blueprint
```

Place the XrmToolBox DLLs in `XrmToolbox\` at the solution root (copy them from your XrmToolBox installation):

```
XrmToolbox\
  XrmToolBox.Extensibility.dll
  McTools.Xrm.Connection.dll
  McTools.Xrm.Connection.WinForms.dll
  Microsoft.Xrm.Sdk.dll
  Microsoft.Xrm.Tooling.Connector.dll
  Microsoft.Web.WebView2.Core.dll
  Microsoft.Web.WebView2.WinForms.dll
```

Build and test:

```bash
dotnet restore
dotnet test DataverseBlueprint.Tests/DataverseBlueprint.Tests.csproj
dotnet build -c Release DataverseBlueprint/DataverseBlueprint.csproj
```

Produce the NuGet package:

```bash
nuget pack DataverseBlueprint.nuspec
```

### Regenerating icons

The icon PNG files in `icons/` are committed to the repo. If you need to regenerate them (e.g. after a brand change), run:

```powershell
powershell -NoProfile -File scripts\Generate-Icons.ps1
```

## Publishing to XrmToolBox Tool Library

Before submitting, verify every item in the official [Tool Library validation checklist](https://www.xrmtoolbox.com/documentation/for-developers/).

### Pre-publish checklist

**NuGet package**

| Item | Status |
|------|--------|
| `<iconUrl>` declared in `.nuspec` | ✅ |
| `<projectUrl>` declared in `.nuspec` | ✅ |
| Plugin DLL and dependencies under `Plugins\` folder in the package | ✅ |
| `AssemblyVersion` matches `<version>` in `.nuspec` | ✅ (both `1.0.0`) |

**Tool behavior**

| Item | Status |
|------|--------|
| Large tool display image (`BigImageBase64`) | ✅ 80×80 px |
| Small tool display image (`SmallImageBase64`) | ✅ 32×32 px |
| Controls resize with XrmToolBox window | ✅ Dock-based layout |
| Opens without an organization connected | ✅ Load button is disabled |
| Controls usable without connection (no errors) | ✅ Filter/solution controls are always enabled |
| Controls requiring connection open the connection dialog | ✅ `ExecuteMethod` pattern |
| Long-running operations are async | ✅ `WorkAsync` for entity load; `async void` for image export |

### Step-by-step publish flow

1. **Commit and push** all files including `icons/` to `ejadull/Dataverse-Blueprint` on GitHub
2. **Verify the icon URL resolves** — open in browser:
   `https://raw.githubusercontent.com/ejadull/Dataverse-Blueprint/main/icons/icon-64.png`
3. **Build Release**
   ```powershell
   dotnet build -c Release DataverseBlueprint/DataverseBlueprint.csproj
   ```
4. **Pack**
   ```powershell
   nuget pack DataverseBlueprint.nuspec
   ```
5. **Verify package structure** — open `DataverseBlueprint.1.0.0.nupkg` with [NuGet Package Explorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer) and confirm all DLLs are under `Plugins\`
6. **Local smoke test** — copy DLLs to `%APPDATA%\MscrmTools\XrmToolBox\Plugins\`, restart XrmToolBox, and verify:
   - Plugin appears in the list with icon
   - Opens without connection (filter controls enabled, Load disabled)
   - Connects and loads entities without error
   - All five export formats produce valid output
7. **Submit** — upload `.nupkg` at the [XrmToolBox portal](https://www.xrmtoolbox.com)

## Architecture

```
DataverseBlueprint/
├── Models/                      — EntityModel, AttributeModel, RelationshipModel (immutable)
├── Services/
│   ├── IMetadataService.cs      — loading contract (filter: All | CustomOnly | BySolution)
│   └── MetadataService.cs       — RetrieveAllEntitiesRequest → domain models
├── Exporters/
│   ├── IExporter.cs             — text export contract: Export(entities) → string
│   ├── ISvgRenderer.cs          — async SVG rendering contract
│   ├── DbDiagramExporter.cs     — DBML output
│   ├── MermaidExporter.cs       — Mermaid erDiagram output
│   ├── PlantUmlExporter.cs      — PlantUML entity diagram output
│   ├── PngConverter.cs          — SVG → Bitmap via Svg.NET
│   ├── WebView2SvgRenderer.cs   — Mermaid → SVG via embedded WebView2 + mermaid.js
│   └── MermaidInkSvgRenderer.cs — Mermaid → SVG via mermaid.ink HTTP API (fallback)
├── Settings/
│   └── DataverseBlueprintSettings.cs — XML-serializable preferences (SettingsManager)
├── DataverseBlueprintPlugin.cs         — XrmToolBox entry point (IPluginMetadata)
├── DataverseBlueprintControl.cs        — WinForms UI logic
└── DataverseBlueprintControl.Designer.cs
```

## Contributing

Bug reports and pull requests are welcome at [github.com/ejadull/Dataverse-Blueprint/issues](https://github.com/ejadull/Dataverse-Blueprint/issues).

## License

MIT
