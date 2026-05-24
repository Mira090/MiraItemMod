using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Savvy
{
    public class Charm_SavvyPrecision : Charm_StatusInstance
    {
        public int[] moneyByLevel = new int[] { 2, 3, 5 };
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            var execute = Mathf.Clamp(damage.criticalChancePercent - 100f, 0f, 100f);

            if (execute.Percent())
            {
                damage.criticalChancePercent = 0;
                damage.isCriticalAttack = true;
                damage.useCustomColor = true;

                var add = moneyByLevel.SafeRandomAccess(CurrentLevelToIdx());
                var count = 1 + NetworkAvatar.GetCustomStatUnsafe("JewelryCount".ToSephiriaUpperId());
                int money = ((NetworkAvatar.Money + add) / (500 * count)) * 1;
                if (money > 0 && money.Percent())
                {
                    if(avatar.monsterType != EMonsterType.Dummy)
                        this.AddRandomJewelry();
                    damage.color = ModUtil.Excavation;
                }
                else
                {
                    damage.color = ModUtil.ExcavationFaild;
                }

                var addDamage = NetworkAvatar.GetCustomStatUnsafe("ExcavationDamage".ToSephiriaUpperId());
                damage.damage += damage.damage * addDamage / 100f;
            }
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            int? percent = null;
            if(avatar != null && !ignoreAvatarStatus)
            {
                var add = moneyByLevel.SafeRandomAccess(LevelToIdx(level));
                var count = 1 + avatar.GetCustomStatUnsafe("JewelryCount".ToSephiriaUpperId());
                percent = ((avatar.Money + add) / (500 * count)) * 1;
            }
            string value = showAllLevel ? moneyByLevel.SafeRandomAccess(0) + "→" + moneyByLevel.SafeRandomAccess(maxLevel) : moneyByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("LEAF", "+" + value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", (percent.HasValue ? percent.Value.ToString() : "-") + "%")
            };
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (avatar.monsterType == EMonsterType.Dummy)
                return;
            if (damage.isCriticalAttack)
            {
                var add = moneyByLevel.SafeRandomAccess(CurrentLevelToIdx());
                //add += add * NetworkAvatar.GetCustomStat(ECustomStat.MoneyDrop) / 100;
                //SephiriaPrefabs.SpawnMoney(add, avatar.transform.position);
                NetworkAvatar.AddMoney(add);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
