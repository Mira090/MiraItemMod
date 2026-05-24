using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_DebuffByDefense :WeaponAddon
    {
        public CharacterDebuff[] Debuffs => new CharacterDebuff[] { SephiriaPrefabs.Poison, SephiriaPrefabs.Burn, SephiriaPrefabs.Frostbite, SephiriaPrefabs.Electric };
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack || avatar.IsDead)
                return;
            var defense = Mathf.Abs(parent.Networkowner.unitAvatar.GetCustomStat(ECustomStat.DamageReduction));
            if (defense > 0)
            {
                for (int q = 0; q < (defense / 10); q++)
                {
                    if(30.Percent())
                        avatar.ApplyDebuff(Debuffs.SafeRandomAccess(q, ArrayExtensions.ERandomAccessType.Repeat), parent.Networkowner.unitAvatar);
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
