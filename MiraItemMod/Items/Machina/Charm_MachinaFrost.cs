using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaFrost : Charm_MachinaBasic
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Charm_Machina.FrostToMachina, 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Charm_Machina.FrostToMachina, -1);
        }
    }
}
