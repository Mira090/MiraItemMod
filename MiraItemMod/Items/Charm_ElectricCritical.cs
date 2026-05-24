using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ElectricCritical : Charm_StatusInstance
    {
        private int[] percent = new int[] { 36, 42, 48, 56, 64, 75 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            var damage = avatar.GetCustomStat(ECustomStat.LightningDamage);
            string value = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? (damage * 0.01f * percent.SafeRandomAccess(0)).ToString("0.##") + "→" + (damage * 0.01f * percent.SafeRandomAccess(maxLevel)).ToString("0.##") : (damage * 0.01f * percent.SafeRandomAccess(LevelToIdx(level))).ToString("0.##");
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CRITICAL", "+" + value2 + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if(damage.fromType == EDamageFromType.DirectAttack || damage.IsSameElementalType(EDamageElementalType.Lightning))
            {
                damage.criticalChancePercent += NetworkAvatar.GetCustomStat(ECustomStat.LightningDamage) * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
    }
}
