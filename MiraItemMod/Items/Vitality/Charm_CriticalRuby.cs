using Miniscript;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Vitality
{
    public class Charm_CriticalRuby : Charm_StatusInstance
    {
        public static readonly string Stat = "CriticalRuby".ToSephiriaUpperId();
        public static readonly int Critical = 100;
        public static readonly int Max = 500;

        public static readonly LocalizedString Charm = new LocalizedString("Item_FinalHP_Name");

        public int percent = 5;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string current = "-%";
            if(avatar && !ignoreAvatarStatus)
            {
                current = GetCurrentBonus(avatar) + "%";
            }
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("CURRENT", current, Color.yellow),
                new Loc.KeywordValue("CRITICAL", (Critical / 100f).ToString() + "%"),
            new Loc.KeywordValue("ITEM", Charm.ToString()),
            new Loc.KeywordValue("PERCENT", percent + "%"),
            new Loc.KeywordValue("MAX", Max + "%")
            };
        }
        public int GetCurrentBonus(UnitAvatar avatar)
        {
            var critical = avatar.GetCustomStat(ECustomStat.Critical);
            var percent = critical / Charm_CriticalRuby.Critical;
            return percent;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, percent);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, -percent);
        }
    }
}
