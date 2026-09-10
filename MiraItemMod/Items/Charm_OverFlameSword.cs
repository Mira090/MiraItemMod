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
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("COOLDOWN", FlameSwordPatch.Cooldown.ToString()),
            };
        }
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
            public static Dictionary<ComboEffect_FlameSword, bool> IsInCooldown = new Dictionary<ComboEffect_FlameSword, bool>();
            public static float Cooldown = 1.5f;
            static void Prefix(ComboEffect_FlameSword __instance, int amount)
            {
                if (IsInCooldown.TryGetValue(__instance, out var value) && value)
                    return;
                try
                {
                    if (__instance.Networkavatar.GetCustomStatUnsafe(OverFlameSword) <= 0 || !__instance.Networkavatar.IsInBattle)
                        return;

                    int b = __instance.maxSword + __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDMAX");
                    var bonus = __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDPICKBONUS");
                    var over = (__instance.currentSword + amount + bonus) - b;//

                    for (int q = 0; q < over; q++)
                    {
                        __instance.ServerFireSword(__instance.Networkavatar.transform.position, false, false);
                    }
                    if (DungeonManager.Instance == null || over <= 0)
                        return;
                    IsInCooldown[__instance] = true;
                    DungeonManager.Instance.Delay(Cooldown, () =>
                    {
                        if (__instance == null)
                            return;
                        IsInCooldown.Remove(__instance);
                    });
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
