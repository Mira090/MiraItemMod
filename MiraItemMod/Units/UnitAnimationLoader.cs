using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Units
{
    public static class UnitAnimationLoader
    {
        public static UnitAnimationMetadata LoadMetadata(ModUnit mod)
        {
            return LoadMetadata(AssetLoader.GetAssetsFolder(ModUtil.UnitPath + mod.Name + "\\Metadata.json"));
        }
        public static UnitAnimationMetadata LoadMetadata(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream);
            var metadata = JsonConvert.DeserializeObject<UnitAnimationMetadata>(streamReader.ReadToEnd());
            streamReader.Close();
            fileStream.Close();
            return metadata;
        }
        public static string SaveMetadata(int id)
        {
            var item = ItemDatabase.FindItemById(id);
            if (item == null)
                return "Item Not Found";
            if(item.resourcePrefab == null)
                return "Item Prefab Not Found";
            GameObject unitPrefab = null;
            if (item.resourcePrefab.TryGetComponent<Charm_SummonUnit>(out var summon))
            {
                unitPrefab = summon.unitPrefab;
            }
            else if (item.resourcePrefab.TryGetComponent<Charm_MiniBallista>(out var ballista))
            {
                unitPrefab = ballista.unitPrefab;
            }
            else
            {
                return "Summon Unit Not Found";
            }
            if (unitPrefab == null)
                return "Unit Prefab Not Found";
            if(!unitPrefab.TryGetComponent<TopdownActorRenderingMetadata>(out var actor))
                return "TopdownActor Not Found";
            SaveMetadata(actor.animator.currentSet, item.name);
            return "Success";
        }
        public static void SaveMetadata(AnimationSet set, string folder)
        {
            var metadata = new UnitAnimationMetadata();
            metadata.name = folder;
            metadata.animationData = new List<UnitAnimationMetadata.AnimationDatum>();
            foreach (var state in set.sprites)
            {
                var data = new UnitAnimationMetadata.AnimationDatum();
                data.state = state.state;
                data.repeat = state.repeat;
                data.fps = state.fps;
                data.frameEvents = state.frameEvents.Select(x => new UnitAnimationMetadata.FrameEvent() { frame = x.frame, events = x.events.Select(y => new UnitAnimationMetadata.Event() { componentName = y.componentName, methodName = y.methodName, priority = y.priority.ToString() }).ToList() }).ToList();
                data.soundEvents = state.soundEvents.Select(x => new UnitAnimationMetadata.SoundEvent() { frame = x.frame, events = x.events.Select(y => new UnitAnimationMetadata.SEvent() { attachToPerformer = y.attachToPerformer, path = y.path.GUIDToPath() }).ToList() }).ToList();
                data.timeline = new List<UnitAnimationMetadata.Timeline>();
                foreach (var sprite in state.timeline)
                {
                    AssetLoader.SaveSprite(sprite.sprite, System.IO.Path.Combine(folder, sprite.sprite.name));
                    data.timeline.Add(new UnitAnimationMetadata.Timeline() { frameIdx = sprite.frameIdx, sprite = sprite.sprite.name + ".png" });
                }
                metadata.animationData.Add(data);
            }
            var json = JsonConvert.SerializeObject(metadata, Formatting.Indented);

            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.IO.DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = System.IO.Path.Combine(dllDirectory, "Outputs", folder, "Metadata.json");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(json);
            }
        }
    }
}
