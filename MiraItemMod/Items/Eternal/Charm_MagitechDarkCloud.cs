using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Eternal
{
    public class Charm_MagitechDarkCloud : Charm_StatusInstance
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.id != "Ability_DarkCloud")
                return;
            avatar.ApplyDebuff(SephiriaPrefabs.Electric, NetworkAvatar);
        }
    }
}
