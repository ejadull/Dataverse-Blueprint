namespace DataverseBlueprint
{
    partial class DataverseBlueprintControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblFilter = new System.Windows.Forms.Label();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbCustomOnly = new System.Windows.Forms.RadioButton();
            this.rbBySolution = new System.Windows.Forms.RadioButton();
            this.cmbSolution = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.clbEntities = new System.Windows.Forms.CheckedListBox();
            this.pnlExport = new System.Windows.Forms.Panel();
            this.lblExport = new System.Windows.Forms.Label();
            this.btnExportDbml = new System.Windows.Forms.Button();
            this.btnExportMermaid = new System.Windows.Forms.Button();
            this.btnExportPlantUml = new System.Windows.Forms.Button();
            this.btnExportSvg = new System.Windows.Forms.Button();
            this.btnExportPng = new System.Windows.Forms.Button();

            this.pnlFilter.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlExport.SuspendLayout();
            this.SuspendLayout();

            // pnlFilter
            this.pnlFilter.Controls.Add(this.btnLoad);
            this.pnlFilter.Controls.Add(this.cmbSolution);
            this.pnlFilter.Controls.Add(this.rbBySolution);
            this.pnlFilter.Controls.Add(this.rbCustomOnly);
            this.pnlFilter.Controls.Add(this.rbAll);
            this.pnlFilter.Controls.Add(this.lblFilter);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Height = 44;
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.TabIndex = 0;

            // lblFilter
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(8, 14);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Text = "Filter:";

            // rbAll
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(55, 11);
            this.rbAll.Name = "rbAll";
            this.rbAll.TabIndex = 1;
            this.rbAll.Text = "All";

            // rbCustomOnly
            this.rbCustomOnly.AutoSize = true;
            this.rbCustomOnly.Checked = true;
            this.rbCustomOnly.Location = new System.Drawing.Point(100, 11);
            this.rbCustomOnly.Name = "rbCustomOnly";
            this.rbCustomOnly.TabIndex = 2;
            this.rbCustomOnly.TabStop = true;
            this.rbCustomOnly.Text = "Custom Only";

            // rbBySolution
            this.rbBySolution.AutoSize = true;
            this.rbBySolution.Location = new System.Drawing.Point(210, 11);
            this.rbBySolution.Name = "rbBySolution";
            this.rbBySolution.TabIndex = 3;
            this.rbBySolution.Text = "By Solution";
            this.rbBySolution.CheckedChanged += new System.EventHandler(this.rbBySolution_CheckedChanged);

            // cmbSolution
            this.cmbSolution.Enabled = false;
            this.cmbSolution.Location = new System.Drawing.Point(315, 10);
            this.cmbSolution.Name = "cmbSolution";
            this.cmbSolution.Size = new System.Drawing.Size(200, 21);
            this.cmbSolution.TabIndex = 4;

            // btnLoad
            this.btnLoad.Enabled = false;
            this.btnLoad.Location = new System.Drawing.Point(525, 9);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(110, 26);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load Entities";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            // pnlActions
            this.pnlActions.Controls.Add(this.lblCount);
            this.pnlActions.Controls.Add(this.btnDeselectAll);
            this.pnlActions.Controls.Add(this.btnSelectAll);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActions.Height = 36;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.TabIndex = 1;

            // btnSelectAll
            this.btnSelectAll.Location = new System.Drawing.Point(8, 6);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(90, 24);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);

            // btnDeselectAll
            this.btnDeselectAll.Location = new System.Drawing.Point(106, 6);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(90, 24);
            this.btnDeselectAll.TabIndex = 1;
            this.btnDeselectAll.Text = "Deselect All";
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);

            // lblCount
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(210, 11);
            this.lblCount.Name = "lblCount";
            this.lblCount.Text = "0 / 0 selected";

            // clbEntities
            this.clbEntities.CheckOnClick = true;
            this.clbEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbEntities.FormattingEnabled = true;
            this.clbEntities.IntegralHeight = false;
            this.clbEntities.Name = "clbEntities";
            this.clbEntities.TabIndex = 2;
            this.clbEntities.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbEntities_ItemCheck);

            // pnlExport
            this.pnlExport.Controls.Add(this.btnExportPng);
            this.pnlExport.Controls.Add(this.btnExportSvg);
            this.pnlExport.Controls.Add(this.btnExportPlantUml);
            this.pnlExport.Controls.Add(this.btnExportMermaid);
            this.pnlExport.Controls.Add(this.btnExportDbml);
            this.pnlExport.Controls.Add(this.lblExport);
            this.pnlExport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlExport.Height = 44;
            this.pnlExport.Name = "pnlExport";
            this.pnlExport.TabIndex = 3;

            // lblExport
            this.lblExport.AutoSize = true;
            this.lblExport.Location = new System.Drawing.Point(8, 14);
            this.lblExport.Name = "lblExport";
            this.lblExport.Text = "Export as:";

            // btnExportDbml
            this.btnExportDbml.Enabled = false;
            this.btnExportDbml.Location = new System.Drawing.Point(80, 9);
            this.btnExportDbml.Name = "btnExportDbml";
            this.btnExportDbml.Size = new System.Drawing.Size(80, 26);
            this.btnExportDbml.TabIndex = 0;
            this.btnExportDbml.Text = "DBML";
            this.btnExportDbml.Click += new System.EventHandler(this.btnExportDbml_Click);

            // btnExportMermaid
            this.btnExportMermaid.Enabled = false;
            this.btnExportMermaid.Location = new System.Drawing.Point(168, 9);
            this.btnExportMermaid.Name = "btnExportMermaid";
            this.btnExportMermaid.Size = new System.Drawing.Size(90, 26);
            this.btnExportMermaid.TabIndex = 1;
            this.btnExportMermaid.Text = "Mermaid";
            this.btnExportMermaid.Click += new System.EventHandler(this.btnExportMermaid_Click);

            // btnExportPlantUml
            this.btnExportPlantUml.Enabled = false;
            this.btnExportPlantUml.Location = new System.Drawing.Point(266, 9);
            this.btnExportPlantUml.Name = "btnExportPlantUml";
            this.btnExportPlantUml.Size = new System.Drawing.Size(90, 26);
            this.btnExportPlantUml.TabIndex = 2;
            this.btnExportPlantUml.Text = "PlantUML";
            this.btnExportPlantUml.Click += new System.EventHandler(this.btnExportPlantUml_Click);

            // btnExportSvg
            this.btnExportSvg.Enabled = false;
            this.btnExportSvg.Location = new System.Drawing.Point(364, 9);
            this.btnExportSvg.Name = "btnExportSvg";
            this.btnExportSvg.Size = new System.Drawing.Size(70, 26);
            this.btnExportSvg.TabIndex = 3;
            this.btnExportSvg.Text = "SVG";
            this.btnExportSvg.Click += new System.EventHandler(this.btnExportSvg_Click);

            // btnExportPng
            this.btnExportPng.Enabled = false;
            this.btnExportPng.Location = new System.Drawing.Point(442, 9);
            this.btnExportPng.Name = "btnExportPng";
            this.btnExportPng.Size = new System.Drawing.Size(70, 26);
            this.btnExportPng.TabIndex = 4;
            this.btnExportPng.Text = "PNG";
            this.btnExportPng.Click += new System.EventHandler(this.btnExportPng_Click);

            // DataverseBlueprintControl
            // Controls.Add order controls docking priority:
            // last added = first to dock (outermost). Fill is added first so it gets remaining space.
            this.Controls.Add(this.clbEntities);
            this.Controls.Add(this.pnlExport);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlFilter);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "DataverseBlueprintControl";
            this.Size = new System.Drawing.Size(1200, 700);
            this.Load += new System.EventHandler(this.DataverseBlueprintControl_Load);
            this.OnCloseTool += new System.EventHandler(this.DataverseBlueprintControl_OnCloseTool);

            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.pnlExport.ResumeLayout(false);
            this.pnlExport.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbCustomOnly;
        private System.Windows.Forms.RadioButton rbBySolution;
        private System.Windows.Forms.ComboBox cmbSolution;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.CheckedListBox clbEntities;
        private System.Windows.Forms.Panel pnlExport;
        private System.Windows.Forms.Label lblExport;
        private System.Windows.Forms.Button btnExportDbml;
        private System.Windows.Forms.Button btnExportMermaid;
        private System.Windows.Forms.Button btnExportPlantUml;
        private System.Windows.Forms.Button btnExportSvg;
        private System.Windows.Forms.Button btnExportPng;
    }
}
