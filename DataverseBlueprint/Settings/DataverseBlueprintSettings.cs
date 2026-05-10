using DataverseBlueprint.Services;

namespace DataverseBlueprint.Settings
{
    public class DataverseBlueprintSettings
    {
        public MetadataFilter LastFilter { get; set; } = MetadataFilter.CustomOnly;
        public string LastSolutionId { get; set; } = string.Empty;
        public bool IncludeSystemAttributes { get; set; } = false;
        public bool IncludeRelationships { get; set; } = true;
    }
}
