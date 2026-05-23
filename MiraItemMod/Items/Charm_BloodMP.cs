using HarmonyLib;
using Newtonsoft.Json;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_BloodMP : Charm_StatusInstance
    {
        public int requireLevel = 0;
        public int[] percent = new int[] { 10 };
        public int[] steal = new int[] { 8, 8, 8, 10, 12, 15 };
        public bool toHPRegen = false;
        private int mp = 0;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? steal.SafeRandomAccess(0) + "→" + steal.SafeRandomAccess(maxLevel) : steal.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
                new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("STEAL", "+" + value2, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if(CurrentLevelToIdx() >= requireLevel)
            {
                NetworkAvatar.AddCustomStatUnsafe("BLOODMP", 1);
                NetworkAvatar.AddCustomStatUnsafe("INFINITYMP", 1);

                if (toHPRegen && NetworkAvatar.IsInBattle)
                {
                    mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                    NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                    NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, mp);
                }
            }
            NetworkAvatar.OnStartBattle += OnStartBattle;
            NetworkAvatar.OnStopBattle += OnStopBattle;
            NetworkAvatar.OnMpUsedServerside += OnMpUsedServerside;
            //NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnit;
        }

        private void OnStopBattle()
        {
            if (CurrentLevelToIdx() < requireLevel)
                return;
            if (!toHPRegen)
                return;
            NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
            NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, -mp);
            mp = 0;
        }

        private void OnStartBattle()
        {
            if (CurrentLevelToIdx() < requireLevel)
                return;
            if (!toHPRegen)
                return;
            mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
            NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
            NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, mp);
        }

        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (LevelToIdx(level) < requireLevel && idx == 0)
                return null;
            if (LevelToIdx(level) < requireLevel && idx == 1)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (CurrentLevelToIdx() < requireLevel)
                return;
            if (damage.fromType == EDamageFromType.Magic)
            {
                damage.hpSteal += 10;
            }
        }

        private void OnMpUsedServerside(int used)
        {
            int consume = used;
            /*
            if(NetworkAvatar.MP > 0)
            {
                consume = Mathf.Max(0, -(NetworkAvatar.mp - used));
                NetworkAvatar.Networkmp = Mathf.Max(0, NetworkAvatar.mp - used);
            }*/
            NetworkAvatar.SetHp(NetworkAvatar.hp - consume * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f));

            try
            {
                NetworkAvatar.InvokeOnDamagedServerside(null);
            }
            catch (Exception e)
            {
                Core.LoggerWarning(e);
            }

            if (NetworkAvatar.hp <= 0f)
            {
                if (NetworkAvatar.additionalLifeUsed < NetworkAvatar.additionalLife)
                {
                    NetworkAvatar.StartHitFeedback(5);
                    NetworkAvatar.NetworkremainingSuperArmor = 0f;
                    NetworkAvatar.StartReviveInvulnerable();
                    NetworkAvatar.NetworkadditionalLifeUsed = (sbyte)(NetworkAvatar.additionalLifeUsed + 1);
                    NetworkAvatar.RpcCreateAdditionalLifeFx();
                    NetworkAvatar.SetHp(NetworkAvatar.MaxHp * 0.6f);
                    NetworkAvatar.WriteLog("Player Revived!", Color.white);
                }
                else
                {
                    NetworkAvatar.Die(5, null);
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (CurrentLevelToIdx() >= requireLevel)
            {
                NetworkAvatar.AddCustomStatUnsafe("BLOODMP", -1);
                NetworkAvatar.AddCustomStatUnsafe("INFINITYMP", -1);
            }
            if (mp != 0)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, -mp);
                mp = 0;
            }
            NetworkAvatar.OnMpUsedServerside -= OnMpUsedServerside;
            //NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnit;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (LevelToIdx(newLevel) >= requireLevel && LevelToIdx(oldLevel) < requireLevel)
            {
                if (NetworkAvatar.IsInBattle && toHPRegen)
                {
                    mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                    NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                    NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, mp);
                }
                NetworkAvatar.AddCustomStatUnsafe("BLOODMP", 1);
                NetworkAvatar.AddCustomStatUnsafe("INFINITYMP", 1);
            }
            else if (LevelToIdx(newLevel) < requireLevel && LevelToIdx(oldLevel) >= requireLevel)
            {
                if (NetworkAvatar.IsInBattle && toHPRegen)
                {
                    NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                    NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, -mp);
                    mp = 0;
                }
                NetworkAvatar.AddCustomStatUnsafe("BLOODMP", -1);
                NetworkAvatar.AddCustomStatUnsafe("INFINITYMP", -1);
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (!IsEffectEnabled)
                return;
            if (CurrentLevelToIdx() >= requireLevel && NetworkAvatar.IsInBattle && toHPRegen)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, -mp);
                mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                NetworkAvatar.AddCustomStat(ECustomStat.HPRegen, mp);
            }
        }

        [HarmonyPatch(typeof(UI_MpBar), "LateUpdate")]
        public static class MpBarPatch
        {
            public static Color? DefaultColor = null;
            public static Color BloodColor = new Color(0.6f, 0, 0.1f);
            static void Postfix(UI_MpBar __instance)
            {
                if (__instance.GetTarget() == null || __instance.valueImage == null)
                    return;
                if(DefaultColor == null)
                {
                    DefaultColor = __instance.valueImage.color;
                }

                if(__instance.GetTarget().GetCustomStatUnsafe("BLOODMP") > 0)
                {
                    __instance.valueImage.color = BloodColor;
                }
                else
                {
                    __instance.valueImage.color = DefaultColor.Value;
                }
            }
        }
        [HarmonyPatch(typeof(UI_MultiplayerHPBar), "Update")]
        public static class MpBarMultiplayerPatch
        {
            public static Color? DefaultColor = null;
            public static Color BloodColor = new Color(0.6f, 0, 0);
            static void Postfix(UI_MultiplayerHPBar __instance)
            {
                if (DefaultColor == null)
                {
                    DefaultColor = __instance.mpBar.color;
                }

                if (__instance.GetPlayer().GetCustomStatUnsafe("BLOODMP") > 0)
                {
                    __instance.mpBar.color = BloodColor;
                }
                else
                {
                    __instance.mpBar.color = DefaultColor.Value;
                }
            }
        }
        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.MaxMp), MethodType.Getter)]
        public static class UnitAvatarMaxMPPatch
        {
            static void Postfix(UnitAvatar __instance, ref int __result)
            {
                if (__instance.GetCustomStatUnsafe("INFINITYMP") > 0 && __instance.GetCustomStatUnsafe("BLOODMP") > 0)
                {
                    __result = __instance.maxMp + (int)((float)(__instance.GetCustomStat(ECustomStat.FinalMP) * __instance.maxMp) / 100f);
                }
            }
        }
        [HarmonyPatch(typeof(AvatarStatsHooker), nameof(AvatarStatsHooker.HookStat))]
        public static class AvatarStatsHookerHookStatPatch
        {
            static void Postfix(AvatarStatsHooker __instance, string id, ref string __result)
            {
                if (id == "MAX_MP" && __instance.GetCustomStatUnsafe("INFINITYMP") > 0 && __instance.GetCustomStatUnsafe("BLOODMP") > 0)
                {
                    __result = __instance.GetUnitAvatar().maxMp.ToString();
                }
                if (id == "FINAL_MP" && __instance.GetCustomStatUnsafe("INFINITYMP") > 0 && __instance.GetCustomStatUnsafe("BLOODMP") > 0)
                {
                    __result = $"{(int)((float)(__instance.GetUnitAvatar().GetCustomStat(ECustomStat.FinalMP) * __instance.GetUnitAvatar().maxMp) / 100f)} ({__instance.GetUnitAvatar().GetCustomStat(ECustomStat.FinalMP)}%)";
                }
            }
        }
    }
}
