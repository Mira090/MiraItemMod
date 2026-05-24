using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Eternal
{
    public class Charm_GlacierFrost : Charm_StatusInstance
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget += OnAttackUnit;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAttackUnit;
        }

        private void OnAttackUnit(CharacterDebuff debuff, string id)
        {
            if (id != "FREEZE")
                return;
            foreach(var charm in NetworkAvatar.Inventory.charms.Values)
            {
                if (!charm.IsEffectEnabled)
                    continue;
                if (charm is Charm_IceHammer hammer)
                {
                    hammer.chargingCharm.AddTimer(2f);
                }
                if (charm is Charm_IceSpear spear)
                {
                    spear.chargingCharm.AddTimer(2f);
                }
                if (charm is Charm_AirSlash slash)
                {
                    slash.chargingCharm.AddTimer(2f);
                }
                if (charm is Charm_IceBow bow)
                {
                    bow.chargingCharm.AddTimer(2f);
                    if (bow.NetworkreadyArrowCount < bow.arrowReloadLimit)
                        bow.NetworkreadyArrowCount = bow.readyArrowCount + 2;
                }
            }
        }
    }
}
