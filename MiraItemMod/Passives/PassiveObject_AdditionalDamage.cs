using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Passives
{
    public class PassiveObject_AdditionalDamage : PassiveObject
    {
        public int Damage = 5;
        public string DamageId = "Perk_AdditionalDamage";

        protected override void OnEffectEnabled(PlayerAvatar player, bool runtime)
        {
            base.OnEffectEnabled(player, runtime);
            player.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (avatar.IsDead || player.IsDead || damage.fromType != EDamageFromType.DirectAttack)
                return;

            DamageInstance damage2 = DamageInstance.GetDamage(player, DamageId, avatar.transform.position, 4294967295L, 5, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
            damage2.elementalType = EDamageElementalType.Physical;
            avatar.ApplyDamage(damage2);
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[1] { new Loc.KeywordValue() { keyword = "DAMAGE", value = Damage.ToString() } };
        }
    }
}
