using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataverseBlueprint.Models;

namespace DataverseBlueprint.Services
{
    public enum MetadataFilter { All, CustomOnly, BySolution }

    public interface IMetadataService
    {
        Task<List<EntityModel>> GetEntitiesAsync(
            MetadataFilter filter,
            string solutionId,
            CancellationToken cancellationToken);

        Task<List<string>> GetSolutionNamesAsync(CancellationToken cancellationToken);
    }
}
