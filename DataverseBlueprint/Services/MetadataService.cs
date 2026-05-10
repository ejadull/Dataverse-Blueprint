using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataverseBlueprint.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseBlueprint.Services
{
    public sealed class MetadataService : IMetadataService
    {
        private readonly IOrganizationService _service;

        public MetadataService(IOrganizationService service)
        {
            _service = service;
        }

        public Task<List<EntityModel>> GetEntitiesAsync(
            MetadataFilter filter,
            string solutionId,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => FetchEntities(filter, solutionId), cancellationToken);
        }

        public Task<List<string>> GetSolutionNamesAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => FetchSolutionNames(), cancellationToken);
        }

        // ── Private implementation ────────────────────────────────────────────────

        private List<EntityModel> FetchEntities(MetadataFilter filter, string solutionId)
        {
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveAllEntitiesResponse)_service.Execute(request);
            var all = response.EntityMetadata ?? new EntityMetadata[0];

            IEnumerable<EntityMetadata> filtered;
            switch (filter)
            {
                case MetadataFilter.CustomOnly:
                    filtered = all.Where(e => e.IsCustomEntity == true);
                    break;
                case MetadataFilter.BySolution:
                    filtered = FilterBySolution(all, solutionId);
                    break;
                default:
                    filtered = all;
                    break;
            }

            return filtered.Select(MapEntity).ToList();
        }

        private EntityModel MapEntity(EntityMetadata em)
        {
            var attributes = (em.Attributes ?? new AttributeMetadata[0])
                .Select(a => new AttributeModel(
                    a.LogicalName ?? string.Empty,
                    ResolveLabel(a.DisplayName) ?? a.LogicalName ?? string.Empty,
                    a.AttributeType.HasValue ? a.AttributeType.Value.ToString() : "String",
                    isPrimaryId: a.LogicalName == em.PrimaryIdAttribute,
                    isPrimaryName: a.LogicalName == em.PrimaryNameAttribute))
                .ToList();

            var relationships = MapRelationships(em);

            return new EntityModel(
                em.LogicalName ?? string.Empty,
                ResolveLabel(em.DisplayName) ?? em.LogicalName ?? string.Empty,
                attributes,
                relationships);
        }

        private static string ResolveLabel(Label label)
        {
            if (label == null) return null;
            return label.UserLocalizedLabel?.Label
                ?? label.LocalizedLabels?.FirstOrDefault()?.Label;
        }

        private static IReadOnlyList<RelationshipModel> MapRelationships(EntityMetadata em)
        {
            var result = new List<RelationshipModel>();

            if (em.OneToManyRelationships != null)
            {
                foreach (var rel in em.OneToManyRelationships)
                {
                    result.Add(new RelationshipModel(
                        Models.RelationshipType.OneToMany,
                        rel.ReferencedEntity  ?? string.Empty,
                        rel.ReferencingEntity ?? string.Empty,
                        rel.ReferencedAttribute  ?? string.Empty,
                        rel.ReferencingAttribute ?? string.Empty,
                        rel.SchemaName ?? string.Empty));
                }
            }

            if (em.ManyToManyRelationships != null)
            {
                foreach (var rel in em.ManyToManyRelationships)
                {
                    result.Add(new RelationshipModel(
                        Models.RelationshipType.ManyToMany,
                        rel.Entity1LogicalName ?? string.Empty,
                        rel.Entity2LogicalName ?? string.Empty,
                        string.Empty,
                        string.Empty,
                        rel.SchemaName ?? string.Empty));
                }
            }

            return result;
        }

        // Task 3.3: filter by solution components
        private IEnumerable<EntityMetadata> FilterBySolution(
            IEnumerable<EntityMetadata> all, string solutionId)
        {
            if (string.IsNullOrWhiteSpace(solutionId))
                return Enumerable.Empty<EntityMetadata>();

            var query = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("objectid"),
                Criteria  = new FilterExpression()
            };
            query.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 1); // Entity
            query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);

            var response = _service.RetrieveMultiple(query);
            var entityIds = new HashSet<string>(
                response.Entities.Select(e => e.GetAttributeValue<System.Guid>("objectid").ToString()));

            // Match by MetadataId — each EntityMetadata has a MetadataId (Guid)
            return all.Where(e => e.MetadataId.HasValue && entityIds.Contains(e.MetadataId.Value.ToString()));
        }

        private List<string> FetchSolutionNames()
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("uniquename", "friendlyname"),
                Criteria  = new FilterExpression()
            };
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);

            var response = _service.RetrieveMultiple(query);
            return response.Entities
                .Select(e => e.GetAttributeValue<string>("uniquename"))
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToList();
        }
    }
}
