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
            {
                Core.LoggerError("[Machina] Faild FindWeaponById(420)");
                return null;
            }
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Katana>(out var simple))
            {
                Core.LoggerError("[Machina] Faild TryGetComponent<WeaponSimple_Katana>");
                return null;
            }
            return simple.enhancedDashAttackFireData_Ice;
        }
    }
}
