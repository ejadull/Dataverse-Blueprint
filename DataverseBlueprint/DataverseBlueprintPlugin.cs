using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DataverseBlueprint
{
    [Export(typeof(IXrmToolBoxPlugin))]
    public class DataverseBlueprintPlugin : PluginBase, IPluginMetadata
    {
        public override IXrmToolBoxPluginControl GetControl() => new DataverseBlueprintControl();

        public string Name => "Dataverse Blueprint";
        public string Description => "Export Dataverse entity model to DBML, Mermaid, PlantUML, SVG and PNG";
        public string BackgroundColor => "#FFFFFF";
        public string PrimaryFontColor => "#1B1B1B";
        public string SecondaryFontColor => "#1B1B1B";
        public string SmallImageBase64 => _smallLogo.Value;
        public string BigImageBase64 => _bigLogo.Value;

        private static readonly Lazy<string> _smallLogo = new Lazy<string>(() => BuildLogoBase64(32));
        private static readonly Lazy<string> _bigLogo = new Lazy<string>(() => BuildLogoBase64(80));

        private static string BuildLogoBase64(int size)
        {
            var asm = typeof(DataverseBlueprintPlugin).Assembly;
            using (var stream = asm.GetManifestResourceStream("DataverseBlueprint.blueprint.png"))
            {
                if (stream == null)
                    return string.Empty;

                using (var original = Image.FromStream(stream))
                using (var bmp = new Bitmap(size, size))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode      = SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode    = PixelOffsetMode.HighQuality;
                        g.DrawImage(original, 0, 0, size, size);
                    }
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
    }
}
