using Harmony;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class NukePatches
    {
        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("OnSpawn")]
        public static class World_OnLoadLevel_Patch
        {
            public static void Postfix()
            {
                if(SettingsManager.TempSettings.NukeAsphaltTiles)
                { 
                    if (ModAssets.Prefabs.nukeScreenPrefab == null) 
                    { 
                        Log.Warning("Could not display UI: Nuke screen prefab is null"); 
                        return; 
                    }

                    Transform parent = UIHelper.GetACanvas("AsphaltNuke").transform;
                    GameObject nukeScreen = UnityEngine.Object.Instantiate(ModAssets.Prefabs.nukeScreenPrefab.gameObject, parent);
                    NukeScreen nukeScreenComponent = nukeScreen.AddComponent<NukeScreen>();
                    nukeScreenComponent.ShowDialog();
                }
            }
        }


    }
}
