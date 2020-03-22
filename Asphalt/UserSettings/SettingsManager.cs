using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

// WIP
namespace Asphalt
{
    class SettingsManager
    {
        private const string SETTINGSFOLDER = "settings";
        private const string FOLDERNAME = "AsphaltTiles";
        private const string FILE_NAME = "config.json";

        public static string localPath;
        public static string exteriorPath;
        public static bool isThereAnOutsideConfig = false;
        public static bool isThereALocalConfig = false;

        public static UserSettings Settings { get; set; }
        public static UserSettings DefaultSettings { get; set; }
        public static UserSettings LoadedSettings { get; set; }
        public static TempSettings TempSettings { get; set; } // settings that dont get saved


        public static void Initialize()
        {
            localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            exteriorPath = GetDirectory();

            isThereAnOutsideConfig = File.Exists(Path.Combine(localPath, FILE_NAME));
            isThereAnOutsideConfig = File.Exists(Path.Combine(exteriorPath, FILE_NAME));

            Log.Debuglog("Config save folder in case of local: " + localPath);
            Log.Debuglog("Config save folder in case of exterior: " + exteriorPath);

            LoadSettings();
        }

        public static void LoadSettings()
        {
            if (isThereAnOutsideConfig)
                Settings = LoadSettingsFromFile(exteriorPath);
            else if (isThereALocalConfig)
                Settings = LoadSettingsFromFile(localPath);
            else Settings = new UserSettings();

            LoadedSettings = Settings.Clone();
            DefaultSettings = new UserSettings();
            TempSettings = new TempSettings();
        }

        public static void SaveSettings()
        {
            if (Settings.UseLocalFolder)
            {
                if (isThereAnOutsideConfig)
                {
                    Log.Info($"Removed settings file from {exteriorPath}.");
                    RemoveModSettingsFolder();
                }

                WriteSettingsToFile(localPath);
            }
            else
                WriteSettingsToFile(exteriorPath);
        }

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
        private static void RemoveModSettingsFolder()
        {
            Directory.Delete(exteriorPath, true); // Removes Asphalt Tiles folder

            // .../Klei/OxygenNotIncluded/mods/settings 
            string settingsPath = Path.Combine(Util.RootFolder(), "mods", SETTINGSFOLDER);

            // making sure no other mods were using the settings folder, only then delete that folder too
            if (IsDirectoryEmpty(settingsPath)) 
            {
                Directory.Delete(settingsPath, true);
                Log.Info($"Removed settings file from {settingsPath}, saving settings from now on at {localPath}");
            }
            else
                Log.Info($"Removed settings file from {exteriorPath}, saving settings from now on at {localPath}");
        }


        private static UserSettings LoadSettingsFromFile(string path)
        {
            var filePath = Path.Combine(path, FILE_NAME);
            Log.Debuglog("Loading config files from: " + filePath);
            UserSettings userSettings = new UserSettings();

            try
            {
                using (var r = new StreamReader(filePath))
                {
                    var json = r.ReadToEnd();
                    userSettings = JsonConvert.DeserializeObject<UserSettings>(json);
                }
            }
            catch (Exception e)
            {
                Log.Warning($"Couldn't read {filePath}, {e.Message}. Using default settings.");
                return new UserSettings();
            }

            return userSettings;
        }
        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        public static string GetDirectory()
        {
            return Path.Combine(Util.RootFolder(), "mods", SETTINGSFOLDER, FOLDERNAME);
        }

    }
}
