using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonDagger_IceDagger : WeaponAddonCommon_Status
    {
        public static readonly int ItemID = 3020;//霜の短剣

        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("NoFury".ToUpperInvariant(), 1);
        }
        public override void OnParry(DamageInstance damage)
        {
            base.OnParry(damage);

            if (parent.Networkowner.unitAvatar is PlayerAvatar player)
            {
                foreach (var charm in parent.Networkowner.unitAvatar.Inventory.charms.Values)
                {
                    if (charm.Item != null && charm.Item.EntityID == ItemID && charm is Charm_Magic magic)
                    {
                        magic.FireCasting(player.transform.position, player.transform.position, player.TopdownActor.CenterYPos, 1, false, false, null);
                        player.GetSkillController().SetLastUsedMagicServerside(magic);
                        return;
                    }
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("NoFury".ToUpperInvariant(), -1);
        }
    }
}
