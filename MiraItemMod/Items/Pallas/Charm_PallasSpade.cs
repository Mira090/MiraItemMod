using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Pallas
{
    public class Charm_PallasSpade : Charm_PallasEnhancer
    {
        public static ItemPosition[] Directions = new ItemPosition[]
        {
        new ItemPosition(1, -1),
        new ItemPosition(-1, 1)
        };
        protected override ItemPosition[] GetDirections()
            => Directions;
        protected override void SetStats(CustomPallasController card)
        {
            card.HasSpade = true;
        }
        protected override void ClearStats(CustomPallasController card)
        {
            card.HasSpade = false;
        }
    }
}
