using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModCharmStatus : ModCharm
    {
        public static ModCharmStatus Create(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats)
            => new ModCharmStatus().SetCharmStatus(name, maxLevel, stats);
        public static ModCharmStatus Create<T>(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_StatusInstance
            => new ModCharmStatus().SetCharmStatus<T>(name, maxLevel, stats);
        internal ModCharmStatus SetCharmStatus(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats)
            => SetCharmStatus<Charm_StatusInstance>(name, maxLevel, stats);
        internal ModCharmStatus SetCharmStatus<T>(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_StatusInstance
        {
            SetCharm<T>(name, maxLevel);
            Stats = stats;
            return this;
        }
        public Charm_StatusInstance.StatusGroup[] Stats { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var charm = o.AddComponent(CharmType) as Charm_StatusInstance;
            //Core.Logger($"CreateCharm");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            o.SetAssetId(AssetId);
            charm.maxLevel = MaxLevel;
            charm.effectsString = Effects;
            charm.isUniqueEffect = IsUniqueEffect;
            charm.stats = Stats;
            charm.isWeaponRelatedCharm = IsWeaponRelatedCharm;
            charm.relatedWeapon = RelatedWeapon;
            charm.enabled = false;
            return o;
        }
    }
}
