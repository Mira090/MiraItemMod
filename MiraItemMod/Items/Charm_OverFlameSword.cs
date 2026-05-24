using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_OverFlameSword : Charm_StatusInstance
    {
        public static readonly string OverFlameSword = "OverFlameSword".ToUpperInvariant();
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(OverFlameSword, 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(OverFlameSword, -1);
        }

        [HarmonyPatch(typeof(ComboEffect_FlameSword), "AddSwordServer")]
        public static class FlameSwordPatch
        {
            static void Prefix(ComboEffect_FlameSword __instance, int amount)
            {
                try
                {
                    if (__instance.Networkavatar.GetCustomStatUnsafe(OverFlameSword) <= 0 || !__instance.Networkavatar.IsInBattle)
                        return;

                    int b = __instance.maxSword + __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDMAX");
                    var over = (__instance.currentSword + amount + __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDPICKBONUS")) - b;
                    for (int q = 0; q < over; q++)
                    {
                        __instance.ServerFireSword(__instance.Networkavatar.transform.position, false, false);
                    }
                }
                catch(Exception e)
                {
                    Debug.LogWarning(e);
                    Core.LoggerWarning(e);
                }
            }
        }
        [HarmonyPatch(typeof(FlameSwordPickLocal), "Pick")]
        public static class PickLocalPatch
        {
            static void Prefix(FlameSwordPickLocal __instance, ref bool addsword)
            {
                try
                {
                    var combo = __instance.GetComboEffect();
                    if (combo == null || combo.Networkavatar.GetCustomStatUnsafe(OverFlameSword) <= 0 || !combo.Networkavatar.IsInBattle)
                        return;

                    if (__instance.autoDestroyTimer.GetTimer() == 0f)
                    {
                        addsword = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    Core.LoggerWarning(e);
                }
            }
        }
        //[HarmonyPatch(typeof(ComboEffect_FlameSword), "UserCode_RpcClearAllPick")]
        [Obsolete]
        public static class OnStartBattlePatch
        {
            static bool Prefix(ComboEffect_FlameSword __instance)
            {
                if (__instance.Networkavatar.GetCustomStatUnsafe(OverFlameSword) <= 0)
                    return true;


                try
                {
                    var pickList = __instance.GetPickList();
                    foreach (FlameSwordPickLocal flameSwordPickLocal in pickList)
                    {
                        if (flameSwordPickLocal)
                        {
                            DestroyPick(flameSwordPickLocal);
                        }
                    }
                    pickList.Clear();
                }
                catch (Exception message)
                {
                    Debug.LogWarning(message);
                }
                return false;
            }

            static void DestroyPick(FlameSwordPickLocal pick)
            {
                UnityEngine.Object.Destroy(pick.gameObject);

                if ((bool)pick.pickFxPrefab)
                {
                    SpriteFx.Pool.Spawn(pick.pickFxPrefab, pick.transform.position);
                }
            }
        }
    }
}
