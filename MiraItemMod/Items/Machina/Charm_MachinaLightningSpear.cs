using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaLightningSpear : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaLightningSpear";
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(1018);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var simple))
                return null;
            return simple.specialAttacks.FirstOrDefault();
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBaisAttackSwing += OnBaisAttackSwing;
            //NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBaisAttackSwing -= OnBaisAttackSwing;
            //NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
