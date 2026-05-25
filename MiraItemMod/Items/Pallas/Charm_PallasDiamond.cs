using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Pallas
{
    public class Charm_PallasDiamond : Charm_PallasEnhancer
    {
        public static ItemPosition[] Directions = new ItemPosition[]
        {
        new ItemPosition(0, -1),
        new ItemPosition(0, 1)
        };
        protected override ItemPosition[] GetDirections()
            => Directions;
        protected override void SetStats(CustomPallasController card)
        {
            card.HasDiamond = true;
        }
        protected override void ClearStats(CustomPallasController card)
        {
            card.HasDiamond = false;
        }
    }
}
