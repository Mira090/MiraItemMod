using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryDamage : Charm_Jewelry
    {
        public string DamageId = "Charm_JewelryWind";
        public int[] damage = new int[] { 0, 0, 5, 10, 15, 20 };
        public override int[] Consume => consumeSmall;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? damage.SafeRandomAccess(0) + "→" + damage.SafeRandomAccess(maxLevel) : damage.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", value2, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (CurrentLevelToIdx() < FirstLevel)
                return;
            if (avatar.IsDead || NetworkAvatar.IsDead || damage.fromType != EDamageFromType.DirectAttack)
                return;
            if (NetworkAvatar.Money < Consume.SafeRandomAccess(CurrentLevelToIdx()))
                return;

            if (avatar.monsterType != EMonsterType.Dummy)
                NetworkAvatar.AddMoney(-Consume.SafeRandomAccess(CurrentLevelToIdx()));

            DamageInstance damage2 = DamageInstance.GetDamage(NetworkAvatar, DamageId, avatar.transform.position, 4294967295L, 5, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
            damage2.elementalType = EDamageElementalType.Physical;
            avatar.ApplyDamage(damage2);
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
