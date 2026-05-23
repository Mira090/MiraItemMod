using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_ChaosAttack : Charm_VariableMaxLevel, IAttackableCharm
    {
        public int[] chanceByLevel = new int[10] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        public int[] percentByLevel = new int[] { 10, 20, 25, 30, 35, 40, 50, 60, 70, 80, 90, 100 };
        public string damageId = "Charm_ChaosAttack";
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? chanceByLevel.SafeRandomAccess(0) + "→" + chanceByLevel.SafeRandomAccess(maxLevel) : chanceByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = ((avatar.GetCustomStat("FireDamage") + avatar.GetCustomStat("IceDamage") + avatar.GetCustomStat("LightningDamage")) * percentByLevel.SafeRandomAccess(LevelToIdx(level)) / 100).ToString();
            string value3 = showAllLevel ? percentByLevel.SafeRandomAccess(0) + "→" + percentByLevel.SafeRandomAccess(maxLevel) : percentByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("CHANCE", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value3 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", value2, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (!NetworkAvatar.IsDead && IsEffectEnabled && damageInstance.id != damageId && damageInstance.fromType == EDamageFromType.DirectAttack)
            {
                float customStatUnsafe = Charm_Basic.CalculateDamage(this);
                if (customStatUnsafe > 0 && unitAvatar != null && !(UnityEngine.Random.Range(0f, 1f) > chanceByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f))
                {
                    DamageInstance damage = DamageInstance.GetDamage(NetworkAvatar, damageId, unitAvatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    damage.SetCustomColor(true, new Color(0.5f, 0, 0));
                    unitAvatar.ApplyDamage(damage);
                }
            }
        }

        public float GetDamage(UnitAvatar avatar)
        {
            float customStatUnsafe = NetworkAvatar.GetCustomStat("FireDamage") + NetworkAvatar.GetCustomStat("IceDamage") + NetworkAvatar.GetCustomStat("LightningDamage");
            customStatUnsafe *= percentByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f;
            return customStatUnsafe;
        }
    }
}
