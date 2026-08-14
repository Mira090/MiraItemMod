using HarmonyLib;
using MiraItemMod.Combos;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaDarkCloud : Charm_MachinaBasic
    {
        public static readonly string Stat = "MachinaDarkCloud".ToSephiriaUpperId();

        public int[] statByLevel = new int[10] { 10, 15, 20, 25, 35, 45, 60, 75, 90, 120 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? statByLevel.SafeRandomAccess(0) + "→" + statByLevel.SafeRandomAccess(maxLevel) : statByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, statByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, -statByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe(Stat, -statByLevel.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe(Stat, statByLevel.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        [HarmonyPatch(typeof(ComboEffect_DarkCloud), nameof(ComboEffect_DarkCloud.UseCloudCoroutine))]
        public class DarkCloudPatch
        {
            static void Postfix(ComboEffect_DarkCloud __instance)
            {
                if (__instance.Networkavatar == null || __instance.Networkavatar.Inventory == null)
                    return;
                var stat = __instance.Networkavatar.GetCustomStatUnsafe(Stat);
                if (stat <= 0)
                    return;
                var combo = __instance.Networkavatar.Inventory.FindComboEffect(ItemCategories.Machina);
                if (!(combo is ComboEffect_Machina machina))
                    return;
                var machinaCharm = machina.GetMachinaCharm();
                if (!(machinaCharm is Charm_Machina armament))
                    return;
                armament.Attack(stat);
            }
        }
    }
}
