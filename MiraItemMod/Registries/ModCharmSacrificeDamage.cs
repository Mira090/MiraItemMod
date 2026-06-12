using MiraItemMod.Items.Sacrifice;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModCharmSacrificeDamage : ModCharmSacrifice
    {
        public static ModCharmSacrificeDamage Create(string name, Func<ItemEntity> reward, float damage, params Charm_StatusInstance.StatusGroup[] stats)
            => new ModCharmSacrificeDamage().SetCharmSacrifice(name, reward, damage, stats);
        public static ModCharmSacrificeDamage Create<T>(string name, Func<ItemEntity> reward, float damage, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_SacrificeDamage
            => new ModCharmSacrificeDamage().SetCharmSacrifice<T>(name, reward, damage, stats);
        internal ModCharmSacrificeDamage SetCharmSacrifice(string name, Func<ItemEntity> reward, float damage, params Charm_StatusInstance.StatusGroup[] stats)
            => SetCharmSacrifice<Charm_SacrificeDamage>(name, reward, damage, stats);
        internal ModCharmSacrificeDamage SetCharmSacrifice<T>(string name, Func<ItemEntity> reward, float damage, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_SacrificeDamage
        {
            SetCharmSacrifice<T>(name, reward, stats);
            RequiredDamage = damage;
            return this;
        }
        public float RequiredDamage { get; internal set; } = 1000f;
        public EDamageFromType? FromType { get; internal set; } = null;
        public EDamageElementalType? ElementalType { get; internal set; } = null;
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var charm = o.AddComponent(CharmType) as Charm_SacrificeDamage;
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
            //charm.rewardEntity = RewardEntity();
            charm.useFromType = FromType.HasValue;
            charm.useElementalType = ElementalType.HasValue;
            if (FromType.HasValue)
                charm.fromType = FromType.Value;
            if (ElementalType.HasValue)
                charm.elementalType = ElementalType.Value;
            charm.requiredDamage = RequiredDamage;
            charm.enabled = false;
            return o;
        }
    }
}
