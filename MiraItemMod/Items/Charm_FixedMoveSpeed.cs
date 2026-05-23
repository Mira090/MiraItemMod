using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_FixedMoveSpeed : Charm_StatusInstance
    {
        public string Id = "MOVESPEED";
        public int[] Status = new int[] { 100, 200, 400, 1000 };
        private float stat = 0;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Status.SafeRandomAccess(0) + "→" + Status.SafeRandomAccess(maxLevel) : Status.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            stat = Status.SafeRandomAccess(CurrentLevelToIdx()) / 100f - NetworkAvatar.NetworkmoveSpeedMultiplier;
            NetworkAvatar.NetworkmoveSpeedMultiplier += stat;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.NetworkmoveSpeedMultiplier -= stat;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.NetworkmoveSpeedMultiplier -= stat;
            stat = Status.SafeRandomAccess(CurrentLevelToIdx()) / 100f - NetworkAvatar.NetworkmoveSpeedMultiplier;
            NetworkAvatar.NetworkmoveSpeedMultiplier += stat;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.NetworkmoveSpeedMultiplier -= stat;
            stat = Status.SafeRandomAccess(LevelToIdx(newLevel)) / 100f - NetworkAvatar.NetworkmoveSpeedMultiplier;
            NetworkAvatar.NetworkmoveSpeedMultiplier += stat;
        }
    }
}
