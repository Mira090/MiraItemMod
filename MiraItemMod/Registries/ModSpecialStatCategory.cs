using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModSpecialStatCategory : IDisposable
    {
        public static ModSpecialStatCategory CreateCategory(string name, int order, Color color)
        {
            return new ModSpecialStatCategory().SetCategory(name, name, order, color);
        }
        public static ModSpecialStatCategory CreateCategory(string name, string nameKey, int order, Color color)
        {
            var keyword = new ModSpecialStatCategory().SetCategory(name, nameKey, order, color);
            return keyword;
        }
        internal ModSpecialStatCategory SetCategory(string name, string nameKey, int order, Color color)
        {
            Id = name;
            DisplayName = new LocalizedString(nameKey);
            Order = order;
            HeaderColor = color;
            return this;
        }
        public string Id { get; internal set; }
        public LocalizedString DisplayName { get; internal set; }
        public string FallbackName { get; internal set; } = string.Empty;
        public Color HeaderColor { get; internal set; }
        public string KeywordImageOriginal { get; internal set; } = null;
        public int Order { get; internal set; } = 80;
        public SpecialStatCategoryVisibility Visibility { get; internal set; } = SpecialStatCategoryVisibility.WhenAnyStatHasValue;
        public bool RequireWeapon { get; internal set; } = false;
        public EWeaponType WeaponType { get; internal set; } = EWeaponType.SwordAndShield;
        public SpecialStatCategoryEntity Entity { get; internal set; }


        public void Init()
        {
            Entity = CreateEntity();
        }
        public SpecialStatCategoryEntity CreateEntity()
        {
            var entity = ScriptableObject.CreateInstance<SpecialStatCategoryEntity>();
            entity.name = Id;
            entity.id = Id;
            entity.displayName = DisplayName;
            entity.fallbackName = FallbackName;
            entity.headerColor = HeaderColor;
            entity.order = Order;
            entity.visibility = Visibility;
            entity.requireWeapon = RequireWeapon;
            entity.weaponType = WeaponType;
            return entity;
        }
        public void Dispose()
        {
            if (Entity != null)
                ScriptableObject.Destroy(Entity);
        }
    }
}
