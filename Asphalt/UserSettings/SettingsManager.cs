using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

// WIP
namespace Asphalt
{
    class SettingsManager
    {
        public static string localPath;
        public static string exteriorPath;
        private const string FILE_NAME = "config.json";
        private const string FOLDER = "settings/AsphaltTiles";
        public static UserSettings Settings { get; set; }

        public static bool IsThereAnOutsideConfig
        {
            get
            {
                return File.Exists(exteriorPath);
            }
        }

        public static bool IsThereALocalConfig
        {
            get
            {
                return File.Exists(localPath);
            }
        }

        public static void Initialize()
        {
            localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            exteriorPath = GetDirectory();

            Log.Debuglog("Config save folder in case of local: " + localPath);
            Log.Debuglog("Config save folder in case of exterior: " + exteriorPath);

            if (IsThereAnOutsideConfig)
                Settings = LoadConfigFromFile(exteriorPath);
            else if (IsThereALocalConfig)
                Settings = LoadConfigFromFile(localPath);
            else Settings = new UserSettings();
            // Leave at defaults otherwise

            if (Settings.UseLocalFolder)
            {
                if (IsThereAnOutsideConfig)
                {
                    if (!IsThereALocalConfig)
                    {
                        Log.Info($"Removed settings file from {exteriorPath}, saving settings from now on at {localPath}");
                        Directory.Delete(exteriorPath);
                        WriteSettingsToFile(localPath);
                    }
                    else
                    {
                        Log.Info($"Removed settings file from {exteriorPath}. Local config file found.");
                        Directory.Delete(exteriorPath);
                    }
                }
            }
        }
        public static string GetDirectory()
        {
            return Path.Combine(Util.RootFolder(), FOLDER);
        }

        // Called from UI
        public static void SaveSettings()
        {
            if (Settings.UseLocalFolder)
                WriteSettingsToFile(localPath);
            else
                WriteSettingsToFile(exteriorPath);
        }

        // Write every property in UserSettings into a JSON file
        public static void WriteSettingsToFile(string path)
        {
            var filePath = Path.Combine(path, FILE_NAME);
            try
            {
                if (!Directory.Exists(path) && path == exteriorPath)
                    Directory.CreateDirectory(path);

                using (var sw = new StreamWriter(filePath))
                {
                    var serializedUserSettings = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                    sw.Write(serializedUserSettings);
                    Log.Info($"Settings saved to: {filePath}");
                }
            }
            catch (Exception e)
            {
                Log.Warning($"Couldn't write to {filePath}, {e.Message}");
            }

        }

        private static UserSettings LoadConfigFromFile(string path)
        {
            var filePath = Path.Combine(path, FILE_NAME);
            Log.Debuglog("Loading config files from: " + filePath);
            UserSettings userSettings = new UserSettings();
           /* try
            {
                using (var r = new StreamReader(filePath))
                {
                    var json = r.ReadToEnd();
                    userSettings = JsonConvert.DeserializeObject<UserSettings>(json);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error reading {filePath}, {e.Message}");
                return null;
            }
*/
            return userSettings;
        }
    }
}
