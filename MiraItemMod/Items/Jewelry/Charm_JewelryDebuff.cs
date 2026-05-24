using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Jewelry
{
    public class Charm_JewelryDebuff : Charm_Jewelry
    {
        public override int[] Consume => consumeMedium;
        public virtual CharacterDebuff Debuff => SephiriaPrefabs.Poison;

        public Timer cooldownTimer = new Timer(0.5f);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? Consume.SafeRandomAccess(0) + "→" + Consume.SafeRandomAccess(maxLevel) : Consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEAF", value, GetNegativeColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (CurrentLevelToIdx() < FirstLevel)
                return;
            if (isInCooldown || damage.fromType != EDamageFromType.DirectAttack|| avatar.IsDead)
                return;
            if(NetworkAvatar.Money >= Consume.SafeRandomAccess(CurrentLevelToIdx()))
            {
                isInCooldown = true;
                if(avatar.monsterType != EMonsterType.Dummy)
                    NetworkAvatar.AddMoney(-Consume.SafeRandomAccess(CurrentLevelToIdx()));
                avatar.ApplyDebuff(Debuff, NetworkAvatar);
            }
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
    }
}
