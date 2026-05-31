using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_CompanionDiedHeal : Charm_StatusInstance
    {
        private int[] healByLevel = new int[] { 2, 2, 3, 5, 8 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? healByLevel.SafeRandomAccess(0) + "→" + healByLevel.SafeRandomAccess(maxLevel) : healByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("HEAL", value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnFollowerAddedServerside += OnFollowerAdded;
            NetworkAvatar.OnFollowerRemovedServerside += OnFollowerRemoved;
            foreach (UnitAvatar follower in NetworkAvatar.Followers)
                follower.OnDie += OnFollowerDie;
            //ModEvent.OnSummonUnitDied += OnSummonUnitDied;
        }

        private void OnFollowerDie(DamageInstance damage)
        {
            if (damage == null || damage.id == "Self")
                return;
            //Core.Logger("Heal: " + healByLevel.SafeRandomAccess(CurrentLevelToIdx()));
            if (damage.victim is UnitAvatar avatar)
                NetworkAvatar.SetHp(NetworkAvatar.hp + healByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f * avatar.MaxHp);
        }
        private void OnFollowerAdded(UnitAvatar follower)
        {
            follower.OnDie += OnFollowerDie;
        }

        private void OnFollowerRemoved(UnitAvatar follower)
        {
            follower.OnDie -= OnFollowerDie;
        }

        private void OnSummonUnitDied(Charm_SummonUnit instance, DamageInstance damage)
        {
            if (instance.NetworkAvatar == NetworkAvatar)
            {
                Core.LoggerMedium("Heal: " + healByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                NetworkAvatar.Heal(healByLevel.SafeRandomAccess(CurrentLevelToIdx()), true);
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnFollowerAddedServerside -= OnFollowerAdded;
            NetworkAvatar.OnFollowerRemovedServerside -= OnFollowerRemoved;
            foreach (UnitAvatar follower in NetworkAvatar.Followers)
                follower.OnDie -= OnFollowerDie;
            //ModEvent.OnSummonUnitDied -= OnSummonUnitDied;
        }
    }
}
