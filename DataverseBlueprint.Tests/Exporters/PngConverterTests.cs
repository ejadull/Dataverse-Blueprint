using DataverseBlueprint.Exporters;
using NUnit.Framework;

namespace DataverseBlueprint.Tests.Exporters
{
    [TestFixture]
    public class PngConverterTests
    {
        private PngConverter _converter;

        private const string ValidSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='200' height='100'>" +
            "<rect width='200' height='100' fill='#ffffff'/>" +
            "<text x='10' y='50'>test</text>" +
            "</svg>";

        [SetUp]
        public void SetUp()
        {
            _converter = new PngConverter();
        }

        // ── Scenario: valid SVG → non-null Bitmap with positive dimensions ────────

        [Test]
        public void Convert_ValidSvg_ReturnsBitmapNotNull()
        {
            var result = _converter.Convert(ValidSvg);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Convert_ValidSvg_HasPositiveWidth()
        {
            var result = _converter.Convert(ValidSvg);

            Assert.That(result.Width, Is.GreaterThan(0));
        }

        [Test]
        public void Convert_ValidSvg_HasPositiveHeight()
        {
            var result = _converter.Convert(ValidSvg);

            Assert.That(result.Height, Is.GreaterThan(0));
        }

        // ── Scenario: empty or null SVG → null ────────────────────────────────────

        [Test]
        public void Convert_EmptyString_ReturnsNull()
        {
            var result = _converter.Convert(string.Empty);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_NullString_ReturnsNull()
        {
            var result = _converter.Convert(null);

            Assert.That(result, Is.Null);
        }
    }
}
