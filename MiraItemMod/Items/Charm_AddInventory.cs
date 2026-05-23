using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_AddInventory : Charm_Basic
    {
        private bool reward = false;
        private bool rewardReceived = false;
        public short inventory = 2;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            //string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel)) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COUNT", inventory.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            reward = true;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (reward && !rewardReceived)
            {
                //NetworkAvatar.Inventory.AddStorage(inventory);

                var instance = StatusDatabase.CreateStatusEntity("InventorySlot".ToSephiriaId(), inventory);
                NetworkAvatar.AddOrphanedStatusInstance(instance);
                if (Core.LogMedium)
                    Core.Logger("Add Status: " + instance.ToString(false, false, false));

                using (new GridInventory.Permission(NetworkAvatar.Inventory))
                {
                    NetworkAvatar.Inventory.ForceRemoveItem(Item.XIdx, Item.YIdx);
                }
                rewardReceived = true;
            }
        }
    }
}
