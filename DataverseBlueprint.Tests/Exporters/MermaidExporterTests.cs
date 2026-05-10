using System.Collections.Generic;
using DataverseBlueprint.Exporters;
using DataverseBlueprint.Models;
using NUnit.Framework;

namespace DataverseBlueprint.Tests.Exporters
{
    [TestFixture]
    public class MermaidExporterTests
    {
        private MermaidExporter _exporter;

        [SetUp]
        public void SetUp()
        {
            _exporter = new MermaidExporter();
        }

        private static AttributeModel PkAttr(string name) =>
            new AttributeModel(name, name, "UniqueIdentifier", isPrimaryId: true, isPrimaryName: false);

        private static AttributeModel FkAttr(string name) =>
            new AttributeModel(name, name, "Lookup", isPrimaryId: false, isPrimaryName: false);

        private static AttributeModel Attr(string name, string type = "String") =>
            new AttributeModel(name, name, type, isPrimaryId: false, isPrimaryName: false);

        private static RelationshipModel OneToMany(
            string referencedEntity, string referencingEntity,
            string referencedAttr = "id", string referencingAttr = "parentid") =>
            new RelationshipModel(RelationshipType.OneToMany,
                referencedEntity, referencingEntity,
                referencedAttr, referencingAttr,
                $"{referencedEntity}_{referencingEntity}");

        // ── Scenario: mermaid code block and erDiagram directive ────────────────

        [Test]
        public void Export_SingleEntity_ContainsMermaidCodeBlock()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("```mermaid"));
            Assert.That(result, Contains.Substring("```"));
        }

        [Test]
        public void Export_SingleEntity_ContainsErDiagramDirective()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("erDiagram"));
        }

        // ── Scenario: PK and FK markers ─────────────────────────────────────────

        [Test]
        public void Export_PrimaryIdAttribute_HasPkMarker()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("PK"));
        }

        [Test]
        public void Export_LookupAttribute_HasFkMarker()
        {
            var entity = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid"), FkAttr("parentaccountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("FK"));
        }

        // ── Scenario: crow's foot notation ──────────────────────────────────────

        [Test]
        public void Export_OneToManyRelationship_UsesCrowsFootNotation()
        {
            var rel = OneToMany("account", "contact", "accountid", "parentaccountid");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid"), FkAttr("parentaccountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("||--o{"));
        }

        // ── Scenario: omit relationships to entities outside selection ───────────

        [Test]
        public void Export_RelationshipToUnselectedEntity_IsOmitted()
        {
            var rel = OneToMany("account", "opportunity", "accountid", "parentaccountid");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });

            var result = _exporter.Export(new List<EntityModel> { account });

            Assert.That(result, Does.Not.Contain("opportunity"));
        }

        // ── Scenario: empty list ─────────────────────────────────────────────────

        [Test]
        public void Export_EmptyList_ReturnsEmptyString()
        {
            var result = _exporter.Export(new List<EntityModel>());

            Assert.That(result, Is.Empty);
        }
    }
}
