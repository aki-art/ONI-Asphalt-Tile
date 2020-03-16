using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Asphalt
{
    public static class Nuker
    {
        private static BuildingDef sandstoneDef = Assets.GetBuildingDef(TileConfig.ID);

        /// <summary>
        /// Spawns a single Sandstone tile
        /// </summary>
        private static void SpawnSandstoneTile(int cell, Orientation buildingOrientation, Storage _, IList<Tag> selectedElements, float temperature)
        {
            sandstoneDef.Build(cell, buildingOrientation, null, selectedElements, temperature, false, GameClock.Instance.GetTime() + 1);
            World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, cell);
        }

        /// <summary>
        /// Replaces a single tile
        /// </summary>
        /// <param name="building"></param>
        private static void ChangeTileToSandstone(BuildingComplete building)
        {
            // Save the asphalt tile-s information
            Orientation buildingOrientation = building.Orientation;
            var selectedElements = new List<Tag>() { "SandStone".ToTag() };
            float temperature = building.primaryElement.Temperature;
            int cell = Grid.PosToCell(building.transform.GetPosition());

            // Replacing SimCell Occupier
            SimCellOccupier simCellOccupier = building.GetComponent<SimCellOccupier>();
            simCellOccupier.DestroySelf(delegate { SpawnSandstoneTile(cell, buildingOrientation, null, selectedElements, temperature); });

            // Destroy Asphalt
            HashSetPool<GameObject, SandboxDestroyerTool>.PooledHashSet objects_to_destroy = HashSetPool<GameObject, SandboxDestroyerTool>.Allocate();
            objects_to_destroy.Add(building.gameObject);
            Util.KDestroyGameObject(building.gameObject);
            objects_to_destroy.Recycle();
        }

        /// <summary>
        /// Loops through all completed buildings, and replaces all tiles
        /// </summary>
        public static void ChangeAllAsphaltToSandstoneTiles()
        {
            Log.Info("Changing tiles");

            try
            {
                foreach (BuildingComplete building in Components.BuildingCompletes)
                {
                    if (building.name == AsphaltConfig.ID + "Complete")
                    {
                        ChangeTileToSandstone(building);

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
