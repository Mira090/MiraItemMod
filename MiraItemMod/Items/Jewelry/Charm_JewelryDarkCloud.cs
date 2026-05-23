using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryDarkCloud : Charm_Jewelry
    {
        public static readonly string Status = "DarkCloudAdditionLeaf".ToUpperInvariant();

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
        [HarmonyPatch(typeof(ComboEffect_DarkCloud), nameof(ComboEffect_DarkCloud.UseCloudCoroutine))]
        public static class UseCloudCoroutinePatch
        {
            private static RaycastHit2D[] cachedHits = new RaycastHit2D[22];
            static void Prefix(ComboEffect_DarkCloud __instance, bool needCloudValue, ref int multishot)
            {
                if (CombatManager.Instance.PeaceMode)
                {
                    return;
                }

                List<UnitAvatar> targets = new List<UnitAvatar>();
                int num = Physics2D.CircleCastNonAlloc(__instance.Networkavatar.transform.position, __instance.cloudRadius, Vector2.zero, cachedHits, 0f, CombatManager.Topdown1FLayerMask);
                for (int j = 0; j < num; j++)
                {
                    Hitbox component = cachedHits[j].transform.GetComponent<Hitbox>();
                    if (!component)
                    {
                        continue;
                    }

                    CombatBehaviour combatBehaviour = component.GetCombatBehaviour(0);
                    if ((bool)combatBehaviour)
                    {
                        UnitAvatar unitAvatar = combatBehaviour as UnitAvatar;
                        if ((bool)unitAvatar && !unitAvatar.IsDead && !unitAvatar.IsInvulnerable && unitAvatar.canBeTarget.IsTrue() && CombatManager.ContainsAttackableFaction(unitAvatar.GetHostileFactionLayers(EDamageFromType.None), __instance.Networkavatar.faction))
                        {
                            targets.Add(unitAvatar);
                        }
                    }
                }

                if(targets.Count > multishot && (__instance.darkCloud > multishot || !needCloudValue))
                {
                    var stat = __instance.Networkavatar.GetCustomStatUnsafe(Status);
                    if(stat > 0 && __instance.Networkavatar.Money >= stat)
                    {
                        __instance.Networkavatar.AddMoney(-stat);
                        multishot++;
                    }
                }
            }
        }
    }
}
