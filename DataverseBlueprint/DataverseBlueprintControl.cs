using DataverseBlueprint.Exporters;
using DataverseBlueprint.Models;
using DataverseBlueprint.Services;
using DataverseBlueprint.Settings;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace DataverseBlueprint
{

    public partial class DataverseBlueprintControl : PluginControlBase, ISettingsPlugin, IGitHubPlugin, IStatusBarMessenger
    {
        private DataverseBlueprintSettings _settings;
        private IMetadataService _service;
        private List<EntityModel> _entities = new List<EntityModel>();

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public DataverseBlueprintControl()
        {
            InitializeComponent();
        }

        // --- Lifecycle ---

        private void DataverseBlueprintControl_Load(object sender, EventArgs e)
        {
            SettingsManager.Instance.TryLoad(typeof(DataverseBlueprintPlugin), out _settings);
            _settings = _settings ?? new DataverseBlueprintSettings();
            RestoreSettings();
        }

        private void DataverseBlueprintControl_OnCloseTool(object sender, EventArgs e)
        {
            SaveCurrentSettings();
            SettingsManager.Instance.Save(typeof(DataverseBlueprintPlugin), _settings);
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (detail != null)
            {
                _service = new MetadataService(newService);
                SetConnectionState(connected: true);
            }
            else
            {
                _service = null;
                SetConnectionState(connected: false);
            }
        }

        // --- Connection state ---

        private void SetConnectionState(bool connected)
        {
            btnLoad.Enabled = connected;
            if (!connected)
            {
                _entities.Clear();
                clbEntities.Items.Clear();
                UpdateExportButtonState();
            }
        }

        // --- Settings ---

        private void RestoreSettings()
        {
            switch (_settings.LastFilter)
            {
                case MetadataFilter.All:
                    rbAll.Checked = true;
                    break;
                case MetadataFilter.BySolution:
                    rbBySolution.Checked = true;
                    break;
                default:
                    rbCustomOnly.Checked = true;
                    break;
            }
            cmbSolution.Text = _settings.LastSolutionId ?? string.Empty;
        }

        private void SaveCurrentSettings()
        {
            _settings.LastFilter = GetCurrentFilter();
            _settings.LastSolutionId = cmbSolution.Text.Trim();
        }

        private MetadataFilter GetCurrentFilter()
        {
            if (rbAll.Checked) return MetadataFilter.All;
            if (rbBySolution.Checked) return MetadataFilter.BySolution;
            return MetadataFilter.CustomOnly;
        }

        // --- Filter UI ---

        private void rbBySolution_CheckedChanged(object sender, EventArgs e)
        {
            cmbSolution.Enabled = rbBySolution.Checked;
        }

        // --- Load entities ---

        private void btnLoad_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);
        }

        private void LoadEntities()
        {
            var filter = GetCurrentFilter();
            var solutionId = filter == MetadataFilter.BySolution ? cmbSolution.Text.Trim() : null;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities from Dataverse...",
                Work = (worker, args) =>
                {
                    args.Result = _service.GetEntitiesAsync(filter, solutionId, CancellationToken.None)
                        .GetAwaiter().GetResult();
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorNotification(args.Error.Message, null);
                        return;
                    }
                    _entities = (List<EntityModel>)args.Result;
                    PopulateList(_entities);
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs($"Loaded {_entities.Count} entities."));
                }
            });
        }

        private void PopulateList(List<EntityModel> entities)
        {
            clbEntities.Items.Clear();
            foreach (var entity in entities)
                clbEntities.Items.Add(entity, true);
            UpdateExportButtonState();
        }

        // --- Select / Deselect All ---

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbEntities.Items.Count; i++)
                clbEntities.SetItemChecked(i, true);
            UpdateExportButtonState();
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbEntities.Items.Count; i++)
                clbEntities.SetItemChecked(i, false);
            UpdateExportButtonState();
        }

        private void clbEntities_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // ItemCheck fires before the state updates — defer so CheckedItems count is accurate
            BeginInvoke(new Action(UpdateExportButtonState));
        }

        private void UpdateExportButtonState()
        {
            bool hasSelection = clbEntities.CheckedItems.Count > 0;
            btnExportDbml.Enabled = hasSelection;
            btnExportMermaid.Enabled = hasSelection;
            btnExportPlantUml.Enabled = hasSelection;
            btnExportSvg.Enabled = hasSelection;
            btnExportPng.Enabled = hasSelection;
            lblCount.Text = $"{clbEntities.CheckedItems.Count} / {clbEntities.Items.Count} selected";
        }

        // --- Text exports ---

        private void btnExportDbml_Click(object sender, EventArgs e)
            => ExportText(new DbDiagramExporter(), "DBML files|*.dbml|All files|*.*", ".dbml");

        private void btnExportMermaid_Click(object sender, EventArgs e)
            => ExportText(new MermaidExporter(), "Mermaid files|*.mmd|All files|*.*", ".mmd");

        private void btnExportPlantUml_Click(object sender, EventArgs e)
            => ExportText(new PlantUmlExporter(), "PlantUML files|*.puml|All files|*.*", ".puml");

        private void ExportText(IExporter exporter, string filter, string defaultExt)
        {
            var selected = GetSelectedEntities();
            if (selected.Count == 0) return;

            using (var dlg = new SaveFileDialog { Filter = filter, DefaultExt = defaultExt })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                var content = exporter.Export(selected);
                File.WriteAllText(dlg.FileName, content, Encoding.UTF8);
                ShowInfoNotification($"Exported {selected.Count} entities to {Path.GetFileName(dlg.FileName)}", null);
            }
        }

        // --- Image exports ---

        private void btnExportSvg_Click(object sender, EventArgs e) => ExportImageAsync(exportPng: false);
        private void btnExportPng_Click(object sender, EventArgs e) => ExportImageAsync(exportPng: true);

        private async void ExportImageAsync(bool exportPng)
        {
            var selected = GetSelectedEntities();
            if (selected.Count == 0) return;

            var mermaid = new MermaidExporter().Export(selected);

            ISvgRenderer renderer = WebView2SvgRenderer.IsAvailable()
                ? (ISvgRenderer)new WebView2SvgRenderer(this)
                : new MermaidInkSvgRenderer();

            string filter = exportPng ? "PNG files|*.png|All files|*.*" : "SVG files|*.svg|All files|*.*";
            string defaultExt = exportPng ? ".png" : ".svg";
            string filename;

            using (var dlg = new SaveFileDialog { Filter = filter, DefaultExt = defaultExt })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                filename = dlg.FileName;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                var svg = await renderer.RenderAsync(mermaid);
                if (string.IsNullOrEmpty(svg))
                {
                    ShowErrorNotification("Failed to render diagram. Verify the Mermaid syntax is valid.", null);
                    return;
                }

                if (exportPng)
                {
                    var bmp = new PngConverter().Convert(svg);
                    if (bmp == null)
                    {
                        ShowErrorNotification("Failed to convert SVG to PNG.", null);
                        return;
                    }
                    using (bmp)
                        bmp.Save(filename, ImageFormat.Png);
                }
                else
                {
                    File.WriteAllText(filename, svg, Encoding.UTF8);
                }

                ShowInfoNotification($"Exported to {Path.GetFileName(filename)}", null);
            }
            catch (Exception ex)
            {
                ShowErrorNotification(ex.Message, null);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // --- Helpers ---

        private IReadOnlyList<EntityModel> GetSelectedEntities()
        {
            return clbEntities.CheckedItems.Cast<EntityModel>().ToList();
        }

        // --- IGitHubPlugin ---

        public string UserName => "ejadull";
        public string RepositoryName => "Dataverse-Blueprint";

        // --- ISettingsPlugin ---

        public void ShowSettings()
        {
            using (var form = new Form
            {
                Text = "Dataverse Blueprint — Settings",
                Size = new System.Drawing.Size(380, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            })
            {
                var chkRelationships = new CheckBox
                {
                    Text = "Include Relationships",
                    Checked = _settings.IncludeRelationships,
                    Location = new System.Drawing.Point(20, 20),
                    AutoSize = true
                };
                var chkSystemAttrs = new CheckBox
                {
                    Text = "Include System Attributes",
                    Checked = _settings.IncludeSystemAttributes,
                    Location = new System.Drawing.Point(20, 50),
                    AutoSize = true
                };
                var btnOk = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new System.Drawing.Point(180, 130),
                    Size = new System.Drawing.Size(80, 26)
                };
                var btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new System.Drawing.Point(270, 130),
                    Size = new System.Drawing.Size(80, 26)
                };
                form.Controls.AddRange(new Control[] { chkRelationships, chkSystemAttrs, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _settings.IncludeRelationships = chkRelationships.Checked;
                    _settings.IncludeSystemAttributes = chkSystemAttrs.Checked;
                }
            }
        }
    }
}
