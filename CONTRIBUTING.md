# Developer Guide — Dataverse Blueprint

## Settings reference

Open **File → Settings → Dataverse Blueprint** inside XrmToolBox:

| Setting | Default | Description |
|---------|---------|-------------|
| Include Relationships | On | Emit relationship lines across all formats |
| Include System Attributes | Off | Include built-in (non-custom) attributes per entity |

The last selected filter and solution name are restored automatically on next launch.

---

## Building from source

### Prerequisites

- Visual Studio 2022 or .NET 8 SDK
- XrmToolBox installed locally

### Setup

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

### Restore, test, and build

```bash
dotnet restore
dotnet test DataverseBlueprint.Tests/DataverseBlueprint.Tests.csproj
dotnet build -c Release DataverseBlueprint/DataverseBlueprint.csproj
```

### Local installation for smoke testing

```powershell
$src  = "DataverseBlueprint\bin\Release\net48"
$dest = "$env:APPDATA\MscrmTools\XrmToolBox\Plugins"
Copy-Item "$src\*.dll" $dest -Force
```

Restart XrmToolBox — the plugin appears without going through the Tool Library.

### Regenerating icons

The PNGs in `icons/` are committed. Regenerate them after a brand change:

```powershell
powershell -NoProfile -File scripts\Generate-Icons.ps1
```

---

## Architecture

```
DataverseBlueprint/
├── Models/                          — Immutable domain models (Entity, Attribute, Relationship)
├── Services/
│   ├── IMetadataService.cs          — Metadata loading contract (filter: All | CustomOnly | BySolution)
│   └── MetadataService.cs           — RetrieveAllEntitiesRequest → domain models
├── Exporters/
│   ├── IExporter.cs                 — Text export contract: Export(entities) → string
│   ├── ISvgRenderer.cs              — Async SVG rendering contract
│   ├── DbDiagramExporter.cs         — DBML output
│   ├── MermaidExporter.cs           — Mermaid erDiagram output
│   ├── PlantUmlExporter.cs          — PlantUML entity diagram output
│   ├── PngConverter.cs              — SVG → Bitmap via Svg.NET
│   ├── WebView2SvgRenderer.cs       — Mermaid → SVG via embedded WebView2 + mermaid.js
│   └── MermaidInkSvgRenderer.cs     — Mermaid → SVG via mermaid.ink HTTP API (fallback)
├── Settings/
│   └── DataverseBlueprintSettings.cs — XML-serializable preferences (XrmToolBox SettingsManager)
├── DataverseBlueprintPlugin.cs      — XrmToolBox entry point (IPluginMetadata, MEF export)
├── DataverseBlueprintControl.cs     — WinForms UI logic
└── DataverseBlueprintControl.Designer.cs
```

### Key design decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| SVG rendering | WebView2 + mermaid.js, mermaid.ink fallback | mermaid.js is the reference implementation; mermaid.ink avoids the WebView2 requirement |
| PNG conversion | Svg.NET | Pure .NET, no external process |
| Async in XrmToolBox | `WorkAsync` + `.GetAwaiter().GetResult()` | `WorkAsync.Work` is not async; blocking inside a background thread is safe |
| Image export | `async void` event handler | Event handlers cannot be awaited; dialog is disposed before await continuation |
| Entity filter BySolution | `QueryExpression` on `solutioncomponent` | No direct SDK filter exists for solution membership |

---

## Publishing a new version

Publishing is fully automated via GitHub Actions. The only manual steps are bumping the version and pushing a tag.

1. Edit `AssemblyVersion` in `DataverseBlueprint/Properties/AssemblyInfo.cs` to match the new version
2. Commit: `git commit -am "chore: bump version to X.Y.Z"`
3. Tag and push:
   ```bash
   git tag vX.Y.Z
   git push origin master --tags
   ```
4. GitHub Actions takes over automatically:
   - Patches the nuspec version from the tag
   - Builds in Release, runs all tests
   - Packs the `.nupkg` and pushes to NuGet.org
   - Creates the GitHub release with the package attached

> The NuGet API key is stored as the `NUGET_API_KEY` GitHub Actions secret — never in the repository.

---

## Contributing

Bug reports and pull requests are welcome. Open an issue at [github.com/ejadull/DataverseBlueprint/issues](https://github.com/ejadull/DataverseBlueprint/issues) before starting significant work.
