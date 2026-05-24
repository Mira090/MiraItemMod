using Mirror;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_HalfHP : Charm_StatusInstance
    {
        public int[] alldamage = new int[] { 1, 2, 4, 8, 10, 12 };
        public int[] tough = new int[] { 1, 2, 4, 8, 10, 12 };
        public bool HasMoreHP => NetworkAvatar.HpRatio >= 0.5f;
        public bool moreHpEffect = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? alldamage.SafeRandomAccess(0) + "→" + alldamage.SafeRandomAccess(maxLevel) : alldamage.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? tough.SafeRandomAccess(0) + "→" + tough.SafeRandomAccess(maxLevel) : tough.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
                new Loc.KeywordValue("DAMAGE", "+" + value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("TOUGHNESS", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDamageApplied += OnDamageApplied;
            moreHpEffect = HasMoreHP;
            if (moreHpEffect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, alldamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            else
            {
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", tough.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        private void OnDamageApplied(DamageInstance damage)
        {
            if (moreHpEffect == HasMoreHP)
                return;
            if (moreHpEffect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, -alldamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            else
            {
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", -tough.SafeRandomAccess(CurrentLevelToIdx()));
            }
            moreHpEffect = HasMoreHP;
            if (moreHpEffect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, alldamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            else
            {
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", tough.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDamageApplied -= OnDamageApplied;
            if (moreHpEffect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, -alldamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            else
            {
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", -tough.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (moreHpEffect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, -alldamage.SafeRandomAccess(LevelToIdx(oldLevel)));
                NetworkAvatar.AddCustomStat(ECustomStat.AllDamageBonus, alldamage.SafeRandomAccess(LevelToIdx(newLevel)));
            }
            else
            {
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", -tough.SafeRandomAccess(LevelToIdx(oldLevel)));
                NetworkAvatar.AddCustomStatUnsafe("TOUGHNESS", tough.SafeRandomAccess(LevelToIdx(newLevel)));
            }
        }
    }
}
