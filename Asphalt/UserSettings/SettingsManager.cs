using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

// WIP
namespace Asphalt
{
    class SettingsManager
    {
        private static readonly string localPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");
        private static readonly string outsidePath;
        public static UserSettings Settings { get; set; }

        public static bool IsThereAnOutsideConfig
        {
            get
            {
                return File.Exists(outsidePath);
            }
        }

        public static bool IsThereALocalConfig
        {
            get
            {
                return File.Exists(localPath);
            }
        }

        public void Initialize()
        {
            if(IsThereAnOutsideConfig)
                Settings = LoadConfigFromFile(outsidePath);
            else if(IsThereALocalConfig)
                Settings = LoadConfigFromFile(localPath);
            // if neither exists, leave on defaults

            if (Settings.UseLocalFolder)
            {
                if (IsThereALocalConfig) return;
                if(IsThereAnOutsideConfig)
                {
                    // move these settings inside;
                }
            }
        }


        /// <summary>
        /// Writed every property in UserSettings into a JSON file
        /// </summary>
        public static void WriteSettingsToFile(string path)
        {
            try
            {
                using (var sw = new StreamWriter(path))
                {
                    var serializedUserSettings = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                    sw.Write(serializedUserSettings);
                    Log.Info($"config writtern to: {path}");
                }
            }
            catch (Exception e)
            {
                Log.Warning($"Couldn't write to {path}, {e.Message}");
            }

        }

        private static UserSettings LoadConfigFromFile(string path)
        {
            Log.Info("Loading config files");
            UserSettings userSettings = new UserSettings();
            try
            {
                using (var r = new StreamReader(path))
                {
                    var json = r.ReadToEnd();
                    userSettings = JsonConvert.DeserializeObject<UserSettings>(json);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error reading {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}, {e.Message}");
                return null;
            }

            return userSettings;
        }
    }
}
