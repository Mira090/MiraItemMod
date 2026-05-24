using SephiriaMod.Entities;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public abstract class ModItem
    {
        internal ModItem SetItem(string name, EItemType type)
        {
            Name = name;
            LocalizedName = new LocalizedString("Item_" + name + "_Name");
            FlavorText = new LocalizedString("Item_" + name + "_FlavorText");
            ItemType = type;
            ItemEntityName = "_" + name;
            ResourcePrefabName = ItemTypeString + "-" + name;
            IconFileName = ModUtil.ItemPath + name;
            return this;
        }
        public ItemEntity ItemEntity { get; internal set; }
        public string Name { get; internal set; }
        public string ItemEntityName { get; internal set; }
        public string ResourcePrefabName { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public LocalizedString FlavorText { get; internal set; }
        public List<string> Categories { get; internal set; } = new List<string>();
        public int Cost { get; internal set; }
        public int Id { get; internal set; }
        public uint AssetId { get; internal set; }
        public EItemActiveType ActiveType { get; internal set; } = EItemActiveType.Default;
        public ItemEntity.EItemBehaviour ItemBehaviour { get; internal set; } = ItemEntity.EItemBehaviour.None;
        public EItemType ItemType { get; internal set; }
        public EItemRarity Rarity { get; internal set; }
        public int SapphirePrice { get; internal set; }
        public string IconFileName { get; internal set; }
        public Sprite Icon { get; internal set; }
        public Sprite IconInWorld { get; internal set; }
        public bool CannotBeReward { get; internal set; } = false;
        public bool CannotThrow { get; internal set; } = false;
        public bool IsDual { get; internal set; } = false;
        public bool IsJewelry { get; internal set; } = false;
        public bool IsExcludedJewelry { get; internal set; } = false;
        public GameObject ResourcePrefab
        {
            get
            {
                if(_resourcePrefab == null)
                    _resourcePrefab = CreateResourcePrefab();
                return _resourcePrefab;
            }
        }
        private GameObject _resourcePrefab;

        public string ItemTypeString => ItemType switch
        {
            EItemType.Charm => "Charm",
            EItemType.StoneTablet => "StoneTablet",
            EItemType.Potion => "Potion",
            EItemType.Food => "Food",
            EItemType.Scroll => "Scroll",
            EItemType.Identifiable => "Identifiable",
            EItemType.ThrowingWeapon => "ThrowingWeapon",
            _ => "Misc",
        };
        public abstract GameObject CreateResourcePrefab();
        public virtual void Init(int id, uint assetId)
        {
            _resourcePrefab = CreateResourcePrefab();
            Id = id;
            AssetId = assetId;
            ItemEntity = CreateItemEntity();
        }
        public virtual void LateInit()
        {

        }
        public ItemEntity CreateItemEntity()
        {
            var entity = IsJewelry ? ScriptableObject.CreateInstance<ItemEntity_Jewelry>() : ScriptableObject.CreateInstance<ItemEntity>();
            entity.name = Id + ItemEntityName;
            entity.activeType = ActiveType;
            entity.aName = LocalizedName;
            entity.aFlavorText = FlavorText;
            entity.categories = Categories;
            entity.cost = Cost;
            entity.id = Id;
            entity.itemBehaviour = ItemBehaviour;
            entity.rarity = Rarity;
            entity.type = ItemType;
            entity.sapphirePrice = SapphirePrice;
            entity.resourcePrefab = ResourcePrefab;
            entity.icon = Icon ?? AssetLoader.LoadSprite(IconFileName);
            entity.iconInWorld = IconInWorld ?? ItemType switch
            {
                EItemType.Charm => Data.IconInWorldCharm,
                EItemType.StoneTablet => Data.IconInWorldTablet,
                _ => Data.IconInWorldPotion,
            };
            entity.cannotBeReward = CannotBeReward;
            entity.cannotThrow = CannotThrow;
            entity.isDual = IsDual;
            return entity;
        }
    }
}
