using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Passives
{
    public class PassiveObject_TrueBuff : PassiveObject
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

            player.ApplyBuff(Data.TrueBuff, 1, player, true);
        }

        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.OnAttackUnit -= OnAttackUnit;
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("VAL0", "1"),
                new Loc.KeywordValue("VAL1", "8"),
            };
        }
    }
}
