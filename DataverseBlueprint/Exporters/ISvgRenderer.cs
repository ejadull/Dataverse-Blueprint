using System.Threading;
using System.Threading.Tasks;

namespace DataverseBlueprint.Exporters
{
    public interface ISvgRenderer
    {
        Task<string> RenderAsync(
            string mermaidCode,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
