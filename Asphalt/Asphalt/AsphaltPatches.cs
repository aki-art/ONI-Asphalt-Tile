using Harmony;
using STRINGS;

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
    }
}