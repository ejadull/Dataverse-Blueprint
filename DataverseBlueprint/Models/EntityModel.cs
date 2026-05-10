using System.Collections.Generic;

namespace DataverseBlueprint.Models
{
    public sealed class EntityModel
    {
        public string LogicalName { get; }
        public string DisplayName { get; }
        public IReadOnlyList<AttributeModel> Attributes { get; }
        public IReadOnlyList<RelationshipModel> Relationships { get; }

        public EntityModel(
            string logicalName,
            string displayName,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<RelationshipModel> relationships)
        {
            LogicalName = logicalName;
            DisplayName = displayName;
            Attributes = attributes;
            Relationships = relationships;
        }

        public override string ToString() => DisplayName ?? LogicalName;
    }
}
