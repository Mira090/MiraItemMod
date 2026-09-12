using HarmonyLib;
using MiraItemMod.Buffs;
using MiraItemMod.Items.Vitality;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
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

        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.Heal), new Type[] { typeof(float), typeof(bool), typeof(bool) })]
        private static class HealPatch
        {
            static void Postfix(UnitAvatar __instance, float amount)
            {
                if (__instance.IsDead)
                    return;
                var soulsteal = __instance.GetCustomStatUnsafe(StatusID);
                if (soulsteal > 0)
                {
                    float value = Mathf.Max(16f / soulsteal, 1);
                    if (value > 0 && amount > 0)
                    {
                        __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                        for (int q = 0; q < amount / value; q++)
                        {
                            __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                        }
                    }
                }
                var attackspeed = __instance.GetCustomStatUnsafe(Charm_WindSongVitality.Stat);
                if(attackspeed > 0)
                {
                    for(int q = 0; q < (amount / Charm_WindSongVitality.Per); q++)
                    {
                        for(int q2 = 0; q2 < attackspeed; q2++)
                        {
                            __instance.ApplyBuff(Data.CraveBuff, 1, __instance, true);
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.HealPercent), new Type[] { typeof(float), typeof(bool), typeof(bool) })]
        private static class HealPercentPatch
        {
            static void Postfix(UnitAvatar __instance, float percent)
            {
                if (__instance.IsDead)
                    return;
                var soulsteal = __instance.GetCustomStatUnsafe(StatusID);
                if (soulsteal > 0)
                {
                    var value = Mathf.Max(16 / soulsteal, 1);
                    if (value > 0 && percent > 0)
                    {
                        __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                        for (int q = 0; q < (__instance.MaxHp * (percent / 100f)) / value; q++)
                        {
                            __instance.ApplyBuff(Data.SoulStealBuff, 1, __instance, true);
                        }
                    }
                }
                var amount = __instance.MaxHp * (percent / 100f);
                var attackspeed = __instance.GetCustomStatUnsafe(Charm_WindSongVitality.Stat);
                if (attackspeed > 0)
                {
                    for (int q = 0; q < (amount / Charm_WindSongVitality.Per); q++)
                    {
                        for (int q2 = 0; q2 < attackspeed; q2++)
                        {
                            __instance.ApplyBuff(Data.CraveBuff, 1, __instance, true);
                        }
                    }
                }
            }
        }
    }
}
