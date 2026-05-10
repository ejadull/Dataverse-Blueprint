using System.Collections.Generic;
using DataverseBlueprint.Exporters;
using DataverseBlueprint.Models;
using NUnit.Framework;

namespace DataverseBlueprint.Tests.Exporters
{
    [TestFixture]
    public class PlantUmlExporterTests
    {
        private PlantUmlExporter _exporter;

        [SetUp]
        public void SetUp()
        {
            _exporter = new PlantUmlExporter();
        }

        private static AttributeModel PkAttr(string name) =>
            new AttributeModel(name, name, "UniqueIdentifier", isPrimaryId: true, isPrimaryName: false);

        private static AttributeModel Attr(string name, string type = "String") =>
            new AttributeModel(name, name, type, isPrimaryId: false, isPrimaryName: false);

        private static RelationshipModel OneToMany(
            string referencedEntity, string referencingEntity) =>
            new RelationshipModel(RelationshipType.OneToMany,
                referencedEntity, referencingEntity,
                $"{referencedEntity}id", $"parent{referencedEntity}id",
                $"{referencedEntity}_{referencingEntity}");

        // ── Scenario: @startuml / @enduml wrapper ────────────────────────────────

        [Test]
        public void Export_SingleEntity_StartsWithStartuml()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result.TrimStart(), Does.StartWith("@startuml"));
        }

        [Test]
        public void Export_SingleEntity_EndsWithEnduml()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result.TrimEnd(), Does.EndWith("@enduml"));
        }

        // ── Scenario: entity block ───────────────────────────────────────────────

        [Test]
        public void Export_SingleEntity_ContainsEntityBlock()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("entity account"));
        }

        [Test]
        public void Export_PrimaryIdAttribute_MarkedWithAsterisk()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid"), Attr("name") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            Assert.That(result, Contains.Substring("* accountid"));
        }

        [Test]
        public void Export_NonPrimaryAttribute_NoAsterisk()
        {
            var entity = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid"), Attr("name") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { entity });

            var lines = result.Split('\n');
            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("name"))
                    Assert.That(line, Does.Not.Contain("* name"));
            }
        }

        // ── Scenario: relationships ──────────────────────────────────────────────

        [Test]
        public void Export_OneToManyRelationship_ContainsRelationLine()
        {
            var rel = OneToMany("account", "contact");
            var account = new EntityModel("account", "Account",
                new List<AttributeModel> { PkAttr("accountid") },
                new List<RelationshipModel> { rel });
            var contact = new EntityModel("contact", "Contact",
                new List<AttributeModel> { PkAttr("contactid") },
                new List<RelationshipModel>());

            var result = _exporter.Export(new List<EntityModel> { account, contact });

            Assert.That(result, Contains.Substring("account"));
            Assert.That(result, Contains.Substring("contact"));
            Assert.That(result, Contains.Substring("}|"));
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
