using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.StatusInstances
{
    public class PotionDrinkTimer : Timer
    {
        public UnitAvatar Avatar;
        public PotionDrinkTimer(UnitAvatar avatar)
        {
            Avatar = avatar;
        }

    }
}
