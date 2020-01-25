using System;
using System.IO;
using TUNING;
using UnityEngine;

namespace Asphalt
{
    public class AsphaltConfig : IBuildingConfig
    {
        public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_bunker_tops");
        public const string ID = "AsphaltTile";

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "floor_mesh_kanim",
                hitpoints: 100,
                construction_time: 30f,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: new string[1] { "Bitumen" },
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Tile,
                decor: BUILDINGS.DECOR.PENALTY.TIER0,
                noise: NOISE_POLLUTION.NONE
            );

            BuildingTemplates.CreateFoundationTileDef(buildingDef);

            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.isKAnimTile = true;
            buildingDef.isSolidTile = true;
            buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");

            BlockTileDecorInfo decorBlockTileInfo = UnityEngine.Object.Instantiate(Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_info"));
            decorBlockTileInfo.atlas = GetCustomAtlas("anim\\assets\\tiles_asphalt_tops", this.GetType(), decorBlockTileInfo.atlas);

            buildingDef.BlockTileAtlas = GetCustomAtlas("anim\\assets\\tiles_asphalt", this.GetType(), Assets.GetTextureAtlas("tiles_metal"));
            buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_mesh_place");
            buildingDef.DecorBlockTileInfo = decorBlockTileInfo;
            buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_place_info");

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<SimCellOccupier>().doReplaceElement = false;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = MeshTileConfig.BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.movementSpeedMultiplier = 3.0f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles, false);
            go.AddComponent<SimTemperatureTransfer>();
            go.AddComponent<ZoneTile>();
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
        }

        // This code is courtesy of CynicalBusiness
        // Original proof of concept: https://lab.vevox.io/games/oxygen-not-included/matts-mods/blob/master/IndustrializationFundementals/Building/TileWoodConfig.cs
        public static TextureAtlas GetCustomAtlas(string name, Type type, TextureAtlas tileAtlas)
        {
            var dir = Path.GetDirectoryName(type.Assembly.Location);
            var texFile = Path.Combine(dir, name + ".png");

            TextureAtlas atlas = null;

            if (File.Exists(texFile))
            {
                var data = File.ReadAllBytes(texFile);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                atlas = ScriptableObject.CreateInstance<TextureAtlas>();
                atlas.texture = tex;
                atlas.vertexScale = tileAtlas.vertexScale;
                atlas.items = tileAtlas.items;
            }
            else
                Debug.LogError($"ASPHALT: Could not load atlas image at path {texFile}.");

            return atlas;
        }


    }
}
