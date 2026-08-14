using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiraItemMod.Config
{
    public static class ConfigManager
    {
        public static readonly int CurrentConfigVersion = 1;
        public static string GetConfigPath()
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\" + "config.json";
            return path;
        }
        public static ModConfig Config { get; private set; }
        public static void Init()
        {
            LoadConfig();
        }
        public static void LoadConfig()
        {
            Config = null;
            try
            {
                using FileStream stream = new FileStream(GetConfigPath(), FileMode.Open, FileAccess.Read);
                using StreamReader streamReader = new StreamReader(stream);
                Config = JsonConvert.DeserializeObject<ModConfig>(streamReader.ReadToEnd());
            }
            catch (Exception exception)
            {
                Core.LoggerWarning(exception);
            }
            if (Config == null || Config.Version < CurrentConfigVersion)
                SaveConfig();
        }
        public static void SaveConfig()
        {
            Config ??= new ModConfig();
            Config.Version = CurrentConfigVersion;

            try
            {
                string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                using StreamWriter wr = new StreamWriter(GetConfigPath(), false);
                wr.WriteLine(json);
            }
            catch (Exception exception)
            {
                Core.LoggerError(exception);
            }
        }
    }
}
