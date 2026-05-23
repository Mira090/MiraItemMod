using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonCommon_CriticalFire : WeaponAddonCommon_StatusUnsafe
    {
        public float Cooldown = 0.2f;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.isCriticalAttack)
            {
                foreach(var charm in parent.Networkowner.unitAvatar.Inventory.charms.Values)
                {
                    if(charm is Charm_FireFeather feather && !feather.isFeatherEnabled)
                    {
                        if (feather.featherEnableTimer.Update(Cooldown))
                        {
                            feather.NetworkisFeatherEnabled = true;
                        }
                    }
                }
            }
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            List<Loc.KeywordValue> list = new List<Loc.KeywordValue>();
            for (int i = 0; i < status.Length; i++)
            {
                list.Add(new Loc.KeywordValue("VAL" + i, (status[i].value / 100).ToString()));
            }
            list.Add(new Loc.KeywordValue("COOLDOWN", Cooldown.ToString("0.#")));

            return list.ToArray();
        }
    }
}
