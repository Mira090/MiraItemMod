using FMODUnity;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ItemEntity;
using static Miracle;

namespace MiraItemMod.Registries
{
    public class ModComboEffect : IModDamageId , IDisposable
    {
        public static ModComboEffect Create(string name, bool requireStartUpItem = false)
            => new ModComboEffect().SetCombo<ComboEffectBase>(name, requireStartUpItem);
        public static ModComboEffect Create<T>(string name, bool requireStartUpItem = false) where T : ComboEffectBase
            => new ModComboEffect().SetCombo<T>(name, requireStartUpItem);
        internal ModComboEffect SetCombo<T>(string name, bool requireStartUpItem = false) where T : ComboEffectBase
        {
            SetItem(name);

            RequireStartUpItem = requireStartUpItem;
            ComboEffectType = typeof(T);

            return this;
        }

        internal ModComboEffect SetItem(string name)
        {
            Name = name;
            CategoryName = new LocalizedString("ItemCategory_" + name);
            ResourcePrefabName = "ItemCategory" + "-" + name;
            FruitName = new LocalizedString("ItemCategory_" + name + "_Fruit");
            IconFileName = ModUtil.ItemCategoryPath + name;
            DefaultEffect = new LocalizedString();
            Id = name.ToSephiriaUpperId();
            IconFruitFileName = ModUtil.ItemCategoryPath + name + "_Fruit";
            IconFruitIceFileName = ModUtil.ItemCategoryPath + name + "_FruitIce";
            return this;
        }
        public string Name { get; internal set; }
        public ItemCategoryEntity ItemCategoryEntity { get; internal set; }
        public LocalizedString CategoryName { get; internal set; }
        public LocalizedString FruitName { get; internal set; }
        public LocalizedString DefaultEffect { get; internal set; }
        public string ResourcePrefabName { get; internal set; }
        public string Id { get; internal set; }
        public uint AssetId { get; internal set; }
        public string IconFileName { get; internal set; }
        public Sprite Icon { get; internal set; }
        public string IconFruitFileName { get; internal set; }
        public Sprite IconFruit { get; internal set; }
        public string IconFruitIceFileName { get; internal set; }
        public Sprite IconFruitIce { get; internal set; }
        public Type ComboEffectType { get; internal set; }
        public bool RequireStartUpItem { get; internal set; }
        public EventReference EnableSound { get; internal set; }
        public ComboEffectBase.ComboStat[] Stats { get; internal set; }
        public GameObject ResourcePrefab
        {
            get
            {
                if (_resourcePrefab == null)
                    _resourcePrefab = CreateResourcePrefab();
                return _resourcePrefab;
            }
        }
        private GameObject _resourcePrefab;
        public ModDamageId DamageId { get; internal set; }
        public DamageIdEntity DamageIdEntity { get; internal set; }
        public bool HasDamageId => DamageIdEntity != null;
        public void Init(uint assetId)
        {
            AssetId = assetId;
            _resourcePrefab = CreateResourcePrefab();
            ItemCategoryEntity = CreateItemCategoryEntity();
            if (DamageId != null)
                DamageIdEntity = DamageId.CreateEntity();
        }
        public ItemCategoryEntity CreateItemCategoryEntity()
        {
            var entity = ScriptableObject.CreateInstance<ItemCategoryEntity>();
            entity.name = Name;
            entity.categoryName = CategoryName;
            entity.id = Id;
            entity.categoryIcon = Icon ?? AssetLoader.LoadSprite(IconFileName);
            entity.comboEffectPrefab = ResourcePrefab;
            entity.requireStartUpItem = RequireStartUpItem;
            entity.enableSound = EnableSound.IsNull ? Data.DefaultEnableSound : EnableSound;
            entity.categoryFruitName = FruitName;
            entity.categoryFruitIcon = IconFruit ?? AssetLoader.LoadSprite(IconFruitFileName);
            entity.categoryIceFruitIcon = IconFruitIce ?? AssetLoader.LoadSprite(IconFruitIceFileName);
            return entity;
        }
        public GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var combo = o.AddComponent(ComboEffectType) as ComboEffectBase;
            //Core.Logger($"CreateComboEffect");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            o.SetAssetId(AssetId);
            combo.addStatByCombo = Stats;
            combo.defaultEffect = DefaultEffect;
            combo.enabled = false;
            return o;
        }
        public void Dispose()
        {
            if (ItemCategoryEntity != null)
                ScriptableObject.Destroy(ItemCategoryEntity);
            if (_resourcePrefab != null)
                GameObject.Destroy(_resourcePrefab);
            Icon = null;
            IconFruit = null;
            IconFruitIce = null;
        }
    }
}
