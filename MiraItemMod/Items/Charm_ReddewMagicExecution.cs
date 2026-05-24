using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace MiraItemMod.Items
{
    public class Charm_ReddewMagicExecution : Charm_StatusInstance
    {
        public static string DamageId = "ReddewMagicExecution";
        public static int Count = 5;

        public int[] percent = new int[] { 10, 20, 30 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", Count.ToString()),
            new Loc.KeywordValue("ITEM", new LocalizedString("Item_RedDew_Name").ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("AdditionalReddew".ToUpperInvariant(), percent.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("AdditionalReddew".ToUpperInvariant(), -percent.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe("AdditionalReddew".ToUpperInvariant(), -percent.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("AdditionalReddew".ToUpperInvariant(), percent.SafeRandomAccess(LevelToIdx(newLevel)));
        }

        [HarmonyPatch(typeof(Charm_Reddew), nameof(Charm_Reddew.SummonDueFromVictim), new Type[] { typeof(UnitAvatar), typeof(DamageInstance) })]
        public static class ReddewPatch
        {
            public static bool Prefix(UnitAvatar unit, DamageInstance damage, Charm_Reddew __instance)
            {
                if (damage.id == DamageId)
                    return false;
                if (__instance.NetworkAvatar.IsDead || !damage.IsCriticalOrExecutionAttack || !__instance.GetChance() || damage.id == __instance.damageId)
                    return true;
                if (!damage.IsMagicExecution())
                    return true;
                var percent = __instance.NetworkAvatar.GetCustomStatUnsafe("AdditionalReddew".ToUpperInvariant());
                //Core.Logger($"Reddew: " + percent);
                if (percent <= 0)
                    return true;

                Damage(unit, damage, __instance, percent);
                return true;
            }
            private static void Damage(UnitAvatar unit, DamageInstance damage, Charm_Reddew __instance, int percent)
            {
                Vector2 vector = unit.transform.position;
                ECustomStat highestDamageElementalType = __instance.GetHighestDamageElementalType(unit);
                float damage2 = (float)__instance.NetworkAvatar.GetCustomStat(highestDamageElementalType) * percent / 100f;
                RaycastHit2D[] allocatedHits = new RaycastHit2D[12];
                int num = Physics2D.CircleCastNonAlloc(vector, __instance.attackRadius, Vector2.zero, allocatedHits, 0f, CombatManager.Topdown1FLayerMask);
                for (int i = 0; i < num; i++)
                {
                    if (!allocatedHits[i])
                        continue;
                    Vector2 vector2 = vector - (Vector2)allocatedHits[i].transform.position;
                    vector2.y *= 2f;
                    //Core.Logger($"{vector2.x}, {vector2.y}");
                    if (vector2.magnitude > __instance.attackRadius)
                    {
                        continue;
                    }

                    Hitbox component = allocatedHits[i].transform.GetComponent<Hitbox>();
                    if ((bool)component)
                    {
                        CombatBehaviour combatBehaviour = component.GetCombatBehaviour(0);
                        if ((bool)combatBehaviour)
                        {
                            for(int q = 0; q < Count; q++)
                            {
                                DamageInstance damage3 = DamageInstance.GetDamage(__instance.NetworkAvatar, DamageId, allocatedHits[i].point, __instance.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), damage2, EDamageType.Projectile, EDamageFromType.None, (Vector2)combatBehaviour.transform.position - vector, 1, 1f);
                                //damage3.criticalChancePercent = -999999f;
                                DamageInstance damageInstance = damage3;
                                /*
                                damageInstance.elementalType = highestDamageElementalType switch
                                {
                                    ECustomStat.PhysicalDamage => EDamageElementalType.Physical,
                                    ECustomStat.FireDamage => EDamageElementalType.Fire,
                                    ECustomStat.IceDamage => EDamageElementalType.Ice,
                                    ECustomStat.LightningDamage => EDamageElementalType.Lightning,
                                    _ => EDamageElementalType.Physical,
                                };*/
                                damage3.SetCustomColor(true, ModUtil.FourGradation);
                                combatBehaviour.ApplyDamage(damage3);
                            }
                        }
                    }
                }
            }
        }
    }
}
