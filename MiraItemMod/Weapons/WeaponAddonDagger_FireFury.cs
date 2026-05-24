using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonDagger_FireFury : WeaponAddon
    {
        int count = 0;
        int require = 10;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (parent is WeaponSimple_Dagger dagger)
            {
                if (dagger.currentFury >= dagger.maxFury + parent.Networkowner.unitAvatar.GetCustomStatUnsafe("DAGGERTRANCEBONUS"))
                    return;
                if (damage.IsSameElementalType(EDamageElementalType.Fire))
                {
                    count++;
                    if (count >= require)
                    {
                        count = 0;
                        if (dagger.currentFury == 0)
                        {
                            dagger.Networkowner.unitAvatar.CreateEffectHUD("IMMERSION", "DaggerImmersion");
                        }
                        dagger.Networkowner.unitAvatar.SetEffectHUDFlash("DaggerImmersion");
                        dagger.InvokeAddFury(1);
                        if (connectionToClient == null)
                        {
                            dagger.InvokeCreateFuryChargedParticle(new Color(0.176f, 1f, 0.616f, 1f));
                        }
                        else
                        {
                            dagger.InvokeTargetFuryChargedMessage(connectionToClient);
                        }
                    }
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
