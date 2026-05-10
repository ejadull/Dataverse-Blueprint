namespace DataverseBlueprint.Models
{
    public sealed class AttributeModel
    {
        public string LogicalName { get; }
        public string DisplayName { get; }
        public string AttributeTypeName { get; }
        public bool IsPrimaryId { get; }
        public bool IsPrimaryName { get; }

        public AttributeModel(
            string logicalName,
            string displayName,
            string attributeTypeName,
            bool isPrimaryId,
            bool isPrimaryName)
        {
            LogicalName = logicalName;
            DisplayName = displayName;
            AttributeTypeName = attributeTypeName;
            IsPrimaryId = isPrimaryId;
            IsPrimaryName = isPrimaryName;
        }
    }
}
