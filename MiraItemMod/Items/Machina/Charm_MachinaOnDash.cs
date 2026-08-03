using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaOnDash : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaOnDash";
        protected override void Awake()
        {
            cooldownTimer.time = 0.1f;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDashServerside += OnDashServerside;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDashServerside -= OnDashServerside;
        }
        private void OnDashServerside(Vector2 motionTo, bool consume)
        {
            if (isInCooldown)
                return;
            isInCooldown = true;
            Attack();
        }
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(1026);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var simple))
                return null;
            return simple.haetaeStrikeAttackExplosionFireData;
        }
    }
}
