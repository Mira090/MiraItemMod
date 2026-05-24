using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_SeparateDirectAttack : Charm_StatusInstance
    {
        public int[] separate = new int[] { 56, 52, 48, 42, 36 };
        public Timer cooldown = new Timer(0.1f);
        public int cooldownCount = 0;
        public int countMax = 5;
        public bool IsInCooldown => cooldownCount > countMax;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? separate.SafeRandomAccess(0) + "→" + separate.SafeRandomAccess(maxLevel) : separate.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("DAMAGE", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if(cooldownCount > 0 && cooldown.Update(Time.deltaTime))
            {
                cooldownCount = 0;
            }
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (IsInCooldown)
                return;
            if (damage.id == "Charm_MinHPKill")//白い卵の殻は除外
                return;
            if(damage.fromType == EDamageFromType.DirectAttack && damage.damage >= separate.SafeRandomAccess(CurrentLevelToIdx()))
            {
                damage.damage -= damage.damage / 2f;
                cooldownCount++;
                var color = damage.useCustomColor;
                if (!color)
                    damage.SetCustomColor(false, new Color(0.25f, 0.25f, 0.8f));

                float customStatUnsafe = damage.damage;
                if (customStatUnsafe > 0 && avatar != null && !avatar.IsDead)
                {
                    DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damage.id, avatar.transform.position, 4294967295L, customStatUnsafe, damage.damageType, damage.fromType, damage.direction, damage.staggeringLevel, damage.externalForcePower);
                    d.elementalType = damage.elementalType;
                    if (!color)
                        d.SetCustomColor(false, new Color(0.8f, 0.25f, 0.25f));
                    else
                        d.SetCustomColor(false, damage.color);
                    avatar.ApplyDamage(d);
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
    }
}
