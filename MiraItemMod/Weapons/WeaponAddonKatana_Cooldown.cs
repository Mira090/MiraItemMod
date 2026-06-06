using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonKatana_Cooldown : WeaponAddon
    {
        public float KatanaHeat = 0.03f;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.OnSwingCreated += OnSwingCreated;
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.OnSwingCreated -= OnSwingCreated;
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            if (damage.elementalType != EDamageElementalType.IceAndLightning)
                return;
            var combo = parent.Networkowner.unitAvatar.Inventory.FindComboEffect(ItemCategories.DarkCloud);
            if (combo is ComboEffect_DarkCloud darkCloud)
            {
                parent.Networkowner.unitAvatar.AddCustomStatUnsafe("DARKCLOUDICE", 1);
                darkCloud.UseCloudToTarget(avatar, 100);
                parent.Networkowner.unitAvatar.AddCustomStatUnsafe("DARKCLOUDICE", -1);
            }
        }

        private void OnSwingCreated(bool last, ProjectileBase projectile)
        {
            var player = parent.Networkowner.unitAvatar;
            if(parent is WeaponSimple_Katana katana)
            {
                katana.SetCurrentKatanaGauge(Mathf.Clamp01(katana.GetCurrentKatanaGauge()) + KatanaHeat);
                if(katana.GetCurrentKatanaGauge() > 1f)
                {
                    katana.SetCurrentKatanaGauge(1f);
                    var damage = DamageInstance.GetDamage(null, null, player.transform.position, long.MaxValue, UnityEngine.Random.Range(1f, 3f), EDamageType.ElementalEffectDamage, EDamageFromType.Trap, UnityEngine.Vector2.zero, 0, 0f);
                    katana.Networkowner.unitAvatar.ApplyDamage(damage);
                }
            }
        }
        //通常攻撃のNewWeaponFireData変更パッチはWeaponAddonKatana_Plasmaにある
    }
}
