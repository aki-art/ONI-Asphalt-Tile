using System.IO;
using System.Reflection;
using UnityEngine;

namespace Asphalt
{
    public static class CustomAssets
    {
        /* TODO
         * - Not reload existing assets (eg fonts)
         * - Sounds
         */

        public static AssetBundle AssetBundle { get; set; }
        public static GameObject NukeModDialogPrefab { get; set; }
        public static GameObject ModSettingsScreenPrefab { get; set; }

        public static void LoadAssetBundle(string path, string filename)
        {
            Log.Info("Loading asset files... ");
            string fullPath = Path.Combine(path, filename);
            AssetBundle = AssetBundle.LoadFromFile(fullPath);
            if (AssetBundle == null) Log.Warning($"Failed to load AssetBundle from path {fullPath}");
            else
            {
                ModSettingsScreenPrefab = AssetBundle.LoadAsset<GameObject>("SettingsCanvas");
                NukeModDialogPrefab = AssetBundle.LoadAsset<GameObject>("NukeDialog");
            }
        }
    }
}
