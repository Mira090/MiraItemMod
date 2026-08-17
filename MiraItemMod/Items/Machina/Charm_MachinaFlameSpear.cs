using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaFlameSpear : Charm_Machina
    {
        protected override void Awake()
        {
            damageByLevel = new int[10] { 115, 120, 130, 140, 150, 160, 170, 190, 210, 240 };
        }
        public override string DamageId => "Charm_MachinaFlameSpear";
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(519);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponAddonCommon_ChangeWeaponAction>(out var addon))
                return null;
            return addon.fireData;
        }
        protected override float ModifyDamage(float damage)
        {
            damage += damage * NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDDAMAGE") / 100f;
            return base.ModifyDamage(damage);
        }
        ///パッチはCharm_EmberFlameSword
    }
}
