using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MiraItemMod.Items.Vitality
{
    public class Charm_WindSongVitality : Charm_StatusInstance
    {
        public static readonly string Stat = "HealAttackSpeed".ToSephiriaUpperId();
        public static readonly float Per = 0.5f;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, -1);
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("PER", Per.ToString())
            };
        }
        ///パッチはWeaponAddonCommon_HealWeaponRangeにある
    }
}
