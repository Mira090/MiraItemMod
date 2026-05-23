using HarmonyLib;
using SephiriaMod.Items.Jewelry;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Eternal
{
    public class Charm_EmberFlameSword : Charm_StatusInstance
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("FlameSwordAdditionBurn".ToUpperInvariant(), 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("FlameSwordAdditionBurn".ToUpperInvariant(), -1);
        }

        [HarmonyPatch(typeof(ComboEffect_FlameSword), "HandleAttackUnit")]
        public static class ComboEffect_FlameSwordHandleAttackUnitPatch
        {
            public static UnitAvatar RecentTarget;
            static bool Prefix(UnitAvatar target, DamageInstance damage, ComboEffect_FlameSword __instance)
            {
                if ((bool)target && !target.IsDead && !__instance.GetIsCooldown())
                {
                    var magitech = __instance.Networkavatar.GetCustomStatUnsafe(Charm_MagitechFlameSword.Status);

                    if (magitech > 0)
                    {
                        if (Core.LogMany)
                            Core.Logger($"DamageId: {damage.id}");
                        if(damage.id == "Debuff_Electric" || damage.id == "Debuff_Plasma")
                        {
                            __instance.SetIsCooldown(true);
                            __instance.SetCooldownTimer(0);
                            __instance.ServerFireSword(target.transform.position, false, false);
                        }
                        return false;
                    }


                    bool flag = damage.fromType == EDamageFromType.DirectAttack;
                    bool flag2 = damage.fromType == EDamageFromType.Magic;
                    if (flag || flag2)
                    {
                        RecentTarget = target;
                    }
                    else if ((bool)__instance.Networkavatar && __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDCALLBACKFROST") > 0 && damage.flag.Contains("FROSTRELIC"))
                    {
                        RecentTarget = target;
                    }
                    else
                    {
                        RecentTarget = null;
                    }
                }
                else
                {
                    RecentTarget = null;
                }

                return true;
            }
        }
        [HarmonyPatch(typeof(ComboEffect_FlameSword), nameof(ComboEffect_FlameSword.ServerFireSword))]
        public static class ComboEffect_FlameSwordLocalFireSwordPatch
        {
            static void Postfix(Vector3 motionTo, bool isDireectAttack, bool isMagic, ComboEffect_FlameSword __instance)
            {
                if(ComboEffect_FlameSwordHandleAttackUnitPatch.RecentTarget == null)
                    return;
                ByBurn(__instance, motionTo, isDireectAttack, isMagic);
            }
            private static void ByBurn(ComboEffect_FlameSword __instance, Vector3 motionTo, bool isDireectAttack, bool isMagic)
            {
                var addition = __instance.Networkavatar.GetCustomStatUnsafe("FlameSwordAdditionBurn".ToUpperInvariant());
                if (addition <= 0)
                    return;

                int burn = 0;
                foreach (var debuff in ComboEffect_FlameSwordHandleAttackUnitPatch.RecentTarget.Debuffs)
                {
                    if (debuff.ID == "BURN")
                    {
                        burn += debuff.CurrentStack;
                    }
                }

                for (int q = 0; q < burn * addition; q++)
                {
                    Vector3 motionFrom = motionTo + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 2.5f;
                    __instance.FireSword(motionFrom, motionTo);
                    /*
                    if (__instance.currentSword <= 0)
                    {
                        return;
                    }

                    __instance.NetworkcurrentSword = __instance.currentSword - 1;
                    __instance.StartCoroutine(__instance.InvokeCreateFlameSword(motionTo));*/
                }
            }
            private static void ByLeaf(ComboEffect_FlameSword __instance, Vector3 motionTo, bool isDireectAttack, bool isMagic)
            {
                var addition = __instance.Networkavatar.GetCustomStatUnsafe(Charm_JewelryFlameSword.Status);
                if (addition <= 0)
                    return;

                var money = __instance.Networkavatar.Money;
                if (money < addition)
                    return;
                __instance.Networkavatar.AddMoney(-money);

                Vector3 motionFrom = motionTo + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 2.5f;
                __instance.FireSword(motionFrom, motionTo);
            }
        }
    }
}
