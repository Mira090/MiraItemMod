using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaIssen : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaIssen";
        public override float AttackDashScale => 1f;
        protected override void Awake()
        {
            cooldownTimer.time = 0.2f;
        }
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(405);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Katana>(out var simple))
                return null;
            return simple.electricChargeIssenFireData;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSwingCreated_Dash += OnSwingCreated_Dash;
            //NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSwingCreated_Dash -= OnSwingCreated_Dash;
            //NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        protected void OnSwingCreated_Dash(ProjectileBase projectile)
        {
            if (isInCooldown)
                return;
            if (FireData == null)
                return;
            isInCooldown = true;
            Attack();
        }
        protected override float ModifyDamage(float damage)
        {
            damage += damage * NetworkAvatar.GetCustomStat(ECustomStat.DashAttackDamageBonus) / 100f;
            return base.ModifyDamage(damage);
        }
    }
}
