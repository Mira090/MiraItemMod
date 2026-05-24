using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_DrunkDarkCloud : Charm_StatusInstance
    {
        public int[] speed = new int[] { -80 };
        public int[] series = new int[] { 1 };
        public int[] seriesAddition = new int[] { 1 };
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? speed.SafeRandomAccess(0) + "→" + speed.SafeRandomAccess(maxLevel) : speed.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? series.SafeRandomAccess(0) + "→" + series.SafeRandomAccess(maxLevel) : series.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value3 = showAllLevel ? seriesAddition.SafeRandomAccess(0) + "→" + seriesAddition.SafeRandomAccess(maxLevel) : seriesAddition.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("SPEED", value + "%", GetNegativeColor(virtualLevelOffset)),
            new Loc.KeywordValue("SERIES", "+" + value2, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("ADDITION", "+" + value3, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatAmp("DARKCLOUDSPEED", speed.SafeRandomAccess(CurrentLevelToIdx()));
            //NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSERIES", series.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSERIESUNDEFENSE", seriesAddition.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatAmp("DARKCLOUDSPEED", -speed.SafeRandomAccess(CurrentLevelToIdx()));
            //NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSERIES", -series.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("DARKCLOUDSERIESUNDEFENSE", -seriesAddition.SafeRandomAccess(CurrentLevelToIdx()));
        }

        [HarmonyPatch(typeof(ComboEffect_DarkCloud), nameof(ComboEffect_DarkCloud.UseCloud), new Type[] { typeof(bool) })]
        public class UseCloudPatch
        {
            static void Postfix(bool needCloudValue, ComboEffect_DarkCloud __instance)
            {
                var series = __instance.Networkavatar.GetCustomStatUnsafe("DARKCLOUDSERIES");
                var addition = __instance.Networkavatar.GetCustomStatUnsafe("DARKCLOUDSERIESUNDEFENSE");
                if(addition > 0)
                {
                    series += Mathf.Max(0, -__instance.Networkavatar.GetCustomStat(ECustomStat.DamageReduction)) / 10 * addition;
                }
                if (series <= 0)
                    return;
                int num = __instance.Networkavatar.GetCustomStatUnsafe("DARKCLOUDMULTISHOT");
                if (num <= 0)
                {
                    num = 1;
                }
                __instance.StartCoroutine(Coroutine(series, needCloudValue, num, __instance));
            }
            static IEnumerator Coroutine(int count, bool needCloudValue, int multishot, ComboEffect_DarkCloud __instance)
            {
                for(int q = 0; q < count; q++)
                {
                    if (__instance.Networkavatar.IsDead)
                        yield break;

                    yield return new WaitForSeconds(0.1f);
                    __instance.StartCoroutine(__instance.UseCloudCoroutine(needCloudValue, multishot));
                }
            }
        }
        [HarmonyPatch(typeof(ComboEffect_DarkCloud), nameof(ComboEffect_DarkCloud.UseCloud), new Type[] { typeof(bool), typeof(int) })]
        public class UseCloudWeaponPatch
        {
            static void Postfix(bool needCloudValue, int multishot, ComboEffect_DarkCloud __instance)
            {
                var series = __instance.Networkavatar.GetCustomStatUnsafe("DARKCLOUDSERIES");
                var addition = __instance.Networkavatar.GetCustomStatUnsafe("DARKCLOUDSERIESUNDEFENSE");
                if (addition > 0)
                {
                    series += Mathf.Max(0, -__instance.Networkavatar.GetCustomStat(ECustomStat.DamageReduction)) / 20 * addition;
                }
                if (series <= 0)
                    return;
                __instance.StartCoroutine(Coroutine(series, needCloudValue, multishot, __instance));
            }
            static IEnumerator Coroutine(int count, bool needCloudValue, int multishot, ComboEffect_DarkCloud __instance)
            {
                for (int q = 0; q < count; q++)
                {
                    if (__instance.Networkavatar.IsDead)
                        yield break;

                    yield return new WaitForSeconds(0.1f);
                    __instance.StartCoroutine(__instance.UseCloudCoroutine(needCloudValue, multishot));
                }
            }
        }
    }
}
