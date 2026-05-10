using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataverseBlueprint.Models;

namespace DataverseBlueprint.Exporters
{
    public sealed class MermaidExporter : IExporter
    {
        public string Export(IReadOnlyList<EntityModel> entities)
        {
            if (entities == null || entities.Count == 0)
                return string.Empty;

            var entityNames = new HashSet<string>(entities.Select(e => e.LogicalName));
            var sb = new StringBuilder();

            sb.AppendLine("```mermaid");
            sb.AppendLine("erDiagram");
            sb.AppendLine();

            foreach (var entity in entities)
                AppendEntity(sb, entity);

            foreach (var entity in entities)
                AppendRelationships(sb, entity, entityNames);

            sb.Append("```");
            return sb.ToString();
        }

        private static void AppendEntity(StringBuilder sb, EntityModel entity)
        {
            sb.AppendLine($"  {entity.LogicalName} {{");
            foreach (var attr in entity.Attributes)
            {
                var marker = attr.IsPrimaryId ? "PK"
                    : IsLookup(attr.AttributeTypeName) ? "FK"
                    : string.Empty;

                var markerPart = string.IsNullOrEmpty(marker) ? string.Empty : $" {marker}";
                sb.AppendLine($"    {MapType(attr.AttributeTypeName)} {attr.LogicalName}{markerPart}");
            }
            sb.AppendLine("  }");
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

                    sb.AppendLine(
                        $"  {rel.ReferencedEntity} ||--o{{ {rel.ReferencingEntity} : \"{rel.SchemaName}\"");
                }
            }
        }

        private static bool IsLookup(string typeName) =>
            typeName == "Lookup" || typeName == "Owner" || typeName == "Customer";

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
