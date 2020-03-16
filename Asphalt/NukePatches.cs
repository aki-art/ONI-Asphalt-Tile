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
                //ModSettingsScreen modSettingsScreen = new ModSettingsScreen();
                //KScreenManager.Instance.PushScreen(modSettingsScreen);
                ConfirmDialogScreen referenceScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GetSomeExistingCanvas(), true).GetComponent<ConfirmDialogScreen>();

                referenceScreen.PopupConfirmDialog("Reported Error", null, null, null, null, null, null, null, null, true);

                GameScheduler instance = GameScheduler.Instance;
                Action<object> callback;
                callback = delegate (object m)
                {
                    new NukeScreen();
                };
                //instance.Schedule("NukeAsphalt", 0, callback, null, null);

            }
        }

        private static GameObject GetSomeExistingCanvas()
        {
            GameObject parent;
            if (FrontEndManager.Instance != null)
            {
                parent = FrontEndManager.Instance.gameObject;
            }
            else
            {
                if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
                {
                    parent = GameScreenManager.Instance.ssOverlayCanvas;
                }
                else
                {
                    parent = new GameObject();
                    parent.name = "FileErrorCanvas";
                    UnityEngine.Object.DontDestroyOnLoad(parent);
                    Canvas canvas = parent.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
                    canvas.sortingOrder = 10;
                    parent.AddComponent<GraphicRaycaster>();
                }
            }

            return parent;
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
