using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaSliding : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaSliding";
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(420);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Katana>(out var simple))
                return null;
            return simple.enhancedDashAttackFireData_Ice;
        }
    }
}
