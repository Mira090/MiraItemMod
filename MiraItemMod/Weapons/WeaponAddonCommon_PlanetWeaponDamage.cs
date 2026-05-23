using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonCommon_PlanetWeaponDamage : WeaponAddon
    {
        private float timer = 0;
        private int stat = 0;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void Update()
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.FinalWeaponDamage, -stat);
                    stat = 0;
                }
            }
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (ModUtil.PlanetDamageIds.Contains(damage.id))
            {
                timer += 3;
                if(stat < 50)
                {
                    parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.FinalWeaponDamage, 1);
                    stat += 1;
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
            timer = 0;
            parent.Networkowner.unitAvatar.AddCustomStat(ECustomStat.FinalWeaponDamage, -stat);
            stat = 0;
        }
    }
}
