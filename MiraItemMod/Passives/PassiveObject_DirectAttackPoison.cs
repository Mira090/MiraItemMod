using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Passives
{
    public class PassiveObject_DirectAttackPoison : PassiveObject_StatusInstance
    {
        public bool IsEffectEnabled = false;
        public Timer cooldownTimer = new Timer(1f);
        public bool isInCooldown;
        protected override void OnEffectEnabled(PlayerAvatar player, bool runtime)
        {
            base.OnEffectEnabled(player, runtime);
            player.OnAttackUnit += OnAttackUnit;
            IsEffectEnabled = true;
        }
        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.OnAttackUnit -= OnAttackUnit;
            IsEffectEnabled = false;
        }
        protected void Update()
        {
            if (!base.isServer || !IsEffectEnabled)
                return;
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            if (!isInCooldown)
            {
                isInCooldown = true;
                avatar.ApplyDebuff(SephiriaPrefabs.Poison, player);
            }
        }
    }
}
