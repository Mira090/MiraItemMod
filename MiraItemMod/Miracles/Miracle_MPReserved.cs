using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Miracles
{
    public class Miracle_MPReserved : Miracle_StatusInstance
    {
        protected override void SetOwnerInner(UnitAvatar owner)
        {
            base.SetOwnerInner(owner);
            Owner.NetworkreservedMp += 8;
        }
        protected override void DestroyInner()
        {
            base.DestroyInner();
            Owner.NetworkreservedMp -= 8;
        }
    }
}
