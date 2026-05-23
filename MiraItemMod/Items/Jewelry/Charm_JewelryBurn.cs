using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_JewelryBurn : Charm_JewelryDebuff
    {
        public override CharacterDebuff Debuff => SephiriaPrefabs.Burn;
    }
}
