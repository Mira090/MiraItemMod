using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Sacrifice
{
    public class Charm_Sacrifice : Charm_StatusInstance
    {
        public ItemEntity rewardEntity;
        protected bool quest = false;
        protected bool reward = false;
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (quest && !reward && rewardEntity != null)
            {
                ItemPosition pos = new ItemPosition(Item.XIdx, Item.YIdx);
                GridInventory inventory = Inventory;
                int instanceID = Item.InstanceID;

                using (new GridInventory.Permission(inventory))
                {
                    inventory.ForceRemoveItem(Item.XIdx, Item.YIdx);
                }
                inventory.AddItemAtPosition(new ItemMetadata(instanceID, rewardEntity, 1), pos);
                reward = true;
            }
        }
    }
}
