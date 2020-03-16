using Harmony;
using PeterHan.PLib;
using PeterHan.PLib.Options;
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
        public static string ModPath { get; set; }
        public static class Mod_OnLoad
        {
            public static void OnLoad(string path)
            {
                ModPath = path;
                PUtil.InitLibrary(true);
                POptions.RegisterOptions(typeof(UserSettings));
                POptions.ReadSettings<UserSettings>();

                ModAssets.LoadAll();
            }
        }

        // Add building strings, add it to building plan
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                string ID = AsphaltConfig.ID.ToUpperInvariant();
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.NAME", AsphaltConfig.NAME);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.EFFECT", AsphaltConfig.EFFECT);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID}.DESC", AsphaltConfig.DESC);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.OILREFINERY.EFFECT", $"Converts {UI.FormatAsLink("Crude Oil", "CRUDEOIL")} into {UI.FormatAsLink("Petroleum", "PETROLEUM")}, {UI.FormatAsLink("Bitumen", "BITUMEN")} and {UI.FormatAsLink("Natural Gas", "METHANE")}.");

                ModUtil.AddBuildingToPlanScreen("Base", AsphaltConfig.ID);
            }
        }

        // Add Bitumen as edible for hatches
        [HarmonyPatch(typeof(BaseHatchConfig), "BasicRockDiet")]
        public static class HatchConfig_BasicRockDiet_Patch
        {
            public static void Postfix(List<Diet.Info> __result)
            {
                __result[0].consumedTags.Add(SimHashes.Bitumen.CreateTag());
            }
        }

        // Adding Asphalt tiles to the research tree.
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                var techList = new List<string>(Database.Techs.TECH_GROUPING["ImprovedCombustion"]) 
                { 
                    AsphaltConfig.ID 
                };
                Database.Techs.TECH_GROUPING["ImprovedCombustion"] = techList.ToArray();
            }
        }

        // Making the Oil refinery produce bitumen
        [HarmonyPatch(typeof(OilRefineryConfig), "ConfigureBuildingTemplate")]
        public static class OilRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go)
            {
                if(UserSettings.Instance.BitumenProduction)
                {
                    ElementDropper elementDropper = go.AddComponent<ElementDropper>();
                    elementDropper.emitMass = 100f;
                    elementDropper.emitTag = new Tag("Bitumen");
                    elementDropper.emitOffset = Vector3.zero;

                    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();

                    var bitumenOutput = new ElementConverter.OutputElement(
                        kgPerSecond: 5f,
                        element: SimHashes.Bitumen,
                        minOutputTemperature: 348.15f,
                        useEntityTemperature: false,
                        storeOutput: true,
                        outputElementOffsetx: 0,
                        outputElementOffsety: 1f );

                    // Pushing it into the outputElements array
                    Array.Resize(ref elementConverter.outputElements, elementConverter.outputElements.Length + 1);
                    elementConverter.outputElements[elementConverter.outputElements.GetUpperBound(0)] = bitumenOutput;
                }
            }
        }

        // Fixing up bitumen
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

                // Assigning appropiate tags
                bitumen.materialCategory = CreateMaterialCategoryTag(phaseTag, GameTags.ManufacturedMaterial.ToString());      // This tag is for storage
                bitumen.oreTags = new Tag[] { 
                    GameTags.ManufacturedMaterial,                // This tag is for the autosweeper
                    GameTags.BuildableAny,                        // This tag is for any building material category (Tempshift Plates, Wallpapers)
                    GameTags.Solid                                // This tag is for mod compatibilities
                };

                // Pickupable bitumen art
                KAnimFile animFile = Assets.Anims.Find(anim => anim.name == "solid_bitumen_kanim");

                // Assigning new material and texture
                if (ModAssets.bitumenSubstanceTexture == null)
                {
                    Log.Warning("Texture file not found for Bitumen.");
                    return;
                }

                var material = subTable.solidMaterial;
                material.mainTexture = ModAssets.bitumenSubstanceTexture;
                var darkSlateGreyColor = new Color32(65, 65, 79, 255);

                try
                {
                    Substance bitumensubstance = ModUtil.CreateSubstance(
                        name: "Bitumen",
                        state: Element.State.Solid,
                        kanim: animFile,
                        material: material,
                        colour: darkSlateGreyColor,
                        ui_colour: darkSlateGreyColor,
                        conduit_colour: darkSlateGreyColor
                        );

                    bitumen.substance = bitumensubstance;
                }
                catch(Exception e) {
                    Log.Error("Could not assign new material to Bitumen element: " + e);
                }
            }
        }

        private static Tag CreateMaterialCategoryTag(Tag phaseTag, string materialCategoryField)
        {
            if (string.IsNullOrEmpty(materialCategoryField))
                return phaseTag;

            return TagManager.Create(materialCategoryField);
        }

    }
}