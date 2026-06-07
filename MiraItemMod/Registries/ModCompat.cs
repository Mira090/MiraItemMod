using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiraItemMod.Registries
{
    public abstract class ModCompat
    {
        public static readonly List<ModCompat> ModCompats = new List<ModCompat>();
        public static void LoadCompats()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach(var type in types)
            {
                if(type.IsSubclassOf(typeof(ModCompat)) && !type.IsAbstract)
                {
                    ModCompats.Add(Activator.CreateInstance(type) as ModCompat);
                }
            }
            foreach(var compat in ModCompats)
            {
                compat.OnModLoaded();
            }
        }
        public static void UnloadCompats()
        {
            foreach (var compat in ModCompats)
            {
                compat.OnModUnloaded();
            }
            ModCompats.Clear();
        }

        public abstract string ModName { get; }

        public AddOnLoader.LoadedAddOn LoadedMod
        {
            get
            {
                foreach(var mod in AddOnLoader.LoadedMods)
                {
                    if(mod.Metadata?.modName == ModName)
                        return mod;
                }
                return null;
            }
        }
        public bool HasLoaded
        {
            get
            {
                foreach (var mod in AddOnLoader.LoadedMods)
                {
                    if (mod.Metadata?.modName == ModName)
                        return true;
                }
                return false;
            }
        }
        public bool TryGetType(string name, out Type result)
        {
            result = null;
            var assembly = LoadedMod?.Assembly;
            if(assembly == null)
                return false;
            var type = assembly.GetType(name);
            if(type == null)
            {
                Core.LoggerWarning("ModCompat Faild Load: " + name);
                return false;
            }
            result = type;
            return true;
        }
        public bool TryGetType<T>(out Type result)
        {
            result = null;
            var assembly = LoadedMod?.Assembly;
            if (assembly == null)
                return false;
            var type = assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(T)));
            if (type == null)
                return false;
            result = type;
            return true;
        }

        public ModCompat()
        {
            Core.LoggerFew("Compat for " + ModName);
            OnInitialized();
        }
        public virtual void OnInitialized()
        {

        }
        public virtual void OnModLoaded()
        {

        }
        public virtual void OnModUnloaded()
        {

        }
    }
}
