using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_GoldRush : WeaponAddon
    {
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            foreach (var buff in parent.Networkowner.unitAvatar.Buffs)
            {
                if (buff.ID == Data.GoldRushBuff.ID)
                    return;
            }
            var percent = parent.Networkowner.unitAvatar.GetCustomStat(ECustomStat.Negotiation);
            if(percent > 0 && percent.Percent())
            {
                parent.Networkowner.unitAvatar.ApplyBuff(Data.GoldRushBuff, 1, parent.Networkowner.unitAvatar);
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
