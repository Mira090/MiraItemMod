using MiraItemMod.Buffs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_StealLeaf : WeaponAddon
    {
        public int MoneySteal = 2;
        public int thorns = 2;
        public int applied = 0;
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("LEAF", MoneySteal.ToString()),
                new Loc.KeywordValue("THORNS", thorns + "%")
            };
        }
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.Inventory.OnCharmEffectRefreshedForServer += OnCharmEffectRefreshedForServer;
            //parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
            //parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;

            applied = parent.Networkowner.unitAvatar.GetCustomStat(ECustomStat.Negotiation) * thorns;
            parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.Thorns, applied);
        }

        private void OnCharmEffectRefreshedForServer()
        {
            parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.Thorns, -applied);
            applied = parent.Networkowner.unitAvatar.GetCustomStat(ECustomStat.Negotiation) * thorns;
            parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.Thorns, applied);
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            damage.hpSteal += MoneySteal;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            var steal = (damage.damage / 1000f) * MoneySteal;
            parent.Networkowner.unitAvatar.AddMoney(Mathf.RoundToInt(steal));
            parent.Networkowner.unitAvatar.HealMpFloat(steal);
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.Inventory.OnCharmEffectRefreshedForServer -= OnCharmEffectRefreshedForServer;
            //parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
            //parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;

            parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.Thorns, -applied);
            applied = 0;
        }
    }
}
