# Dataverse Blueprint

[![NuGet](https://img.shields.io/nuget/v/DataverseBlueprint.svg)](https://www.nuget.org/packages/DataverseBlueprint)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![XrmToolBox](https://img.shields.io/badge/XrmToolBox-plugin-orange.svg)](https://www.xrmtoolbox.com)

An [XrmToolBox](https://www.xrmtoolbox.com) plugin that exports your Microsoft Dataverse entity data model to multiple diagram and documentation formats — so you can visualize, share, and document your schema without writing a single line of code.

---

## What it does

Connect to any Dataverse environment, select the entities you care about, and export the full entity relationship diagram in one click.

| Format | Best for |
|--------|----------|
| **DBML** | [dbdiagram.io](https://dbdiagram.io) — interactive ER diagrams |
| **Mermaid** | GitHub, GitLab, Notion, Confluence, [mermaid.live](https://mermaid.live) |
| **PlantUML** | UML-based documentation pipelines |
| **SVG** | Scalable vector diagrams for docs and presentations |
| **PNG** | Raster images for reports, slides, and wikis |

---

## Requirements

| Component | Minimum version |
|-----------|-----------------|
| XrmToolBox | 1.2023.x |
| .NET Framework | 4.8 |
| Microsoft Edge WebView2 Runtime | Latest ([download](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)) |

> **WebView2** is only required for SVG and PNG export. If not installed, the plugin automatically falls back to the [mermaid.ink](https://mermaid.ink) public API.

---

## Installation

### Via XrmToolBox Tool Library

1. Open XrmToolBox
2. Go to **Tool Library**
3. Search for **Dataverse Blueprint**
4. Click **Install** and restart XrmToolBox

### Manual installation

1. Download `DataverseBlueprint.1.x.x.nupkg` from [GitHub Releases](https://github.com/ejadull/DataverseBlueprint/releases)
2. Copy all DLLs from the `Plugins\` folder inside the package to:
   ```
   %APPDATA%\MscrmTools\XrmToolBox\Plugins\
   ```
3. Restart XrmToolBox

---

## Getting started

1. **Connect** to a Dataverse environment using the XrmToolBox connection manager
2. **Choose a filter**
   - *All* — every entity in the environment
   - *Custom Only* — entities with `IsCustomEntity = true`
   - *By Solution* — type the solution unique name
3. Click **Load Entities**
4. Select the entities to include (or use **Select All** / **Deselect All**)
5. Click an **Export as** button and choose a destination file

---

## Export formats in detail

### DBML
Compatible with [dbdiagram.io](https://dbdiagram.io). Each entity becomes a table, each attribute becomes a column, and each relationship becomes a `Ref:` line. Many-to-Many intersect entities are included as junction tables.

### Mermaid
Produces a fenced ` ```mermaid ``` ` block with `erDiagram` syntax. Paste it directly into a GitHub pull request description, a Notion page, or open it in [mermaid.live](https://mermaid.live). Only relationships between the selected entities are emitted, keeping large diagrams clean.

### PlantUML
Produces an `@startuml` / `@enduml` block with entity notation. Primary key attributes are marked with `*`. Works with any [PlantUML](https://www.plantuml.com/plantuml) renderer or CI pipeline integration.

### SVG
The Mermaid diagram is rendered inside an embedded [WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) browser using the official [mermaid.js](https://mermaid.js.org/) library. The resulting SVG is extracted directly from the DOM — no external service required when WebView2 is present. Falls back to [mermaid.ink](https://mermaid.ink) if WebView2 is not installed.

### PNG
The SVG is rasterized using [Svg.NET](https://github.com/svg-net/SVG) at full resolution. Useful for reports and documentation where a static image is preferred.

---

## Settings

Open **File → Settings → Dataverse Blueprint**:

| Setting | Default | Description |
|---------|---------|-------------|
| Include Relationships | On | Emit relationship lines across all formats |
| Include System Attributes | Off | Include built-in (non-custom) attributes per entity |

The last selected filter and solution name are restored automatically on next launch.

---

## Building from source

```bash
git clone https://github.com/ejadull/DataverseBlueprint.git
cd DataverseBlueprint
```

Place the XrmToolBox DLLs in a `XrmToolbox\` folder at the solution root (copy from your XrmToolBox installation):

```
XrmToolbox/
  XrmToolBox.Extensibility.dll
  McTools.Xrm.Connection.dll
  McTools.Xrm.Connection.WinForms.dll
  Microsoft.Xrm.Sdk.dll
  Microsoft.Xrm.Tooling.Connector.dll
  Microsoft.Web.WebView2.Core.dll
  Microsoft.Web.WebView2.WinForms.dll
```

Restore, test, and build:

```bash
dotnet restore
dotnet test DataverseBlueprint.Tests/DataverseBlueprint.Tests.csproj
dotnet build -c Release DataverseBlueprint/DataverseBlueprint.csproj
```

---

## Architecture

```
DataverseBlueprint/
├── Models/                          — Immutable domain models (Entity, Attribute, Relationship)
├── Services/
│   ├── IMetadataService.cs          — Metadata loading contract
│   └── MetadataService.cs           — RetrieveAllEntitiesRequest → domain models
├── Exporters/
│   ├── DbDiagramExporter.cs         — DBML
│   ├── MermaidExporter.cs           — Mermaid erDiagram
│   ├── PlantUmlExporter.cs          — PlantUML entity diagram
│   ├── PngConverter.cs              — SVG → PNG via Svg.NET
│   ├── WebView2SvgRenderer.cs       — Mermaid → SVG via WebView2 + mermaid.js
│   └── MermaidInkSvgRenderer.cs     — Mermaid → SVG via mermaid.ink (fallback)
├── Settings/
│   └── DataverseBlueprintSettings.cs
├── DataverseBlueprintPlugin.cs      — XrmToolBox entry point (IPluginMetadata)
└── DataverseBlueprintControl.cs     — WinForms UI
```

---

## Contributing

Bug reports and pull requests are welcome at [github.com/ejadull/DataverseBlueprint/issues](https://github.com/ejadull/DataverseBlueprint/issues).

---

## License

[MIT](LICENSE) © Edgardo Jadull
