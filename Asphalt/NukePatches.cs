using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asphalt
{
    class NukePatches
    {
        public static bool NukeAsphalt = false;
        private static BuildingDef sandstoneDef = Assets.GetBuildingDef(TileConfig.ID);

        [HarmonyPatch(typeof(BuildingComplete))]
        [HarmonyPatch("OnSpawn")]
        public static class BuildingComplete_OnSpawn_Patch
        {
            public static void Postfix(BuildingComplete __instance)
            {
              /*  if (!NukeAsphalt) return;
                if (__instance.name == AsphaltConfig.ID + "Complete")
                {
                    changeTileToSandstone(__instance);
                }*/
            }
        }

        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("OnSpawn")]
        public static class World_OnLoadLevel_Patch
        {
            public static void Postfix()
            {
                Log.Info("Scheduled");

                GameScheduler instance = GameScheduler.Instance;
                Action<object> callback;
                callback = delegate (object m)
                {
                    new NukeScreen();
                };
                instance.Schedule("NukeAsphalt", 0, callback, null, null);

            }
        }

        private static void spawnNewtile(int cell, Orientation buildingOrientation, Storage _, IList<Tag> selectedElements, float temperature)
        {
            sandstoneDef.Build(cell, buildingOrientation, null, selectedElements, temperature, false, GameClock.Instance.GetTime() + 1);
            World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, cell);
        }
        private static void changeTileToSandstone(BuildingComplete building)
        {
            // save the asphalt tile-s information
            Orientation buildingOrientation = building.Orientation;
            var selectedElements = new List<Tag>() { "SandStone".ToTag() };
            float temperature = building.primaryElement.Temperature;
            int cell = Grid.PosToCell(building.transform.GetPosition());
            SimCellOccupier simCellOccupier = building.GetComponent<SimCellOccupier>();
            simCellOccupier.DestroySelf(delegate { spawnNewtile(cell, buildingOrientation, null, selectedElements, temperature); });

            // destroy Asphalt
            HashSetPool<GameObject, SandboxDestroyerTool>.PooledHashSet objects_to_destroy = HashSetPool<GameObject, SandboxDestroyerTool>.Allocate();
            objects_to_destroy.Add(building.gameObject);
            Util.KDestroyGameObject(building.gameObject);
            objects_to_destroy.Recycle();

           
        }


        public static void ChangeAsphaltToSandstoneTiles()
        {

            Log.Info("Changing tiles");

            try
            {
                foreach (BuildingComplete building in Components.BuildingCompletes)
                {
                    if (building.name == AsphaltConfig.ID + "Complete")
                    {
                        changeTileToSandstone(building);

                    }
                }

            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            RebuildTiles();

        }

        private static void RebuildTiles()
        {
            for (int i = 0; i < Grid.CellCount; i++)
            {
                World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, i);
            }
            Log.Info("Rebuilt Tiles.");
        }
    }
}
