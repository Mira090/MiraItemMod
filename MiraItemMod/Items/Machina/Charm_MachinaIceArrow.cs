using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaIceArrow : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaIceArrow";
        protected override void Awake()
        {
            damageByLevel = new int[10] { 50, 55, 60, 65, 70, 80, 100, 120, 140, 170 };
        }
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(114);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Crossbow>(out var simple))
                return null;
            return simple.iceArrow;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBaisAttackSwing += OnBaisAttackSwing;
            NetworkAvatar.OnAttackUnit += OnAttack;
        }

        protected void OnAttack(UnitAvatar avatar, DamageInstance damage)
        {
            if(damage.id == DamageId)
            {
                avatar.ApplyDebuff(SephiriaPrefabs.Frostbite, NetworkAvatar);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBaisAttackSwing -= OnBaisAttackSwing;
            NetworkAvatar.OnAttackUnit -= OnAttack;
        }
    }
}
