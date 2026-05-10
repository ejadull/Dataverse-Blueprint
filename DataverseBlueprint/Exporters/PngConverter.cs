using System.Drawing;
using Svg;

namespace DataverseBlueprint.Exporters
{
    public sealed class PngConverter
    {
        public Bitmap Convert(string svgContent)
        {
            if (string.IsNullOrWhiteSpace(svgContent))
                return null;

            try
            {
                var doc = SvgDocument.FromSvg<SvgDocument>(svgContent);
                return doc.Draw();
            }
            catch
            {
                return null;
            }
        }
    }
}
