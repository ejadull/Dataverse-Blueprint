using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataverseBlueprint.Exporters
{
    /// <summary>
    /// Fallback SVG renderer using the mermaid.ink public API.
    /// Requires an internet connection; no local runtime needed.
    /// </summary>
    public sealed class MermaidInkSvgRenderer : ISvgRenderer
    {
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public async Task<string> RenderAsync(string mermaidCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(mermaidCode))
                return string.Empty;

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(mermaidCode));
            var url = $"https://mermaid.ink/svg/{encoded}";

            var response = await _http.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
