using Harmony;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Asphalt
{
    class AsphaltPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                LocString NAME = UI.FormatAsLink("Asphalt Tile", nameof(AsphaltConfig.ID));
                LocString DESC = "Asphalt tiles feel great to run on.";
                LocString EFFECT = "Used to build the walls and floors of rooms.\n\nSubstantially increases Duplicant runspeed.";

                string ID = AsphaltConfig.ID.ToUpperInvariant();
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.NAME", NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.EFFECT", EFFECT);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.DESC", DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.OILREFINERY.EFFECT", $"Converts {UI.FormatAsLink("Crude Oil", "CRUDEOIL")} into {UI.FormatAsLink("Petroleum", "PETROLEUM")}, {UI.FormatAsLink("Bitumen", "BITUMEN")} and {UI.FormatAsLink("Natural Gas", "METHANE")}.");

                ModUtil.AddBuildingToPlanScreen("Base", AsphaltConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                var techList = new List<string>(Database.Techs.TECH_GROUPING["ImprovedCombustion"]) { AsphaltConfig.ID };
                Database.Techs.TECH_GROUPING["ImprovedCombustion"] = techList.ToArray();
            }
        }

        [HarmonyPatch(typeof(OilRefineryConfig), "ConfigureBuildingTemplate")]
        public static class OilRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go, Tag prefab_tag)
            {
                ElementDropper elementDropper = go.AddComponent<ElementDropper>();
                elementDropper.emitMass = 100f;
                elementDropper.emitTag = new Tag("Bitumen");
                elementDropper.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);

                ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
                var bitumenOutput = new ElementConverter.OutputElement(
                    kgPerSecond: 5f,
                    element: SimHashes.Bitumen,
                    minOutputTemperature: 348.15f,
                    useEntityTemperature: false,
                    storeOutput: true,
                    outputElementOffsetx: 0.0f,
                    outputElementOffsety: 1f,
                    diseaseWeight: 1f,
                    addedDiseaseIdx: 255,
                    addedDiseaseCount: 0);

                Array.Resize(ref elementConverter.outputElements, elementConverter.outputElements.Length + 1);
                elementConverter.outputElements[elementConverter.outputElements.GetUpperBound(0)] = bitumenOutput;

            }
        }

        [HarmonyPatch(typeof(ElementLoader), "Load")]
        private static class Patch_ElementLoader_Load
        {
            private static SubstanceTable subTable;

            private static void Prefix(ref Hashtable substanceList, SubstanceTable substanceTable)
            {
                subTable = substanceTable;
            }

            private static void Postfix()
            {
                Tag phaseTag = TagManager.Create("Solid");
                var bitumen = ElementLoader.FindElementByHash(SimHashes.Bitumen);
                bitumen.materialCategory = CreateMaterialCategoryTag(bitumen.id, phaseTag, "ManufacturedMaterial");

                var material = subTable.solidMaterial;
                KAnimFile animFile = Assets.Anims.Find(anim => anim.name == "solid_bitumen_kanim");
                var tex = getTex(Path.Combine(Path.Combine("anim", "assets"), "solid_bitumen"));
                material.mainTexture = tex;

                Substance bitumensubstance = ModUtil.CreateSubstance(
                    name: "Bitumen",
                    state: Element.State.Solid,
                    kanim: animFile,
                    material: material,
                    colour: new Color32(65, 65, 79, 255), 
                    ui_colour: new Color32(65, 65, 79, 255),
                    conduit_colour: new Color32(65, 65, 79, 255)
                    );

                bitumen.substance = bitumensubstance;
            }
        }

        private static Texture2D getTex(string name)
        {
            Texture2D tex = null;
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var texFile = Path.Combine(dir, name + ".png");

            if (File.Exists(texFile))
            {
                var data = File.ReadAllBytes(texFile);
                tex = new Texture2D(1, 1);
                tex.LoadImage(data);
            }
            else
                Debug.LogError($"ASPHALT: Could not load texture at path {texFile}.");
            return tex;
        }

        private static Tag CreateMaterialCategoryTag(
            SimHashes element_id,
            Tag phaseTag,
            string materialCategoryField)
        {
            if (string.IsNullOrEmpty(materialCategoryField))
                return phaseTag;
            Tag tag = TagManager.Create(materialCategoryField);
            if (!GameTags.MaterialCategories.Contains(tag) && !GameTags.IgnoredMaterialCategories.Contains(tag))
                Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", (object)element_id, (object)materialCategoryField);
            return tag;
        }
    }
}