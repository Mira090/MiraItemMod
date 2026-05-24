using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Jewelry
{
    public class Charm_JewelrySturdy : Charm_Jewelry
    {
        public int[] amp = new int[] { 0, 0, 1, 2, 3 };
        public override int[] Consume => consumeSmall;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? (amp.SafeRandomAccess(0) * 8) + "→" + (amp.SafeRandomAccess(maxLevel) * 8) : (amp.SafeRandomAccess(LevelToIdx(level) * 8)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("DAMAGE", "+" + value2 + "%", GetNegativeColor(virtualLevelOffset)),
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBeginAttackAnimation += OnBeginAttackAnimation;
            WeaponController.OnBeginSpecialAttackAnimation += OnBeginAttackAnimation;
            WeaponController.OnBeginDashAttackAnimation += OnBeginAttackAnimation;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBeginAttackAnimation -= OnBeginAttackAnimation;
            WeaponController.OnBeginSpecialAttackAnimation -= OnBeginAttackAnimation;
            WeaponController.OnBeginDashAttackAnimation -= OnBeginAttackAnimation;
        }
        private void OnBeginAttackAnimation(int idx)
        {
            OnBeginAttackAnimation();
        }
        private void OnBeginAttackAnimation()
        {
            if (CurrentLevelToIdx() < FirstLevel)
                return;
            var money = Consume.SafeRandomAccess(CurrentLevelToIdx());
            if (NetworkAvatar.Money >= money && !HasBuff())
            {
                NetworkAvatar.AddMoney(-money);
                NetworkAvatar.ApplyBuff(Data.WeaponDamageBuff, amp.SafeRandomAccess(CurrentLevelToIdx()), NetworkAvatar);
            }
        }
        private bool HasBuff()
        {
            foreach(var buff in NetworkAvatar.Buffs)
            {
                if(buff.ID == Data.WeaponDamageBuff.ID)
                {
                    var a = amp.SafeRandomAccess(CurrentLevelToIdx());
                    return buff.Amplified >= a;
                }
            }
            return false;
        }
    }
}
