using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_GuardianDarkCloud : Charm_StatusInstance
    {
        public int[] percent = new int[] { 130, 180, 240, 360, 480 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("PERCENT", "+" + value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant(), percent.SafeRandomAccess(CurrentLevelToIdx()));
            if (NetworkAvatar.GetIsHitInvincibleEnabled())
            {
                var speed = NetworkAvatar.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());
                NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSPEED", speed);
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (NetworkAvatar.GetIsHitInvincibleEnabled())
            {
                var speed = NetworkAvatar.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());
                NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSPEED", -speed);
            }
            NetworkAvatar.AddCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant(), -percent.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (NetworkAvatar.GetIsHitInvincibleEnabled())
            {
                var speed = NetworkAvatar.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());
                NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSPEED", -speed);
            }
            NetworkAvatar.AddCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant(), -percent.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant(), percent.SafeRandomAccess(LevelToIdx(newLevel)));
            if (NetworkAvatar.GetIsHitInvincibleEnabled())
            {
                var speed = NetworkAvatar.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());
                NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSPEED", speed);
            }
        }

        [HarmonyPatch]
        public static class UnitAvatarPatch
        {
            [HarmonyPatch(typeof(UnitAvatar), "Update")]
            [HarmonyPrefix]
            public static void UpdatePrefix(UnitAvatar __instance)
            {
                if (!__instance.isServer)
                    return;
                var speed = __instance.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());

                if (speed == 0)
                    return;

                if (__instance.GetIsHitInvincibleEnabled())
                {
                    var timer = __instance.GetInvincibleTimer() + Time.deltaTime;
                    float num6 = __instance.invincibleTime;
                    if (__instance.GetInvincibleType() == EInvincibleType.Fall)
                    {
                        num6 += __instance.GetRestoreFallingTimer().time;
                    }

                    if (timer >= num6)
                    {
                        //解除
                        __instance.AddCustomStatUnsafe("DARKCLOUDSPEED", -speed);
                    }
                }
            }
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.ApplyDamage), new Type[] { typeof(DamageInstance) })]
            [HarmonyPostfix]
            public static void ApplyDamagePostfix(UnitAvatar __instance, EApplyDamageResult __result)
            {
                if (__result != EApplyDamageResult.Success)
                    return;
                if (__instance.GetIsHitInvincibleEnabled() && __instance.GetInvincibleTimer() == 0)
                {
                    var speed = __instance.GetCustomStatUnsafe("DarkCloudInvincibleSpeed".ToUpperInvariant());
                    __instance.AddCustomStatUnsafe("DARKCLOUDSPEED", speed);
                }
            }
        }
    }
}
