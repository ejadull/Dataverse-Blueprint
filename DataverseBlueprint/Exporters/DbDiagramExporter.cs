using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataverseBlueprint.Models;

namespace DataverseBlueprint.Exporters
{
    public sealed class DbDiagramExporter : IExporter
    {
        public string Export(IReadOnlyList<EntityModel> entities)
        {
            if (entities == null || entities.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            var entityNames = new HashSet<string>(entities.Select(e => e.LogicalName));
            var processedJunctions = new HashSet<string>();

            foreach (var entity in entities)
                AppendTable(sb, entity);

            sb.AppendLine();

            foreach (var entity in entities)
                AppendRefs(sb, entity, entityNames, processedJunctions);

            return sb.ToString().TrimEnd();
        }

        private static void AppendTable(StringBuilder sb, EntityModel entity)
        {
            sb.AppendLine($"Table {entity.LogicalName} {{");
            foreach (var attr in entity.Attributes)
            {
                var pk = attr.IsPrimaryId ? " [pk]" : string.Empty;
                sb.AppendLine($"  {attr.LogicalName} {MapType(attr.AttributeTypeName)}{pk}");
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        private static void AppendRefs(
            StringBuilder sb,
            EntityModel entity,
            HashSet<string> entityNames,
            HashSet<string> processedJunctions)
        {
            foreach (var rel in entity.Relationships)
            {
                if (rel.Type == RelationshipType.OneToMany)
                {
                    if (!entityNames.Contains(rel.ReferencingEntity))
                        continue;

                    sb.AppendLine(
                        $"Ref: {rel.ReferencedEntity}.{rel.ReferencedAttribute} < {rel.ReferencingEntity}.{rel.ReferencingAttribute}");
                }
                else if (rel.Type == RelationshipType.ManyToMany)
                {
                    if (processedJunctions.Contains(rel.SchemaName))
                        continue;

                    processedJunctions.Add(rel.SchemaName);

                    if (!entityNames.Contains(rel.ReferencedEntity) || !entityNames.Contains(rel.ReferencingEntity))
                        continue;

                    sb.AppendLine($"// N:N relationship");
                    sb.AppendLine($"Table {rel.SchemaName} {{");
                    sb.AppendLine($"  {rel.ReferencedEntity}_id UniqueIdentifier [pk]");
                    sb.AppendLine($"  {rel.ReferencingEntity}_id UniqueIdentifier [pk]");
                    sb.AppendLine("}");
                    sb.AppendLine();
                    sb.AppendLine($"Ref: {rel.SchemaName}.{rel.ReferencedEntity}_id < {rel.ReferencedEntity}.{GetPkName(rel.ReferencedEntity)}");
                    sb.AppendLine($"Ref: {rel.SchemaName}.{rel.ReferencingEntity}_id < {rel.ReferencingEntity}.{GetPkName(rel.ReferencingEntity)}");
                }
            }
        }

        private static string MapType(string attributeTypeName)
        {
            switch (attributeTypeName)
            {
                case "UniqueIdentifier": return "uniqueidentifier";
                case "String":          return "nvarchar";
                case "Integer":         return "int";
                case "BigInt":          return "bigint";
                case "Boolean":         return "bit";
                case "DateTime":        return "datetime";
                case "Decimal":         return "decimal";
                case "Double":          return "float";
                case "Money":           return "money";
                case "Memo":            return "nvarchar(max)";
                case "Lookup":          return "uniqueidentifier";
                case "Owner":           return "uniqueidentifier";
                case "Customer":        return "uniqueidentifier";
                default:                return attributeTypeName.ToLowerInvariant();
            }
        }

        private static string GetPkName(string entityLogicalName) => $"{entityLogicalName}id";
    }
}
