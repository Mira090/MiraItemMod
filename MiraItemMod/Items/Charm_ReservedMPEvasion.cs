using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_ReservedMPEvasion : Charm_StatusInstance
    {
        //public int[] evasionByLevel = new int[4] { 2, 3, 5, 7 };

        public int[] mpReserveByLevel = new int[4] { 24, 18, 12, 8 };

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //string value = (showAllLevel ? (evasionByLevel.SafeRandomAccess(0) + "→" + evasionByLevel.SafeRandomAccess(maxLevel)) : evasionByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value2 = showAllLevel ? mpReserveByLevel.SafeRandomAccess(0) + "→" + mpReserveByLevel.SafeRandomAccess(maxLevel) : mpReserveByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("MANA", value2, GetNegativeColor(virtualLevelOffset)),
            //new Loc.KeywordValue("PERCENT", value, Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.NetworkreservedMp = networkAvatar.reservedMp + mpReserveByLevel.SafeRandomAccess(CurrentLevelToIdx());
            //base.NetworkAvatar.AddCustomStat(ECustomStat.Evasion, evasionByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.NetworkreservedMp = networkAvatar.reservedMp - mpReserveByLevel.SafeRandomAccess(CurrentLevelToIdx());
            //base.NetworkAvatar.AddCustomStat(ECustomStat.Evasion, -evasionByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.NetworkreservedMp = networkAvatar.reservedMp - mpReserveByLevel.SafeRandomAccess(LevelToIdx(oldLevel));
            networkAvatar.NetworkreservedMp = networkAvatar.reservedMp + mpReserveByLevel.SafeRandomAccess(LevelToIdx(newLevel));
            //base.NetworkAvatar.AddCustomStat(ECustomStat.Evasion, -evasionByLevel.SafeRandomAccess(LevelToIdx(oldLevel)));
            //base.NetworkAvatar.AddCustomStat(ECustomStat.Evasion, evasionByLevel.SafeRandomAccess(LevelToIdx(newLevel)));
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
