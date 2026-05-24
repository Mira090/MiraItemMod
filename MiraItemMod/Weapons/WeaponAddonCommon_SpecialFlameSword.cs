using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_SpecialFlameSword : WeaponAddonCommon_StatusUnsafe
    {
        public float range = 2f;
        public int count = 4;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.OnSpecialAttackSwing += OnSpecialAttack;
        }
        private void OnSpecialAttack(int idx)
        {
            var combo = parent.Networkowner.unitAvatar.Inventory.FindComboEffect(ItemCategories.FlameSword);
            if (combo == null || !combo.isEnabled)
                return;
            if(combo is ComboEffect_FlameSword flame)
            {
                var center = parent.Networkowner.unitAvatar.transform.position;
                var random = UnityEngine.Random.Range(0f, 360f);
                flame.NetworkcurrentSword = count;
                for (int q = 0; q < count; q++)
                {
                    var angle = (360f / count) * q + random;
                    var dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
                    var pos = center + dir * range;
                    flame.ServerFireSword(pos, true, false);
                }
                this.Delay(0.5f, () => {
                    if (!isEnabled)
                        return;
                    if (combo == null || !combo.isEnabled)
                        return;
                    if (parent.Networkowner.unitAvatar.IsDead)
                        return;
                    if (combo is ComboEffect_FlameSword flame2)
                    {
                        center = parent.Networkowner.unitAvatar.transform.position;
                        random = UnityEngine.Random.Range(0f, 360f);
                        flame.NetworkcurrentSword += count * 2;
                        for (int q = 0; q < count * 2; q++)
                        {
                            var angle = (360f / (count * 2)) * q + random;
                            var dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
                            var pos = center + dir * range * 2;
                            flame.ServerFireSword(pos, true, false);
                        }
                    }
                });
            }
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.OnSpecialAttackSwing -= OnSpecialAttack;
        }
    }
}
