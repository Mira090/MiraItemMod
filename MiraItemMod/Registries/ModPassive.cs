using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModPassive
    {
        public static ModPassive CreatePassive(string name, Color mainColor, params string[] stats)
        {
            return new ModPassive().SetPassive(name, mainColor, stats);
        }
        internal ModPassive SetPassive(string name, Color mainColor, string[] stats)
        {
            Name = name;
            aName = new LocalizedString($"Passive_{name}_Name");
            aDescription = new LocalizedString($"Passive_{name}_Description");
            IconImagePath = ModUtil.PassivePath + name;
            Add1ButtonImagePath = ModUtil.PassivePath + name + "_1";
            Add5ButtonImagePath = ModUtil.PassivePath + name + "_5";
            AddMinus1ButtonImagePath = ModUtil.PassivePath + name + "_1M";
            AddMinus5ButtonImagePath = ModUtil.PassivePath + name + "_5M";
            MainColor = mainColor;
            AddStats = stats;
            return this;
        }
        public string Name { get; internal set; }
        public ulong Id { get; internal set; }
        public int MaxLevel { get; internal set; } = 20;
        public LocalizedString aName { get; internal set; }
        public LocalizedString aDescription { get; internal set; }
        public bool isDefault { get; internal set; } = true;
        public string IconImagePath { get; internal set; }
        public string Add1ButtonImagePath { get; internal set; }
        public string Add5ButtonImagePath { get; internal set; }
        public string AddMinus1ButtonImagePath { get; internal set; }
        public string AddMinus5ButtonImagePath { get; internal set; }
        public Color MainColor { get; internal set; }
        public string[] AddStats { get; internal set; }

        public ModPassivePerk Lv5Perk { get; internal set; }
        public ModPassivePerk Lv10Perk { get; internal set; }
        public ModPassivePerk Lv20Perk { get; internal set; }

        public PassiveEntity PassiveEntity { get; internal set; }
        public void Init(ulong id, uint lv5assetId, uint lv10assetId, uint lv20assetId)
        {
            Id = id;
            Lv5Perk?.Init(lv5assetId);
            Lv10Perk?.Init(lv10assetId);
            Lv20Perk?.Init(lv20assetId);
            PassiveEntity = CreatePassiveEntity();
        }
        public PassiveEntity CreatePassiveEntity()
        {
            var entity = ScriptableObject.CreateInstance<PassiveEntity>();
            entity.name = Name;
            entity.id = Id;
            entity.maxLevel = MaxLevel;
            entity.aName = aName;
            entity.aDescription = aDescription;
            entity.isDefault = isDefault;
            entity.iconImage = AssetLoader.LoadSprite(IconImagePath);
            entity.add1ButtonImage = AssetLoader.LoadSprite(Add1ButtonImagePath);
            entity.add5ButtonImage = AssetLoader.LoadSprite(Add5ButtonImagePath);
            entity.addMinus1ButtonImage = AssetLoader.LoadSprite(AddMinus1ButtonImagePath);
            entity.addMinus5ButtonImage = AssetLoader.LoadSprite(AddMinus5ButtonImagePath);
            entity.mainColor = MainColor;
            entity.addStats = AddStats;
            entity.lv5PerkPrefab = Lv5Perk?.PerkPrefab;
            entity.lv10PerkPrefab = Lv10Perk?.PerkPrefab;
            entity.lv20PerkPrefab = Lv20Perk?.PerkPrefab;
            return entity;
        }
    }
}
