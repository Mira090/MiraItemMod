using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryFrostRelic : Charm_Jewelry
    {
        public static readonly string Status = "FrostRelicAdditionLeaf".ToUpperInvariant();

        public override int[] Consume => consumeMedium;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, Consume.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, -Consume.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe(Status, -Consume.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe(Status, Consume.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset))
            };
        }

        [HarmonyPatch(typeof(ChargingCharm), nameof(ChargingCharm.ActivateChargingCharm))]
        public static class ChargingCharmPatch
        {
            static void Prefix(ChargingCharm __instance, ref bool __state)
            {
                __state = false;

                var avatar = __instance.GetAvatar();
                if (avatar == null)
                    return;
                var stat = avatar.GetCustomStatUnsafe(Status);
                var money = avatar.Money;
                if(stat > 0 && money >= stat)
                {
                    avatar.AddMoney(-stat);
                    avatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", 1);
                    __state = true;
                }
            }
            static void Postfix(ChargingCharm __instance, ref bool __state)
            {
                var avatar = __instance.GetAvatar();
                if (avatar == null)
                    return;
                if (__state)
                {
                    avatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", -1);
                }
            }
        }
        [HarmonyPatch(typeof(ChargingCharm), nameof(ChargingCharm.ActivateChargingCharmNoCount))]
        public static class ChargingCharmNoCountPatch
        {
            static void Prefix(ChargingCharm __instance, ref bool __state)
            {
                __state = false;

                var avatar = __instance.GetAvatar();
                if (avatar == null)
                    return;
                var stat = avatar.GetCustomStatUnsafe(Status);
                var money = avatar.Money;
                if (stat > 0 && money >= stat)
                {
                    avatar.AddMoney(-stat);
                    avatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", 1);
                    __state = true;
                }
            }
            static void Postfix(ChargingCharm __instance, ref bool __state)
            {
                var avatar = __instance.GetAvatar();
                if (avatar == null)
                    return;
                if (__state)
                {
                    avatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", -1);
                }
            }
        }
    }
}
