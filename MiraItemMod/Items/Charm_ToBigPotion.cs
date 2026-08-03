using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_ToBigPotion : Charm_StatusInstance
    {
        public NewItemOwnInstance aquired = null;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.Inventory.OnItemAddedForServer += OnItemAddedForServer;
        }

        private void OnItemAddedForServer(NewItemOwnInstance added)
        {
            if (added.Entity != null && added.Entity.id == 0)
            {
                aquired = added;
            }
        }
        protected override void OnUpdate()
        {
            if (aquired == null)
                return;
            if(aquired.Quantity < 1)
            {
                aquired = null;
                return;
            }
            ItemPosition pos = new ItemPosition(aquired.XIdx, aquired.YIdx);
            GridInventory inventory = Inventory;
            int instanceID = aquired.InstanceID;

            using (new GridInventory.Permission(inventory))
            {
                inventory.ForceRemoveItem(pos);
            }

            inventory.AddItemAtPosition(new ItemMetadata(instanceID, 1, aquired.Quantity), pos);
            aquired = null;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.Inventory.OnItemAddedForServer -= OnItemAddedForServer;
        }
    }
}
