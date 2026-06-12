using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModCharm : ModItem, IModDamageId
    {
        public static ModCharm Create<T>(string name, int maxLevel, bool isUniqueEffect = false) where T : Charm_Basic
            => new ModCharm().SetCharm<T>(name, maxLevel, isUniqueEffect);
        internal ModCharm SetCharm<T>(string name, int maxLevel, bool isUniqueEffect = false) where T : Charm_Basic
        {
            SetItem(name, EItemType.Charm);

            MaxLevel = maxLevel;
            IsUniqueEffect = isUniqueEffect;
            CharmType = typeof(T);

            return this;
        }
        public int MaxLevel { get; internal set; }
        public LocalizedString[] Effects { get; internal set; } = new LocalizedString[0];
        public Type CharmType { get; internal set; }
        public bool IsUniqueEffect { get; internal set; }
        public EWeaponType RelatedWeapon { get; internal set; } = EWeaponType.SwordAndShield;
        public bool IsWeaponRelatedCharm { get; internal set; } = false;
        public ModDamageId DamageId { get; internal set; }
        public DamageIdEntity DamageIdEntity { get; internal set; }
        public bool HasDamageId => DamageIdEntity != null;
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var charm = o.AddComponent(CharmType) as Charm_Basic;
            //Core.Logger($"CreateCharm");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            o.SetAssetId(AssetId);
            charm.maxLevel = MaxLevel;
            charm.effectsString = Effects;
            charm.isUniqueEffect = IsUniqueEffect;
            charm.isWeaponRelatedCharm = IsWeaponRelatedCharm;
            charm.relatedWeapon = RelatedWeapon;
            charm.enabled = false;
            return o;
        }
        public override void Init(int id, uint assetId)
        {
            base.Init(id, assetId);
            if (DamageId != null)
                DamageIdEntity = DamageId.CreateEntity();
        }
    }
}
