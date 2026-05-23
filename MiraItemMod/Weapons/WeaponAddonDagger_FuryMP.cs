using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonDagger_FuryMP : WeaponAddonCommon_StatusUnsafe
    {
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            if(parent is WeaponSimple_Dagger dagger)
            {
                dagger.OnUseFury += OnUseFury;
            }
        }
        private void OnUseFury()
        {
            parent.Networkowner.unitAvatar.HealMpPercent(20);
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            if (parent is WeaponSimple_Dagger dagger)
            {
                dagger.OnUseFury -= OnUseFury;
            }
        }
    }
}
