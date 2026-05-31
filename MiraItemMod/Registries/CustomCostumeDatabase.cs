using FMODUnity;
using Newtonsoft.Json;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static AnimationSet.StateInfo;
using Event = AnimationSet.StateInfo.Event;

namespace MiraItemMod.Registries
{
    public static class CustomCostumeDatabase
    {
        private static Dictionary<string, CustomCostumeEntity> costumeDictionary = null;
        private static Dictionary<string, CustomCostumeMetadata> metadataDictionary = null;
        public static void Initialize()
        {
            costumeDictionary = new Dictionary<string, CustomCostumeEntity>();
            metadataDictionary = new Dictionary<string, CustomCostumeMetadata>();

            CustomCostumeEntity[] array = LoadAll(Path.Combine(Application.streamingAssetsPath, "Costume"));
            foreach (CustomCostumeEntity costumeEntity in array)
            {
                costumeDictionary[costumeEntity.id] = costumeEntity;
                Core.Logger("New Costume: " + costumeEntity.id);
                //costumeEntity.CreateGameObject();
            }
        }
        public static void InitializeGameObject(PlayerAvatarCostume example)
        {
            foreach (var entity in costumeDictionary.Values)
            {
                entity.CreateGameObject(example);
            }
        }

        public static void Destroy()
        {
            costumeDictionary = null;
            metadataDictionary = null;
        }

        public static CustomCostumeEntity FindCostumeByID(string id)
        {
            CustomCostumeEntity result;
            if (costumeDictionary.TryGetValue(id, out result))
            {
                return result;
            }
            return null;
        }

        public static IEnumerable<CustomCostumeEntity> GetAll()
        {
            return costumeDictionary.Values;
        }
        public static IEnumerable<CostumeEntity> CreateAll()
        {
            return GetAll().Select(x => x.ToNormal());
        }
        public static IEnumerable<CostumeSkinEntity> CreateAllSkin()
        {
            return GetAll().Select(x => x.ToSkin());
        }
        public static void LoadAllStartingItems(IEnumerable<CostumeEntity> entities)
        {
            Core.LoggerFew("Loading... CustomeCostume StartingItems");
            foreach(var entity in entities)
            {
                if (metadataDictionary.ContainsKey(entity.id) && metadataDictionary[entity.id].startingItems != null)
                {
                    entity.startingItems = metadataDictionary[entity.id].startingItems.Select(ItemDatabase.FindItemById).Where(x => x != null).ToArray();
                }
            }
        }
        public static CostumeEntity ToNormal(this CustomCostumeEntity custom)
        {
            var entity = ScriptableObject.CreateInstance<CostumeEntity>();
            entity.name = custom.name;
            entity.id = custom.id;
            entity.icon = custom.animationData[0].timeline[0].sprite;
            entity.stats = custom.stats;
            entity.costumePrefab = custom.costumePrefab;
            entity.aName = new LocalizedString(custom.costumeName);
            entity.aFlavorText = new LocalizedString(custom.costumeFlavorText);
            entity.order = 500;
            entity.costumeType = ECostumeUnlockType.Default;
            return entity;
        }
        public static CostumeSkinEntity ToSkin(this CustomCostumeEntity custom)
        {
            var entity = ScriptableObject.CreateInstance<CostumeSkinEntity>();
            entity.name = custom.name;
            entity.relatedCostumeID = custom.id;
            entity.skinID = custom.id + "_Skin";
            entity.icon = custom.animationData[0].timeline[0].sprite;
            entity.purchasePrice = 30;
            entity.skinPrefab = custom.costumePrefab;
            entity.aName = new LocalizedString(custom.costumeName);
            entity.unlockType = CostumeSkinEntity.ECostumeUnlockType.Default;
            return entity;
        }

        public static readonly string Metadata = "Metadata.json";
        public static CustomCostumeEntity[] LoadAll(string path)
        {
            Core.LoggerFew("LoadAll: " + path);
            var paths = Directory.GetDirectories(path);
            var list = new List<CustomCostumeEntity>();
            foreach(var p in paths)
            {
                var entity = Load(p);
                if(entity != null)
                    list.Add(entity);
            }
            return list.ToArray();
        }
        public static CustomCostumeEntity Load(string path)
        {
            Core.LoggerFew("Load: " + path);
            var metadataPath = Path.Combine(path, Metadata);
            if (!File.Exists(metadataPath))
            {
                Core.LoggerWarning("Metadata.json is not exist: " + metadataPath);
                return null;
            }
            var metadata = LoadMetadata(metadataPath);
            if (metadata.id == "SuperDucki")
                return null;
            var entity = MetadataToEntity(metadata, path);
            if (entity != null)
                metadataDictionary[entity.id] = metadata;
            return entity;
        }
        public static CustomCostumeMetadata LoadMetadata(string path)
        {
            Core.LoggerFew("LoadMetadata: " + path);
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream);
            return JsonConvert.DeserializeObject<CustomCostumeMetadata>(streamReader.ReadToEnd());
        }
        public static CustomCostumeEntity MetadataToEntity(CustomCostumeMetadata metadata, string dir)
        {
            var log = false;
            Core.Logger("MetadataToEntity: " + dir);
            var entity = ScriptableObject.CreateInstance<CustomCostumeEntity>();
            entity.name = metadata.id;
            entity.comment = metadata.comment;
            if (log)
                Core.Logger("MetadataToEntity 0");
            entity.id = metadata.id;
            entity.costumeName = metadata.costumeName;
            entity.costumeFlavorText = metadata.costumeFlavorText;
            entity.hitboxSize = metadata.hitboxSize;
            entity.movementCollisionRadius = (float)metadata.movementCollisionRadius;
            entity.hasBackImage = metadata.hasBackImage;
            if (log)
                Core.Logger("MetadataToEntity 1");
            if (metadata.stats != null)
                entity.stats = metadata.stats.ToArray();
            else
                entity.stats = new string[0];
            if (log)
                Core.Logger("MetadataToEntity 2");

            var state = new List<AnimationSet.StateInfo>();
            foreach(var animation in metadata.animationData)
            {
                if (log)
                    Core.Logger("MetadataToEntity 3");
                var info = new AnimationSet.StateInfo();
                info.state = animation.state;
                info.fps = animation.fps;
                info.repeat = animation.repeat;

                info.timeline = new List<SpriteKeyFrame>();
                if (log)
                    Core.Logger("MetadataToEntity 4");
                foreach (var time in animation.timeline)
                {
                    var timeline = new SpriteKeyFrame();
                    timeline.frameIdx = time.frameIdx;
                    timeline.sprite = AssetLoader.LoadSpriteForCostume(Path.Combine(dir, time.sprite));
                    info.timeline.Add(timeline);
                }

                if (log)
                    Core.Logger("MetadataToEntity 5");
                if(animation.frameEvents != null)
                {
                    info.frameEvents = new List<FrameEvent>();
                    foreach (var frame in animation.frameEvents)
                    {
                        var frameEvent = new FrameEvent();
                        frameEvent.frame = frame.frame;

                        frameEvent.events = new List<Event>();
                        foreach (var ev in frame.events)
                        {
                            var even = new Event();
                            even.methodName = ev.methodName;
                            even.componentName = ev.componentName;
                            even.priority = ev.priority == "Essential" ? Event.EPriority.Essential : Event.EPriority.Ignorable;
                            frameEvent.events.Add(even);
                        }
                        info.frameEvents.Add(frameEvent);
                    }
                }

                if (log)
                    Core.Logger("MetadataToEntity 6");
                if (animation.soundEvents != null)
                {
                    info.soundEvents = new List<FrameSoundEvent>();
                    foreach (var sound in animation.soundEvents)
                    {
                        var soundEvent = new FrameSoundEvent();
                        soundEvent.frame = sound.frame;
                        soundEvent.events = new List<SoundEvent>();
                        foreach (var ev in sound.events)
                        {
                            var even = new SoundEvent();
                            even.attachToPerformer = ev.attachToPerformer;
                            even.path = RuntimeManager.PathToEventReference(ev.path);
                            soundEvent.events.Add(even);
                        }
                        info.soundEvents.Add(soundEvent);
                    }
                }
                state.Add(info);
            }
            if (log)
                Core.Logger("MetadataToEntity 7");
            entity.animationData = state.ToArray();

            if (log)
                Core.Logger("MetadataToEntity 8");

            return entity;
        }
        public static GameObject CreateGameObject(this CustomCostumeEntity entity)
        {
            var example = CostumeDatabase.GetAll().First().costumePrefab.GetComponent<PlayerAvatarCostume>();

            var clone = UnityEngine.Object.Instantiate(example);
            clone.gameObject.name = entity.id;
            var animator = clone.gameObject.GetComponent<Animator2D_MultipleSpriteRenderer>();

            var set = ScriptableObject.CreateInstance<AnimationSet>();
            set.name = entity.id;
            set.sprites = entity.animationData.ToList();
            animator.ChangeSet(set);
            clone.movementColliderRadius = entity.movementCollisionRadius;
            var collider = clone.hitbox.gameObject.GetComponent<CircleCollider2D>();

            collider.radius = 0.25f * entity.hitboxSize;
            collider.offset = new Vector2(0, 0.25f * entity.hitboxSize);

            entity.costumePrefab = clone.gameObject;
            return clone.gameObject;
        }
        public static GameObject CreateGameObject(this CustomCostumeEntity entity, PlayerAvatarCostume example)
        {
            var clone = UnityEngine.Object.Instantiate(example);
            clone.gameObject.name = entity.id;
            var animator = clone.gameObject.GetComponent<Animator2D_MultipleSpriteRenderer>();

            var set = ScriptableObject.CreateInstance<AnimationSet>();
            set.name = entity.id;
            set.sprites = entity.animationData.ToList();
            animator.ChangeSet(set);
            clone.movementColliderRadius = entity.movementCollisionRadius;
            var collider = clone.hitbox.gameObject.GetComponent<CircleCollider2D>();

            collider.radius = 0.25f * entity.hitboxSize;
            collider.offset = new Vector2(0, 0.25f * entity.hitboxSize);

            entity.costumePrefab = clone.gameObject;
            return clone.gameObject;
        }
    }
}
