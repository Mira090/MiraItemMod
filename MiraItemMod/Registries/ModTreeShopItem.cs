using HarmonyLib;
using Miniscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraItemMod.Registries
{
    public class ModTreeShopItem : IDisposable
    {
        public static ModTreeShopItem CreateTreeShopItem(string name, int id, int dependency, ELinePos line, params int[] prices)
        {
            return CreateTreeShopItem(name, name, id, dependency, line, prices);
        }
        public static ModTreeShopItem CreateTreeShopItem(string name, string key, int id, int dependency, ELinePos line, params int[] prices)
        {
            return new ModTreeShopItem().SetTreeShopItem(name, key, id, dependency, line, prices);
        }
        internal ModTreeShopItem SetTreeShopItem(string name, string key, int id, int dependency, ELinePos line, params int[] prices)
        {
            Name = id + "_" + name;
            Id = id;
            Dependency = dependency;
            Copy = dependency;
            Behaviour = TreeShopItemEntity.EBehaviour.UnlockItem;
            Group = TreeShopItemEntity.EGroup.Tier1;
            if (!string.IsNullOrEmpty(key))
            {
                LocalizedName = new LocalizedString($"TreeShopItem_{key}_Name");
                LocalizedDescription = new LocalizedString($"TreeShopItem_{key}_Description");
                LocalizedUnlockDescription = new LocalizedString($"TreeShopItem_{key}_UnlockDescription");
            }
            PriceByQuantity = prices;
            MaxQuantity = prices.Length;
            LinePos = line;
            return this;
        }
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public int Dependency { get; internal set; }
        public int Copy { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public LocalizedString LocalizedDescription { get; internal set; }
        public LocalizedString LocalizedUnlockDescription { get; internal set; }
        public TreeShopItemEntity.EGroup Group { get; internal set; }
        public TreeShopItemEntity.EBehaviour Behaviour { get; internal set; }
        public int[] PriceByQuantity { get; internal set; }
        public int MaxQuantity { get; internal set; }
        public Func<Sprite> Icon { get; internal set; }
        public ELinePos LinePos { get; internal set; } = ELinePos.Center;
        public void Init(TreeShopItemEntity copy)
        {
            if (Entity == null)
                Entity = CreateEntity(copy);
        }
        public void SetDependency(TreeShopItemEntity dependency)
        {
            if (!dependency.nextConnections.Contains(Entity))
                dependency.nextConnections = dependency.nextConnections.AddItem(Entity).ToArray();
        }
        public TreeShopItemEntity Entity { get; internal set; }
        public TreeShopItemEntity CreateEntity(TreeShopItemEntity copy)
        {
            var entity = ScriptableObject.CreateInstance<TreeShopItemEntity>();
            entity.name = Name;
            entity.id = Id;
            entity.aName = LocalizedName ?? copy.aName;
            entity.aDescription = LocalizedDescription ?? copy.aDescription;
            entity.anUnlockDescription = LocalizedUnlockDescription ?? copy.anUnlockDescription;
            entity.group = Group;
            entity.behaviour = Behaviour;
            entity.priceByQuantity = PriceByQuantity;
            entity.maxQuantity = MaxQuantity;
            entity.icon = Icon?.Invoke() ?? copy.icon;
            entity.bg = copy.bg;
            return entity;
        }
        public void Dispose()
        {
            if (Entity != null)
                ScriptableObject.Destroy(Entity);
            if (Icon != null)
                Icon = null;
        }
        public enum ELinePos
        {
            Left,
            Right,
            Center,
            LeftUp,
            RightUp,
            CenterUp
        }
    }
}
