using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaEclipse : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaEclipse";
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(422);
            if (weapon == null)
            {
                Core.LoggerError("[Machina] Faild FindWeaponById(422)");
                return null;
            }
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Katana>(out var simple))
            {
                Core.LoggerError("[Machina] Faild TryGetComponent<WeaponSimple_Katana>");
                return null;
            }
            return simple.eclipseBasicAttackFireDatas.LastOrDefault();
        }
        public override EDamageElementalType? GetDamageElementalType(NewWeaponFireData fireData)
        {
            return EDamageElementalType.Fire;
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
