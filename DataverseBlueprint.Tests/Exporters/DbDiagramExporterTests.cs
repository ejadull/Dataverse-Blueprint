using System.Collections.Generic;
using DataverseBlueprint.Exporters;
using DataverseBlueprint.Models;
using NUnit.Framework;

namespace DataverseBlueprint.Tests.Exporters
{
    [TestFixture]
    public class DbDiagramExporterTests
    {
        private DbDiagramExporter _exporter;

        [SetUp]
        public void SetUp()
        {
            _exporter = new DbDiagramExporter();
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private static AttributeModel PkAttr(string name = "id") =>
            new AttributeModel(name, name, "UniqueIdentifier", isPrimaryId: true, isPrimaryName: false);

        private static AttributeModel Attr(string name, string type = "String") =>
            new AttributeModel(name, name, type, isPrimaryId: false, isPrimaryName: false);

        private static RelationshipModel OneToMany(
            string referencedEntity, string referencingEntity,
            string referencedAttr = "id", string referencingAttr = "parentid") =>
            new RelationshipModel(RelationshipType.OneToMany,
                referencedEntity, referencingEntity,
                referencedAttr, referencingAttr,
                $"{referencedEntity}_{referencingEntity}");

        private static RelationshipModel ManyToMany(
            string entityA, string entityB, string schema = "junction_schema") =>
            new RelationshipModel(RelationshipType.ManyToMany,
                entityA, entityB, string.Empty, string.Empty, schema);

        // ── Scenario: Table block per entity ────────────────────────────────────

        [Test]
        public void Export_SingleEntity_ContainsTableBlock()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid"), Attr("name") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("Table account"));
        }

        [Test]
        public void Export_MultipleEntities_ContainsOneTablePerEntity()
        {
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("Table account"));
            Assert.That(result, Contains.Substring("Table contact"));
        }

        // ── Scenario: [pk] marker ────────────────────────────────────────────────

        [Test]
        public void Export_PrimaryIdAttribute_HasPkMarker()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid"), Attr("name") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("[pk]"));
        }

        [Test]
        public void Export_NonPrimaryAttribute_DoesNotHavePkMarker()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid"), Attr("name") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            var lines = result.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("name") && !line.Contains("accountid"))
                    Assert.That(line, Does.Not.Contain("[pk]"));
            }
        }

        // ── Scenario: Ref for 1:N relationships ─────────────────────────────────

        [Test]
        public void Export_OneToManyRelationship_ContainsRefLine()
        {
            var rel = OneToMany("account", "contact", "accountid", "parentaccountid");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid"), Attr("parentaccountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("Ref:"));
            Assert.That(result, Contains.Substring("account.accountid"));
            Assert.That(result, Contains.Substring("contact.parentaccountid"));
        }

        // ── Scenario: N:N junction table ─────────────────────────────────────────

        [Test]
        public void Export_ManyToManyRelationship_ContainsJunctionTable()
        {
            var rel = ManyToMany("account", "contact", "accountcontact_junction");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("accountcontact_junction"));
            Assert.That(result, Contains.Substring("// N:N relationship"));
        }

        [Test]
        public void Export_ManyToManyRelationship_JunctionTableHasTwoForeignKeys()
        {
            var rel = ManyToMany("account", "contact", "accountcontact_junction");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("account_id"));
            Assert.That(result, Contains.Substring("contact_id"));
        }

        // ── Scenario: Empty selection ─────────────────────────────────────────────

        [Test]
        public void Export_EmptyList_ReturnsEmptyString()
        {
            var result = _exporter.Export(new List<EntityModel>());

            Assert.That(result, Is.Empty);
        }
    }
}
