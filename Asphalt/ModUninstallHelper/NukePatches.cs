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
                // put all of this somewhere else later

                float scale = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();

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


     
        [HarmonyPatch(typeof(Debug), "Assert", typeof(bool))]
        internal static class Debug_Assert_Patch
        {
            internal static void Prefix(bool condition)
            {
                if (!condition)
                {
                    var stack = new StackTrace();
                    if (stack.FrameCount > 2)
                    {
                        var callName = stack.GetFrame(2).GetMethod().FullDescription();
                        Debug.LogWarning(
                            $"[AssertFinder] The following method called an Assert that is about to fail:\n{callName}");
                    }
                }
            }
        }

    }
}
