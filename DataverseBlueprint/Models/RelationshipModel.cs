namespace DataverseBlueprint.Models
{
    public enum RelationshipType { OneToMany, ManyToMany }

    public sealed class RelationshipModel
    {
        public RelationshipType Type { get; }
        public string ReferencedEntity { get; }
        public string ReferencingEntity { get; }
        public string ReferencedAttribute { get; }
        public string ReferencingAttribute { get; }
        public string SchemaName { get; }

        public RelationshipModel(
            RelationshipType type,
            string referencedEntity,
            string referencingEntity,
            string referencedAttribute,
            string referencingAttribute,
            string schemaName)
        {
            Type = type;
            ReferencedEntity = referencedEntity;
            ReferencingEntity = referencingEntity;
            ReferencedAttribute = referencedAttribute;
            ReferencingAttribute = referencingAttribute;
            SchemaName = schemaName;
        }
    }
}
