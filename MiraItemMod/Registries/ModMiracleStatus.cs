using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModMiracleStatus : ModMiracle
    {
        public static ModMiracleStatus Create(string name, params Miracle_StatusInstance.StatInfo[] stats)
            => new ModMiracleStatus().SetMiracleStatus<Miracle_StatusInstance>(name, stats);
        public static ModMiracleStatus Create<T>(string name, params Miracle_StatusInstance.StatInfo[] stats) where T : Miracle_StatusInstance
            => new ModMiracleStatus().SetMiracleStatus<T>(name, stats);
        internal ModMiracleStatus SetMiracleStatus<T>(string name, params Miracle_StatusInstance.StatInfo[] stats) where T : Miracle_StatusInstance
        {
            SetItem(name);

            MiracleType = typeof(T);
            Stats = stats;

            return this;
        }
        public Miracle_StatusInstance.StatInfo[] Stats { get; internal set; }
        public bool AutoGenerateEffectString { get; internal set; } = true;
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject("TMiracle_" + Name);
            var miracle = o.AddComponent(MiracleType) as Miracle_StatusInstance;
            //Core.Logger($"CreateMiracleStatus");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            o.SetAssetId(AssetId);
            miracle.id = Id;
            miracle.aName = LocalizedName;
            miracle.giveItem = GiveItem;
            miracle.categories = Categories;
            miracle.miracleImage = MiracleImage ?? AssetLoader.LoadSprite(MiracleImageFileName);
            miracle.tier = Tier;
            miracle.effects = Effects;
            miracle.statInfo = Stats;
            miracle.autoGenerateEffectString = AutoGenerateEffectString;
            //miracle.enabled = false;
            return o;
        }
    }
}
