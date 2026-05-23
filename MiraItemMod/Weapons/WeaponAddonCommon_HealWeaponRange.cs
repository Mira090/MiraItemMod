using HarmonyLib;
using SephiriaMod.Buffs;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonCommon_HealWeaponRange : WeaponAddon
    {
        public static readonly string StatusID = "HealWeaponRange".ToSephiriaUpperId();
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe(StatusID, 32);
            parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (!damage.IsSameElementalType(EDamageElementalType.Chaos))
                return;
            foreach (var buff in parent.Networkowner.unitAvatar.Buffs)
            {
                if(buff.ID == Data.SoulStealBuff.ID && buff is CharacterBuffMod mod)
                {
                    int consume = mod.CurrentStack;
                    if(consume > 5)
                        consume = 5;
                    mod.SetCurrentStack(mod.CurrentStack - consume);
                    damage.criticalChancePercent += consume * 10;
                    return;
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe(StatusID, -32);
            parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }

        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.Heal))]
        private static class HealPatch
        {
            static void Postfix(UnitAvatar __instance, ref float amount)
            {
                if (__instance.IsDead)
                    return;
                var stat = __instance.GetCustomStatUnsafe(StatusID);
                if (stat <= 0)
                    return;
                float value = Mathf.Max(16f / stat, 1);
                if (value > 0 && amount > 0)
                {
                    __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                    for (int q = 0; q < amount / value; q++)
                    {
                        __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.HealPercent))]
        private static class HealPercentPatch
        {
            static void Postfix(UnitAvatar __instance, ref float percent)
            {
                if (__instance.IsDead)
                    return;
                var stat = __instance.GetCustomStatUnsafe(StatusID);
                if (stat <= 0)
                    return;
                stat = Mathf.Max(16 / stat, 1);
                if (stat > 0 && percent > 0)
                {
                    __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                    for (int q = 0; q < (__instance.MaxHp * (percent / 100f)) / stat; q++)
                    {
                        __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                    }
                }
            }
        }
    }
}
