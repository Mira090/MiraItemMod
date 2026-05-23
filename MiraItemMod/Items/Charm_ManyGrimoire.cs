using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_ManyGrimoire : Charm_Basic
    {
        public int[] criticalBonusByLevel = new int[] { 1, 1, 2, 2, 3 };

        private int enabledCriticalBonus;

        private int savedStoneTabletCount;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? ((float)criticalBonusByLevel.SafeRandomAccess(0)).ToString("0.##") + "→" + ((float)criticalBonusByLevel.SafeRandomAccess(maxLevel)).ToString("0.##") : ((float)criticalBonusByLevel.SafeRandomAccess(LevelToIdx(level))).ToString("0.##");
            int num = 0;
            if ((bool)avatar)
            {
                num = avatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
            }

            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("STATUS", "+" + value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", num.ToString())
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer += OnItemUpdated;
                savedStoneTabletCount = NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
                enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
                NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
                NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
                NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            savedStoneTabletCount = 0;
            enabledCriticalBonus = 0;
            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer -= OnItemUpdated;
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(LevelToIdx(newLevel)) * savedStoneTabletCount;
            NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
        }

        private void OnItemUpdated()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            savedStoneTabletCount = NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
            enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
            NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
            NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
