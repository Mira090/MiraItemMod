using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DrunkGlacier : Charm_StatusInstance
    {
        public float[] time = new float[] { 5f, 4f, 3.5f, 3f, 2.5f, 2f };
        public Timer cooldownTimer = new Timer(5f);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? time.SafeRandomAccess(0) + "→" + time.SafeRandomAccess(maxLevel) : time.SafeRandomAccess(LevelToIdx(level)).ToString();
            //string value2 = (showAllLevel ? (series.SafeRandomAccess(0) + "→" + series.SafeRandomAccess(maxLevel)) : series.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value3 = (showAllLevel ? (seriesAddition.SafeRandomAccess(0) + "→" + seriesAddition.SafeRandomAccess(maxLevel)) : seriesAddition.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime * (1 + Mathf.Max(0, -NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction)) / 100f)))
            {
                isInCooldown = false;
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            cooldownTimer.time = time.SafeRandomAccess(CurrentLevelToIdx());
            NetworkAvatar.AddCustomStat("NOSTUNFREEZE", 1);
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            cooldownTimer.time = time.SafeRandomAccess(LevelToIdx(newLevel));
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (!isInCooldown && damage.fromType == EDamageFromType.DirectAttack && !avatar.IsDead)
            {
                isInCooldown = true;
                avatar.ApplyDebuff(SephiriaPrefabs.Frostbite, NetworkAvatar);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStat("NOSTUNFREEZE", -1);
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        [HarmonyPatch(typeof(CharacterDebuff_Freeze), "ApplyStatusInner")]
        public class ApplyFreezePatch
        {
            static bool Prefix(CharacterDebuff_Freeze __instance)
            {
                var nostun = __instance.NetworkAttacker.GetCustomStatUnsafe("NOSTUNFREEZE");
                if(nostun > 0)
                {
                    ApplyStatusInner(__instance);
                    return false;
                }
                return true;
            }
            static void ApplyStatusInner(CharacterDebuff_Freeze __instance)
            {
                Debug.Log("Freeze : Apply Status");
                if (!__instance.NetworkAttacker)
                {
                    return;
                }

                float num = __instance.NetworkAttacker.GetCustomStat(ECustomStat.IceDamage) * __instance.damageMultiplier * __instance.CurrentStack;
                int customStatUnsafe = __instance.NetworkAttacker.GetCustomStatUnsafe("FREEZEDAMAGE");
                num += num * customStatUnsafe / 100f;
                DamageInstance damage = DamageInstance.GetDamage(__instance.NetworkAttacker, "Debuff_Freeze", __instance.NetworkTarget.transform.position, 4294967295L, num, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                damage.elementalType = EDamageElementalType.Ice;
                __instance.NetworkTarget.ApplyDamage(damage);
                //if (__instance.NetworkTarget.StartStun(__instance.BuffDuration))
                {
                    //__instance.NetworkstunApplied = true;
                }

                int customStatUnsafe2 = __instance.NetworkAttacker.GetCustomStatUnsafe("FREEZECLOUD");
                if ((bool)__instance.NetworkAttacker.Inventory)
                {
                    ComboEffectBase comboEffectBase = __instance.NetworkAttacker.Inventory.FindComboEffect("DARKCLOUD");
                    if (comboEffectBase != null && comboEffectBase.isEnabled && comboEffectBase is ComboEffect_DarkCloud comboEffect_DarkCloud)
                    {
                        comboEffect_DarkCloud.FreezeCloudHeal(customStatUnsafe2);
                    }
                }
                __instance.RequestEnd();
            }
        }
    }
}
