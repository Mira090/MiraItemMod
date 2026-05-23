using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryFlameSword : Charm_Jewelry
    {
        public static readonly string Status = "FlameSwordAdditionLeaf".ToUpperInvariant();

        public override int[] Consume => consumeMedium;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, Consume.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Status, -Consume.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe(Status, -Consume.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe(Status, Consume.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset))
            };
        }
        ///メモ
        /// Charm_JewelryFlameSwordの処理はここではなく、Charm_EmberFlameSwordのパッチで
    }
}
