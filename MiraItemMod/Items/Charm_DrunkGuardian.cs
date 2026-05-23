using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DrunkGuardian : Charm_StatusInstance, IAttackableCharm
    {
        public int[] percent = new int[] { 1000, 1500, 2000, 2500 };
        public int[] minus = new int[] { 100 };
        public string damageId = "Charm_DrunkGuardian";
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = "-";
            float weight = 1;
            float minusWeight = 1;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                if(!ignoreAvatarStatus)
                    value = GetDamage(avatar, component.currentWeapon).ToString("0.##");
                weight = component.currentWeapon.AttackWeightPerSwing;
                minusWeight = 1;
            }
            string value2 = showAllLevel ? (percent.SafeRandomAccess(0) * weight).ToString("0.##") + "→" + (percent.SafeRandomAccess(maxLevel) * weight).ToString("0.##") : (percent.SafeRandomAccess(LevelToIdx(level)) * weight).ToString("0.##");
            string value3 = showAllLevel ? (minus.SafeRandomAccess(0) * minusWeight).ToString("0.##") + "→" + (minus.SafeRandomAccess(maxLevel) * minusWeight).ToString("0.##") : (minus.SafeRandomAccess(LevelToIdx(level)) * minusWeight).ToString("0.##");
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DAMAGE", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("MINUS", value3 + "%", GetNegativeColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if(damage.fromType == EDamageFromType.DirectAttack)
            {
                var customStatUnsafe = Charm_Basic.CalculateDamage(this);
                if (customStatUnsafe > 0)
                {
                    DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damageId, avatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
                    d.elementalType = EDamageElementalType.Physical;
                    d.SetCustomColor(false, new Color(0.4f, 0.4f, 0.4f));
                    avatar.ApplyDamage(d);
                }
            }
        }
        private float GetDamage(UnitAvatar avatar, WeaponSimple weapon)
        {
            float weight = weapon.attackWeightPerSwing;
            int drunk = avatar.Inventory.charms.Values.Count(x => x.GetItemCategory().Contains(ItemCategories.Drunk));
            int guardian = avatar.Inventory.charms.Values.Count(x => x.GetItemCategory().Contains(ItemCategories.Guardian));
            float damage = drunk * guardian * (percent.SafeRandomAccess(CurrentLevelToIdx()) * weight / 100f) - Mathf.Abs(avatar.GetCustomStat(ECustomStat.DamageReduction)) * (minus.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            return damage;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        public float GetDamage(UnitAvatar avatar)
        {
            float weight = WeaponController.currentWeapon.attackWeightPerSwing;
            int drunk = avatar.Inventory.charms.Values.Count(x => x.GetItemCategory().Contains(ItemCategories.Drunk));
            int guardian = avatar.Inventory.charms.Values.Count(x => x.GetItemCategory().Contains(ItemCategories.Guardian));
            float damage = drunk * guardian * (percent.SafeRandomAccess(CurrentLevelToIdx()) * weight / 100f) - Mathf.Abs(avatar.GetCustomStat(ECustomStat.DamageReduction)) * (minus.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            return damage;
        }
    }
}
