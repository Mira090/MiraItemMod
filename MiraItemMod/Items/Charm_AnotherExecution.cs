using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiraItemMod.Items
{
    public class Charm_AnotherExecution : Charm_StatusInstance
    {
        public int[] criticalByLevel = new int[] { 20, 16, 12, 8 };
        public int[] ignoreDegence = new int[] { 8, 12, 16, 20 };
        public string damageId = "Charm_AnotherExecution";
        public static float MagicExecutionMax = 8f;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? ignoreDegence.SafeRandomAccess(0) + "→" + ignoreDegence.SafeRandomAccess(maxLevel) : ignoreDegence.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //NetworkAvatar.OnAttackUnit += OnAttackUnit;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            var execute = Mathf.Clamp(damage.criticalChancePercent - 100f, 0f, 100f);
            //Core.Logger("審判: " + execute + "%");
            if(Random.Range(0f, 100f) < execute)
            {
                damage.criticalChancePercent = 0;
                damage.isCriticalAttack = true;
                damage.elementalType = EDamageElementalType.Chaos;
                if (damage.IsFourGradation())
                    damage.color = ModUtil.FourGradationMagicExecution;
                else
                    damage.color = new Color(1, 1, 1, 0);
                damage.useCustomColor = true;
                var phy = NetworkAvatar.GetCustomStat(ECustomStat.PhysicalDamage);
                var fire = NetworkAvatar.GetCustomStat(ECustomStat.FireDamage);
                var ice = NetworkAvatar.GetCustomStat(ECustomStat.IceDamage);
                var lightning = NetworkAvatar.GetCustomStat(ECustomStat.LightningDamage);
                float max = Mathf.Max(phy, fire, ice, lightning);
                float min = Mathf.Min(phy, fire, ice, lightning);
                if (min < 1)
                    min = 1;
                damage.damage *= Mathf.Clamp(max / min, 1, MagicExecutionMax);
                //damage.ignoreDefense += ignoreDegence.SafeRandomAccess(CurrentLevelToIdx());
                //Core.Logger("審判!: " + (max / min));
            }
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if(damage.isCriticalAttack && !damage.isExecutionAttack)
            {
                if (!NetworkAvatar.IsDead && IsEffectEnabled)
                {
                    float customStatUnsafe = damage.damageResult * 0.5f;
                    if (customStatUnsafe > 0 && avatar != null)
                    {
                        DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damageId, avatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                        d.elementalType = EDamageElementalType.Physical;
                        d.criticalChancePercent = d.criticalChancePercent - criticalByLevel.SafeRandomAccess(CurrentLevelToIdx());
                        avatar.ApplyDamage(d);
                    }
                }
            }
            else if (damage.isExecutionAttack)
            {
                if (!NetworkAvatar.IsDead && IsEffectEnabled)
                {
                    float customStatUnsafe = damage.damageResult * 0.5f;
                    if (customStatUnsafe > 0 && avatar != null)
                    {
                        DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damageId, avatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                        d.SetCustomColor(true, Color.black);
                        d.criticalChancePercent = 0;
                        avatar.ApplyDamage(d);
                    }
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //NetworkAvatar.OnAttackUnit -= OnAttackUnit;
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
    }
}
