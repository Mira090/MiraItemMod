using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_DrunkVitality : Charm_StatusInstance
    {
        public int[] heal = new int[] { 5, 5, 10 };
        public int[] percent = new int[] { 250 };
        public string damageId = "Charm_DrunkVitality";
        public LocalizedString item = new LocalizedString("Item_FirstHeal_Name");
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            int b = 0;
            string value = showAllLevel ? heal.SafeRandomAccess(0) + b + "→" + (heal.SafeRandomAccess(maxLevel) + b) : (heal.SafeRandomAccess(LevelToIdx(level)) + b).ToString();
            string value2 = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            //string value3 = (showAllLevel ? (seriesAddition.SafeRandomAccess(0) + "→" + seriesAddition.SafeRandomAccess(maxLevel)) : seriesAddition.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("HEAL", "+" + value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("ITEM", item.ToString())
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("DRUNKHEAL", heal.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.OnDamagedServerside += OnDamagedServerside;
        }

        private void OnDamagedServerside(DamageInstance damage)
        {
            if(damage.origin is UnitAvatar avatar && !avatar.IsDead)
            {
                float customStatUnsafe = damage.damageResult * (percent.SafeRandomAccess(CurrentLevelToIdx()) * 0.01f);

                if (customStatUnsafe > 0)
                {
                    DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damageId, avatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    //d.elementalType = EDamageElementalType.Chaos;
                    d.SetCustomColor(true, new Color(0.9f, 0.6f, 0.6f));
                    avatar.ApplyDamage(d);
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("DRUNKHEAL", -heal.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.OnDamagedServerside -= OnDamagedServerside;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe("DRUNKHEAL", -heal.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("DRUNKHEAL", heal.SafeRandomAccess(LevelToIdx(newLevel)));
        }
    }
}
