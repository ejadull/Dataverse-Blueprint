using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using DataverseBlueprint.Models;
using DataverseBlueprint.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;
using NUnit.Framework;

namespace DataverseBlueprint.Tests.Services
{
    [TestFixture]
    public class MetadataServiceTests
    {
        private Mock<IOrganizationService> _mockOrg;
        private MetadataService _service;

        [SetUp]
        public void SetUp()
        {
            _mockOrg = new Mock<IOrganizationService>();
            _service  = new MetadataService(_mockOrg.Object);
        }

        // ── Reflection helper (SDK internal setters) ─────────────────────────────

        private static void Set(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop?.GetSetMethod(nonPublic: true)?.Invoke(obj, new[] { value });
        }

        // ── Build helpers ────────────────────────────────────────────────────────

        private void SetupResponse(params EntityMetadata[] entities)
        {
            var response = new RetrieveAllEntitiesResponse();
            response.Results["EntityMetadata"] = entities;
            _mockOrg.Setup(s => s.Execute(It.IsAny<RetrieveAllEntitiesRequest>()))
                    .Returns(response);
        }

        private static AttributeMetadata MakePk(string logicalName)
        {
            var attr = new UniqueIdentifierAttributeMetadata();
            Set(attr, "LogicalName", logicalName);
            return attr;
        }

        private static AttributeMetadata MakeAttr(string logicalName, string type = "String")
        {
            var attr = new StringAttributeMetadata();
            Set(attr, "LogicalName", logicalName);
            return attr;
        }

        private static OneToManyRelationshipMetadata MakeOneToMany(
            string referenced, string referencing, string referencedAttr, string referencingAttr, string schema)
        {
            var rel = new OneToManyRelationshipMetadata();
            Set(rel, "ReferencedEntity",     referenced);
            Set(rel, "ReferencingEntity",    referencing);
            Set(rel, "ReferencedAttribute",  referencedAttr);
            Set(rel, "ReferencingAttribute", referencingAttr);
            Set(rel, "SchemaName",           schema);
            return rel;
        }

        private static ManyToManyRelationshipMetadata MakeManyToMany(
            string entity1, string entity2, string schema)
        {
            var rel = new ManyToManyRelationshipMetadata();
            Set(rel, "Entity1LogicalName", entity1);
            Set(rel, "Entity2LogicalName", entity2);
            Set(rel, "SchemaName",         schema);
            return rel;
        }

        private static EntityMetadata BuildEntity(
            string logicalName,
            string displayName,
            bool isCustom,
            AttributeMetadata[] attributes = null,
            OneToManyRelationshipMetadata[] oneToMany = null,
            ManyToManyRelationshipMetadata[] manyToMany = null)
        {
            var pkName = attributes?.OfType<UniqueIdentifierAttributeMetadata>()
                             .FirstOrDefault()?.LogicalName
                         ?? $"{logicalName}id";

            var em = new EntityMetadata();
            Set(em, "LogicalName",             logicalName);
            Set(em, "DisplayName",             new Label(displayName, 1033));
            Set(em, "IsCustomEntity",          isCustom);
            Set(em, "Attributes",              attributes ?? new AttributeMetadata[0]);
            Set(em, "OneToManyRelationships",  oneToMany  ?? new OneToManyRelationshipMetadata[0]);
            Set(em, "ManyToManyRelationships", manyToMany ?? new ManyToManyRelationshipMetadata[0]);
            Set(em, "PrimaryIdAttribute",      pkName);
            return em;
        }

        // ── Scenario: entity mapping ──────────────────────────────────────────────

        [Test]
        public void GetEntitiesAsync_All_MapsLogicalAndDisplayName()
        {
            SetupResponse(BuildEntity("account", "Account", isCustom: false));

            var result = _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Result;

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].LogicalName, Is.EqualTo("account"));
            Assert.That(result[0].DisplayName, Is.EqualTo("Account"));
        }

        [Test]
        public void GetEntitiesAsync_All_ReturnsAllEntities()
        {
            SetupResponse(
                BuildEntity("account", "Account", isCustom: false),
                BuildEntity("contact", "Contact", isCustom: false));

            var result = _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Result;

            Assert.That(result, Has.Count.EqualTo(2));
        }

        // ── Scenario: attribute mapping ───────────────────────────────────────────

        [Test]
        public void GetEntitiesAsync_All_MapsPrimaryIdAttribute()
        {
            var pkAttr   = MakePk("accountid");
            var nameAttr = MakeAttr("name");
            var em = BuildEntity("account", "Account", isCustom: false,
                attributes: new AttributeMetadata[] { pkAttr, nameAttr });

            SetupResponse(em);

            var result = _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Result;
            var attrs = result[0].Attributes;

            var pk = attrs.FirstOrDefault(a => a.LogicalName == "accountid");
            Assert.That(pk, Is.Not.Null);
            Assert.That(pk.IsPrimaryId, Is.True);

            var name = attrs.FirstOrDefault(a => a.LogicalName == "name");
            Assert.That(name, Is.Not.Null);
            Assert.That(name.IsPrimaryId, Is.False);
        }

        // ── Scenario: relationship mapping ────────────────────────────────────────

        [Test]
        public void GetEntitiesAsync_All_MapsOneToManyRelationship()
        {
            var rel = MakeOneToMany("account", "contact", "accountid", "parentaccountid", "account_contact");
            var em  = BuildEntity("account", "Account", isCustom: false, oneToMany: new[] { rel });
            SetupResponse(em);

            var result = _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Result;
            var rels   = result[0].Relationships;

            Assert.That(rels, Has.Count.EqualTo(1));
            Assert.That(rels[0].Type, Is.EqualTo(Models.RelationshipType.OneToMany));
            Assert.That(rels[0].ReferencedEntity, Is.EqualTo("account"));
            Assert.That(rels[0].ReferencingEntity, Is.EqualTo("contact"));
        }

        [Test]
        public void GetEntitiesAsync_All_MapsManyToManyRelationship()
        {
            var rel = MakeManyToMany("account", "contact", "accountcontact_junction");
            var em  = BuildEntity("account", "Account", isCustom: false, manyToMany: new[] { rel });
            SetupResponse(em);

            var result = _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Result;
            var rels   = result[0].Relationships;

            Assert.That(rels, Has.Count.EqualTo(1));
            Assert.That(rels[0].Type, Is.EqualTo(Models.RelationshipType.ManyToMany));
            Assert.That(rels[0].SchemaName, Is.EqualTo("accountcontact_junction"));
        }

        // ── Scenario: CustomOnly filter ───────────────────────────────────────────

        [Test]
        public void GetEntitiesAsync_CustomOnly_ExcludesSystemEntities()
        {
            SetupResponse(
                BuildEntity("account",     "Account",       isCustom: false),
                BuildEntity("new_custom",  "Custom Entity", isCustom: true));

            var result = _service.GetEntitiesAsync(MetadataFilter.CustomOnly, string.Empty, CancellationToken.None).Result;

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].LogicalName, Is.EqualTo("new_custom"));
        }

        [Test]
        public void GetEntitiesAsync_CustomOnly_EmptyWhenNoCustomEntities()
        {
            SetupResponse(
                BuildEntity("account", "Account", isCustom: false),
                BuildEntity("contact", "Contact", isCustom: false));

            var result = _service.GetEntitiesAsync(MetadataFilter.CustomOnly, string.Empty, CancellationToken.None).Result;

            Assert.That(result, Is.Empty);
        }

        // ── Scenario: request uses correct EntityFilters ──────────────────────────

        [Test]
        public void GetEntitiesAsync_SendsRequestWithAttributesAndRelationships()
        {
            SetupResponse();

            _service.GetEntitiesAsync(MetadataFilter.All, string.Empty, CancellationToken.None).Wait();

            _mockOrg.Verify(s => s.Execute(It.Is<RetrieveAllEntitiesRequest>(r =>
                r.EntityFilters.HasFlag(EntityFilters.Attributes) &&
                r.EntityFilters.HasFlag(EntityFilters.Relationships))), Times.Once);
        }
    }
}
