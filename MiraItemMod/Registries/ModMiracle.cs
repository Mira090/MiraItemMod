using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModMiracle
    {
        public static ModMiracle Create(string name)
            => new ModMiracle().SetMiracle<Miracle>(name);
        public static ModMiracle Create<T>(string name) where T : Miracle
            => new ModMiracle().SetMiracle<T>(name);
        internal ModMiracle SetMiracle<T>(string name) where T : Miracle
        {
            SetItem(name);

            MiracleType = typeof(T);

            return this;
        }
        internal ModMiracle SetItem(string name)
        {
            Name = name;
            LocalizedName = new LocalizedString("Miracle_" + name + "_Name");
            MiracleImageFileName = ModUtil.MiraclePath + name;
            Id = name;
            Tier = Miracle.ETier.Tier1;
            GiveItem = Miracle.EItemGiveType.GiveItem;
            return this;
        }
        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public Miracle.EItemGiveType GiveItem { get; internal set; }
        public Miracle.ETier Tier { get; internal set; }
        public string MiracleImageFileName { get; internal set; }
        public Sprite MiracleImage { get; internal set; }
        public string[] Categories { get; internal set; } = new string[0];
        public Miracle.Effect[] Effects { get; internal set; } = new Miracle.Effect[0];
        public Func<ItemEntity[]> ManuallyGivenItems { get; internal set; }
        public Type MiracleType { get; internal set; }
        public GameObject Prefab { get; internal set; }
        public uint AssetId { get; internal set; }
        public void Init(uint assetId)
        {
            Prefab = CreateResourcePrefab();
            AssetId = assetId;
        }
        public virtual GameObject CreateResourcePrefab()
        {
            var o = new GameObject("TMiracle_" + Name);
            var miracle = o.AddComponent(MiracleType) as Miracle;
            //Core.Logger($"CreateMiracle");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            miracle.id = Id;
            miracle.aName = LocalizedName;
            miracle.giveItem = GiveItem;
            miracle.categories = Categories;
            miracle.miracleImage = MiracleImage ?? SpriteLoader.LoadSprite(MiracleImageFileName);
            miracle.tier = Tier;
            miracle.effects = Effects;
            //miracle.enabled = false;
            return o;
        }
        public virtual void LoadManuallyGivenItems()
        {
            if (Prefab == null || !Prefab.TryGetComponent<Miracle>(out var miracle))
                return;
            miracle.manuallyGivenItems = GetManuallyGivenItems();
        }
        protected virtual ItemEntity[] GetManuallyGivenItems()
        {
            if (ManuallyGivenItems == null)
                return new ItemEntity[0];
            return ManuallyGivenItems?.Invoke();
        }
    }
}
