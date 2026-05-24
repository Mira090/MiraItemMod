using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_BondMaker : Charm_StatusInstance
    {
        private int requireLevel = 3;
        private bool quest = false;
        private bool reward = false;
        private Sprite charmSprite;
        private void Start()
        {
            charmSprite = AssetLoader.LoadSprite(ModUtil.MiscPath + "DealUIArtifact");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEVEL", requireLevel.ToString())
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (quest && !reward)
            {
                CreateBond();
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            quest = LevelToIdx(newLevel) >= requireLevel && CanCreateBond();
        }
        private bool CanCreateBond()
        {
            ItemPosition pos1 = new ItemPosition(Item.XIdx - 1, Item.YIdx);
            ItemPosition pos2 = new ItemPosition(Item.XIdx + 1, Item.YIdx);
            GridInventory inventory = Inventory;
            var item1 = inventory.FindItem(pos1);
            var item2 = inventory.FindItem(pos2);

            if (item1 != null && (bool)item1.Charm && item1.Charm.IsEffectEnabled && item1.Charm.GetItemCategory().Count() > 0 && item1.Entity != null && item1.Entity.rarity != ECustomItemRarity.Sacrifice.ToSephiria())
            {
                if (item2 != null && (bool)item2.Charm && item2.Charm.IsEffectEnabled && item2.Charm.GetItemCategory().Count() > 0 && item2.Entity != null && item2.Entity.rarity != ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    return true;
                }
            }
            return false;
        }
        private void CreateBond()
        {
            ItemPosition pos = new ItemPosition(Item.XIdx, Item.YIdx);
            ItemPosition pos1 = new ItemPosition(Item.XIdx - 1, Item.YIdx);
            ItemPosition pos2 = new ItemPosition(Item.XIdx + 1, Item.YIdx);
            GridInventory inventory = Inventory;
            int instanceID = Item.InstanceID;
            var item1 = inventory.FindItem(pos1);
            var item2 = inventory.FindItem(pos2);
            if (item1 != null && (bool)item1.Charm && item1.Charm.IsEffectEnabled && item1.Entity != null && item1.Entity.rarity != ECustomItemRarity.Sacrifice.ToSephiria())
            {
                if (item2 != null && (bool)item2.Charm && item2.Charm.IsEffectEnabled && item2.Entity != null && item2.Entity.rarity != ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    var categories = new List<string>();
                    categories.AddRange(item1.Charm.GetItemCategory());
                    categories.AddRange(item2.Charm.GetItemCategory());
                    var list = new List<ItemEntity>();
                    foreach(var item in ReflectionExtensions.GetItemDictionary().Values)
                    {
                        if (item.isDual && item.categories.All(c => categories.Contains(c)) && item.rarity != EItemRarity.Eternal)
                        {
                            list.Add(item);
                        }
                    }
                    if (list.Count > 0)
                    {
                        using (new GridInventory.Permission(inventory))
                        {
                            inventory.ForceRemoveItem(pos.x, pos.y);
                            inventory.ForceRemoveItem(pos1.x, pos1.y);
                            inventory.ForceRemoveItem(pos2.x, pos2.y);
                        }
                        inventory.AddItemAtPosition(new ItemMetadata(instanceID, list.GetRandom(), 1), pos);
                        reward = true;
                    }
                }

            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            Inventory.UpdatePing(Item.Position);
            if (!quest)
                quest = CurrentLevelToIdx() >= requireLevel && CanCreateBond();
        }

        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }
            var list = new List<CharmConnectionData>();

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(Item.XIdx - 1), Item.YIdx));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
            {
                list.Add(new CharmConnectionData(charmSprite, new Vector2Int(Item.XIdx - 1, Item.YIdx)));
            }
            NewItemOwnInstance newItemOwnInstance2 = NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(Item.XIdx + 1), Item.YIdx));
            if (newItemOwnInstance2 == null || !newItemOwnInstance2.Charm || !newItemOwnInstance2.Charm.IsEffectEnabled)
            {
                list.Add(new CharmConnectionData(charmSprite, new Vector2Int(Item.XIdx + 1, Item.YIdx)));
            }

            if (list.Count == 0)
                return null;
            return list.ToArray();
        }

        public override int GetSubIconCount()
        {
            return 0;//2以上にするとエラーが出る
        }

        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            if (!isInstance)
            {
                return null;
            }

            if (!NetworkAvatar)
            {
                return null;
            }

            ItemPosition p = idx == 0 ? new ItemPosition((sbyte)(pos.x - 1), pos.y) : new ItemPosition((sbyte)(pos.x + 1), pos.y);
            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(p);
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled)
            {
                return newItemOwnInstance.Entity.Icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return index == 0 ? new Vector2(-4f, 0) : new Vector2(4f, 0);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
