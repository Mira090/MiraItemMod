using MiraItemMod.Items;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModCharmOrphanedStatus : ModCharm
    {
        public static ModCharmOrphanedStatus Create(string name, params Charm_StatusInstance.StatusGroup[] stats)
            => new ModCharmOrphanedStatus().SetCharmStatus(name, 0, stats);
        public static ModCharmOrphanedStatus Create<T>(string name, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_AddOrphanedStatusInstance
            => new ModCharmOrphanedStatus().SetCharmStatus<T>(name, 0, stats);
        internal ModCharmOrphanedStatus SetCharmStatus(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats)
            => SetCharmStatus<Charm_AddOrphanedStatusInstance>(name, maxLevel, stats);
        internal ModCharmOrphanedStatus SetCharmStatus<T>(string name, int maxLevel, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_AddOrphanedStatusInstance
        {
            SetCharm<T>(name, maxLevel);
            Stats = stats;
            return this;
        }
        public Charm_StatusInstance.StatusGroup[] Stats { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var charm = o.AddComponent(CharmType) as Charm_AddOrphanedStatusInstance;
            //Core.Logger($"CreateCharm");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
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
