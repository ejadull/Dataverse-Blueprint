using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataverseBlueprint.Models;

namespace DataverseBlueprint.Exporters
{
    public sealed class PlantUmlExporter : IExporter
    {
        public string Export(IReadOnlyList<EntityModel> entities)
        {
            if (entities == null || entities.Count == 0)
                return string.Empty;

            var entityNames = new HashSet<string>(entities.Select(e => e.LogicalName));
            var sb = new StringBuilder();

            sb.AppendLine("@startuml");
            sb.AppendLine("hide empty methods");
            sb.AppendLine();

            foreach (var entity in entities)
                AppendEntity(sb, entity);

            foreach (var entity in entities)
                AppendRelationships(sb, entity, entityNames);

            sb.Append("@enduml");
            return sb.ToString();
        }

        private static void AppendEntity(StringBuilder sb, EntityModel entity)
        {
            sb.AppendLine($"entity {entity.LogicalName} {{");
            foreach (var attr in entity.Attributes)
            {
                var prefix = attr.IsPrimaryId ? "* " : "  ";
                sb.AppendLine($"  {prefix}{attr.LogicalName} : {MapType(attr.AttributeTypeName)}");
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        private static void AppendRelationships(
            StringBuilder sb,
            EntityModel entity,
            HashSet<string> entityNames)
        {
            foreach (var rel in entity.Relationships)
            {
                if (rel.Type == RelationshipType.OneToMany)
                {
                    if (!entityNames.Contains(rel.ReferencingEntity))
                        continue;

                    sb.AppendLine($"{rel.ReferencedEntity} }}|--|| {rel.ReferencingEntity} : \"{rel.SchemaName}\"");
                }
                else if (rel.Type == RelationshipType.ManyToMany)
                {
                    if (!entityNames.Contains(rel.ReferencingEntity))
                        continue;

                    sb.AppendLine($"{rel.ReferencedEntity} }}|--|{{ {rel.ReferencingEntity} : \"{rel.SchemaName}\"");
                }
            }
        }

        private static string MapType(string attributeTypeName)
        {
            switch (attributeTypeName)
            {
                case "UniqueIdentifier": return "uniqueidentifier";
                case "String":           return "nvarchar";
                case "Integer":          return "int";
                case "BigInt":           return "bigint";
                case "Boolean":          return "bit";
                case "DateTime":         return "datetime";
                case "Decimal":          return "decimal";
                case "Double":           return "float";
                case "Money":            return "money";
                case "Memo":             return "nvarchar";
                case "Lookup":           return "uniqueidentifier";
                case "Owner":            return "uniqueidentifier";
                case "Customer":         return "uniqueidentifier";
                default:                 return attributeTypeName.ToLowerInvariant();
            }
        }
    }
}
