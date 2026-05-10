using System.Collections.Generic;
using DataverseBlueprint.Models;

namespace DataverseBlueprint.Exporters
{
    public interface IExporter
    {
        string Export(IReadOnlyList<EntityModel> entities);
    }
}
