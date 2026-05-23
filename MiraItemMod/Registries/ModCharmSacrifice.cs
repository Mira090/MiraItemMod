using SephiriaMod.Items.Sacrifice;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModCharmSacrifice : ModCharmStatus
    {
        public static ModCharmSacrifice Create(string name, Func<ItemEntity> reward, params Charm_StatusInstance.StatusGroup[] stats)
            => new ModCharmSacrifice().SetCharmSacrifice(name, reward, stats);
        public static ModCharmSacrifice Create<T>(string name, Func<ItemEntity> reward, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_Sacrifice
            => new ModCharmSacrifice().SetCharmSacrifice<T>(name, reward, stats);
        internal ModCharmSacrifice SetCharmSacrifice(string name, Func<ItemEntity> reward, params Charm_StatusInstance.StatusGroup[] stats)
            => SetCharmSacrifice<Charm_Sacrifice>(name, reward, stats);
        internal ModCharmSacrifice SetCharmSacrifice<T>(string name, Func<ItemEntity> reward, params Charm_StatusInstance.StatusGroup[] stats) where T : Charm_Sacrifice
        {
            SetCharmStatus<T>(name, 0, stats);
            RewardEntity = reward;
            this.SetSacrifice();
            return this;
        }
        public Func<ItemEntity> RewardEntity { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var charm = o.AddComponent(CharmType) as Charm_Sacrifice;
            //Core.Logger($"CreateCharm");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            charm.maxLevel = MaxLevel;
            charm.effectsString = Effects;
            charm.isUniqueEffect = IsUniqueEffect;
            charm.stats = Stats;
            charm.isWeaponRelatedCharm = IsWeaponRelatedCharm;
            charm.relatedWeapon = RelatedWeapon;
            //dcharm.rewardEntity = RewardEntity();
            charm.enabled = false;
            return o;
        }
        public override void LateInit()
        {
            var charm = ResourcePrefab.GetComponent<Charm_Sacrifice>();
            charm.rewardEntity = RewardEntity();
        }
    }
}
