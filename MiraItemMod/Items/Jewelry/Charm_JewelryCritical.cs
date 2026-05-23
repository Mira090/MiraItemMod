using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryCritical : Charm_Jewelry
    {
        public override int[] Consume => consumeSmall;
        public int[] critical = new int[] { 0, 0, 10, 20, 35, 50 };

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? critical.SafeRandomAccess(0) + "→" + critical.SafeRandomAccess(maxLevel) : critical.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset)),
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
            if (CurrentLevelToIdx() < FirstLevel)
                return;
            if (NetworkAvatar.Money > Consume.SafeRandomAccess(CurrentLevelToIdx()))
            {
                if (avatar.monsterType != EMonsterType.Dummy)
                    NetworkAvatar.AddMoney(-Consume.SafeRandomAccess(CurrentLevelToIdx()));
                damage.criticalChancePercent += critical.SafeRandomAccess(CurrentLevelToIdx());
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
    }
}
