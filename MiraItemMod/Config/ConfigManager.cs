using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiraItemMod.Config
{
    public static class ConfigManager
    {
        public static readonly int CurrentConfigVersion = 2;
        public static string GetConfigPath()
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\" + "config.json";
            return path;
        }
        public static ModConfig Config { get; private set; }
        public static void Init(WorkshopModBase mod)
        {
            LoadConfig(mod);
        }
        public static void LoadConfig(WorkshopModBase mod)
        {
            Config = new ModConfig();
            Config.SetOptions(mod.Context.Options);
        }
    }
}
