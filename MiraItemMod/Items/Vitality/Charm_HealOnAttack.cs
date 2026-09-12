using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Vitality
{
    public class Charm_HealOnAttack : Charm_StatusInstance
    {
        public Timer cooldownTimer = new Timer(1f);
        public bool isInCooldown;

        public float[] heal = new float[] { 0.1f, 0.2f, 0.3f, 0.5f };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? heal.SafeRandomAccess(0) + "→" + heal.SafeRandomAccess(maxLevel) : heal.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("HEAL", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (avatar.monsterType == EMonsterType.Dummy)
                return;
            if (isInCooldown)
                return;
            isInCooldown = true;
            NetworkAvatar.HealPercent(heal.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && !NetworkAvatar.IsDead && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
    }
}
