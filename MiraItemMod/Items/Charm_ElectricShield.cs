using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnitAvatar;

namespace MiraItemMod.Items
{
    public class Charm_ElectricShield : Charm_StatusInstance
    {
        public static readonly string ShieldKey = "Charm_ElectricShield";
        public int[] maxShield = new int[] { 10, 15, 20, 30 };
        public int[] percent = new int[] { 40, 55, 75, 100 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? maxShield.SafeRandomAccess(0) + "→" + maxShield.SafeRandomAccess(maxLevel) : maxShield.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("MAX", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("SHIELD", "1"),
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
            CreateShield(NetworkAvatar, ShieldKey, maxShield.SafeRandomAccess(CurrentLevelToIdx()));
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.id == "Debuff_Electric" || damage.id == "Debuff_Plasma")
            {
                CreateShield(NetworkAvatar, ShieldKey, maxShield.SafeRandomAccess(CurrentLevelToIdx()));
                AddShield(NetworkAvatar, ShieldKey, 1);
            }
            if(damage.id == "Ability_Thorns" && !avatar.IsDead && percent.SafeRandomAccess(CurrentLevelToIdx()).Percent())
            {
                avatar.ApplyDebuff(SephiriaPrefabs.Electric, NetworkAvatar);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
            NetworkAvatar.RemoveShield(ShieldKey);
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            var value = NetworkAvatar.GetShield(ShieldKey);
            NetworkAvatar.RemoveShield(ShieldKey);
            var shields = NetworkAvatar.GetCurShieldDatas();
            var shieldData = new ShieldData();
            shieldData.key = ShieldKey;
            shieldData.shield = value;
            shieldData.maxShield = maxShield.SafeRandomAccess(CurrentLevelToIdx());
            if (shieldData.shield > shieldData.maxShield)
                shieldData.shield = shieldData.maxShield;
            shields.Add(shieldData);
        }
        protected void CreateShield(UnitAvatar avatar, string key, int max, int value = 0)
        {
            var shields = avatar.GetCurShieldDatas();
            var contains = shields.FindIndex(x => x.key == key) >= 0;
            if (!contains)
            {
                var shieldData = new ShieldData();
                shieldData.key = ShieldKey;
                shieldData.shield = value;
                shieldData.maxShield = max;
                if (shieldData.shield > shieldData.maxShield)
                    shieldData.shield = shieldData.maxShield;
                shields.Add(shieldData);
            }
        }
        protected void AddShield(UnitAvatar avatar, string key, int value)
        {
            var shields = avatar.GetCurShieldDatas();
            var shield = shields.Find(x => x.key == key);
            if (shield == null)
                return;
            shield.shield += value;
            if (shield.shield > shield.maxShield)
                shield.shield = shield.maxShield;
        }
    }
}
