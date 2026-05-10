using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace DataverseBlueprint.Exporters
{
    /// <summary>
    /// Renders Mermaid diagrams to SVG using the WebView2 runtime.
    /// Must be called from the UI thread. Requires WebView2 Runtime installed.
    /// </summary>
    public sealed class WebView2SvgRenderer : ISvgRenderer
    {
        private readonly Control _uiOwner;

        public WebView2SvgRenderer(Control uiOwner)
        {
            _uiOwner = uiOwner ?? throw new ArgumentNullException(nameof(uiOwner));
        }

        public static bool IsAvailable()
        {
            try
            {
                return !string.IsNullOrEmpty(CoreWebView2Environment.GetAvailableBrowserVersionString());
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> RenderAsync(string mermaidCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(mermaidCode))
                return string.Empty;

            var wv = new WebView2 { Visible = false, Width = 1200, Height = 800 };
            _uiOwner.Controls.Add(wv);
            try
            {
                await wv.EnsureCoreWebView2Async(null);

                var navDone = new TaskCompletionSource<bool>();
                wv.NavigationCompleted += (s, e) => navDone.TrySetResult(e.IsSuccess);
                wv.NavigateToString(BuildHtml(mermaidCode));

                if (!await navDone.Task)
                    return string.Empty;

                await Task.Delay(1500, cancellationToken);

                var json = await wv.CoreWebView2.ExecuteScriptAsync(
                    "document.querySelector('svg') ? document.querySelector('svg').outerHTML : ''");

                return UnquoteJson(json);
            }
            finally
            {
                _uiOwner.Controls.Remove(wv);
                wv.Dispose();
            }
        }

        private static string BuildHtml(string mermaidCode)
        {
            var encoded = WebUtility.HtmlEncode(mermaidCode);
            return $@"<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <script src='https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js'></script>
</head>
<body style='margin:0;background:#fff'>
  <div class='mermaid'>{encoded}</div>
  <script>mermaid.initialize({{ startOnLoad: true, theme: 'default' }});</script>
</body>
</html>";
        }

        private static string UnquoteJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return string.Empty;
            if (json.StartsWith("\"") && json.EndsWith("\""))
                json = json.Substring(1, json.Length - 2);
            return json.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\'", "'");
        }
    }
}
