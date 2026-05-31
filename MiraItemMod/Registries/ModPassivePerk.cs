using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModPassivePerk : IModDamageId, IDisposable
    {
        public static ModPassivePerk CreatePassivePerk(ModPassive parent, string name, EPassivePerkLv lv)
        {
            return new ModPassivePerk().SetPassivePerk(parent, name, lv);
        }
        internal ModPassivePerk SetPassivePerk(ModPassive parent, string name, EPassivePerkLv lv)
        {
            Parent = parent;
            Name = name;
            EffectString = new LocalizedString($"Passive_{parent.Name}_Effect_{lv.ToUpperString()}");
            IconPath = ModUtil.PassivePath + parent.Name + "_Perk_" + lv.ToUpperString();
            return this;
        }
        public ModPassive Parent { get; private set; }
        public string Name { get; internal set; }
        public EPassivePerkLv PerkLv { get; internal set; }
        public uint AssetId { get; internal set; }
        public LocalizedString EffectString { get; internal set; }
        public string IconPath { get; internal set; }
        public Func<GameObject, PassiveObject> PerkSupplier { get; internal set; }

        public GameObject PerkPrefab { get; internal set; }

        public ModDamageId DamageId { get; internal set; }
        public DamageIdEntity DamageIdEntity { get; internal set; }
        public bool HasDamageId => DamageIdEntity != null;

        public void Init(uint assetId)
        {
            PerkPrefab = CreateResourcePrefab();
            AssetId = assetId;
            if (DamageId != null)
                DamageIdEntity = DamageId.CreateEntity();
        }
        public GameObject CreateResourcePrefab()
        {
            var o = new GameObject($"{Parent.Id}_{PerkLv.ToUpperString()}_{Name}");
            var meta = o.AddComponent<PassiveObjectMetadata>();
            meta.effectString = EffectString;
            meta.icon = AssetLoader.LoadSprite(IconPath);
            o.AddComponent<LogComponent>();
            //Core.Logger($"CreatePassivePerk");
            o.hideFlags = HideFlags.HideAndDontSave;
            if (PerkSupplier != null)
            {
                var effect = PerkSupplier.Invoke(o);
                effect.enabled = false;
            }
            else
            {
                Core.LoggerWarning($"{Parent.Name} Perk {Name}: PerkSupplier is null");
            }
            return o;
        }
        public void Dispose()
        {
            if (PerkPrefab != null)
                GameObject.Destroy(PerkPrefab);
            if (DamageIdEntity != null)
                ScriptableObject.Destroy(DamageIdEntity);
        }
    }
}
