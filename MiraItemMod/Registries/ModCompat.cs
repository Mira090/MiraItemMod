using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiraItemMod.Registries
{
    public abstract class ModCompat
    {
        public static readonly List<ModCompat> ModCompats = new List<ModCompat>();
        public static bool TryGetCompat<T>(out T result) where T : ModCompat
        {
            result = null;
            foreach (var compat in ModCompats)
            {
                if (compat.GetType() == typeof(T))
                {
                    result = compat as T;
                    return true;
                }
            }
            return false;
        }
        public static T GetCompat<T>() where T : ModCompat
        {
            foreach (var compat in ModCompats)
            {
                if (compat.GetType() == typeof(T))
                    return compat as T;
            }
            return null;
        }
        public static bool GetHasLoaded<T>() where T : ModCompat
        {
            var compat = GetCompat<T>();
            if (compat == null)
                return false;
            return compat.HasLoaded;
        }
        public static void Initialize()
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
                compat.OnCompatLoaded();
            }
        }
        public static void Destroy()
        {
            foreach (var compat in ModCompats)
            {
                compat.OnCompatUnloaded();
            }
            ModCompats.Clear();
        }

        public abstract string ModName { get; }
        public abstract string ModId { get; }

        protected AddOnLoader.LoadedAddOn LoadedAddOnMod
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
        protected WorkshopModActivation LoadedWorkshopMod
        {
            get
            {
                foreach (var mod in WorkshopModRuntime.LastResults)
                {
                    if (mod.ModId == ModId)
                        return mod;
                }
                return null;
            }
        }
        public List<Assembly> GetAssemblies()
        {
            if (LoadedWorkshopMod != null)
                return LoadedWorkshopMod.Modules.Where(x => x.Assembly != null).Select(x => x.Assembly).ToList();
            if(LoadedAddOnMod != null)
                return new List<Assembly>() { LoadedAddOnMod.Assembly };
            return new List<Assembly>();
        }
        public bool HasLoaded
        {
            get
            {
                foreach (var mod in WorkshopModRuntime.LastResults)
                {
                    if (mod.ModId == ModId)
                        return true;
                }
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
            var assemblies = GetAssemblies();
            if(assemblies.Count == 0)
                return false;
            foreach(var assembly in assemblies)
            {
                var type = assembly.GetType(name);
                if (type == null)
                {
                    continue;
                }
                result = type;
                return true;
            }
            Core.LoggerWarning("ModCompat Faild Load: " + name);
            return false;
        }
        public bool TryGetType<T>(out Type result)
        {
            result = null;
            var assemblies = GetAssemblies();
            if (assemblies.Count == 0)
                return false;
            foreach(var assembly in assemblies)
            {
                var type = assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(T)));
                if (type == null)
                    continue;
                result = type;
                return true;
            }
            return false;
        }

        public ModCompat()
        {
            Core.LoggerFew("Compat for " + ModName);
            OnInitialized();
        }
        public virtual void OnInitialized()
        {

        }
        /// <summary>
        /// OnModLoadedと同じタイミング。イベントのサブスク用
        /// </summary>
        public virtual void OnCompatLoaded()
        {

        }
        /// <summary>
        /// OnModUnloadedと同じタイミング。イベントのサブスク解除用
        /// </summary>
        public virtual void OnCompatUnloaded()
        {

        }
    }
}
