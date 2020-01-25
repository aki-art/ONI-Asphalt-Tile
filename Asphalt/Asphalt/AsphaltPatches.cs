using Harmony;
using STRINGS;
using System.Collections;
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

                ModUtil.AddBuildingToPlanScreen("Base", AsphaltConfig.ID);

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
                var tex = getTex("anim\\assets\\solid_bitumen");
                material.mainTexture = tex;

                Substance bitumensubstance = ModUtil.CreateSubstance(
                    name: "Bitumen",
                    state: Element.State.Solid,
                    kanim: animFile,
                    material: material,
                    colour: new Color32(255, 255, 255, 255), // set these later
                    ui_colour: new Color32(255, 255, 255, 255),
                    conduit_colour: new Color32(255, 255, 255, 255)
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