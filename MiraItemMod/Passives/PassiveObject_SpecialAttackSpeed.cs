using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Passives
{
    public class PassiveObject_SpecialAttackSpeed : PassiveObject
    {
        protected override void OnEffectEnabled(PlayerAvatar player, bool runtime)
        {
            base.OnEffectEnabled(player, runtime);
            player.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;

            //player.ApplyBuff(Data.SpecialAttackSpeedBuff, 1, player, true);
        }

        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.OnAttackUnit -= OnAttackUnit;
        }
    }
}
