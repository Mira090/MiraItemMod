using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_GuardFrostbite : Charm_StatusInstance
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnGuardSucceeded += OnGuardSucceeded;
        }

        private void OnGuardSucceeded(DamageInstance damage, bool perfect)
        {
            if(damage.origin is UnitAvatar attacker && !attacker.IsDead && SephiriaPrefabs.Frostbite is CharacterDebuff_Frostbite frostbite)
            {
                if (perfect)
                {
                    attacker.ApplyDebuff(frostbite.freezeDebuffPrefab, NetworkAvatar);
                }
                else
                {
                    attacker.ApplyDebuff(frostbite, NetworkAvatar);
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnGuardSucceeded -= OnGuardSucceeded;
        }
    }
}
