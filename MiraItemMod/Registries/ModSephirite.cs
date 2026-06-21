using Mirror;
using MiraItemMod.Sephirites;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModSephirite : IDisposable
    {
        public static ModSephirite Create(string name, Sephirite.Type type = Sephirite.Type.NORMAL)
            => new ModSephirite().SetSephirite<Sephirite_Custom>(name, type);
        public static ModSephirite Create<T>(string name, Sephirite.Type type = Sephirite.Type.NORMAL) where T : Sephirite
            => new ModSephirite().SetSephirite<T>(name, type);
        internal ModSephirite SetSephirite<T>(string name, Sephirite.Type type) where T : Sephirite
        {
            SetItem(name, type);

            SephiriteType = typeof(T);

            return this;
        }
        internal ModSephirite SetItem(string name, Sephirite.Type type)
        {
            Name = name;
            ItemType = type;
            return this;
        }
        public string Name { get; internal set; }
        public GameObject Prefab { get; internal set; }
        public uint AssetId { get; internal set; }
        public Type SephiriteType { get; internal set; }
        public Sephirite.Type ItemType { get; internal set; } = Sephirite.Type.NORMAL;
        public int? AppearLimit { get; internal set; }

        public void Init(uint assetId)
        {
            AssetId = assetId;
            Prefab = CreateResourcePrefab();
        }
        public virtual GameObject CreateResourcePrefab()
        {
            var original = UnityEngine.Object.Instantiate(SephiriaPrefabs.SephiriteLvUp, new Vector3(-1000f, -1000f), Quaternion.identity);
            original.name = "Sephirite_" + Name;
            original.hideFlags = HideFlags.HideAndDontSave;
            original.SetAssetId(AssetId);
            if (!original.TryGetComponent<Sephirite>(out var sephi))
                return null;

            var sephirite = original.AddComponent(SephiriteType) as Sephirite;
            sephirite.interactSoundEvent = sephi.interactSoundEvent;
            sephirite.appearLimit = sephi.appearLimit;
            sephirite.isSkipOpenAnimation = sephi.isSkipOpenAnimation;
            sephirite.minimapElementName = sephi.minimapElementName;
            if (AppearLimit.HasValue)
                sephirite.appearLimit = AppearLimit.Value;

            UnityEngine.Object.Destroy(sephi);
            //sephirite.enabled = false;
            return sephirite.gameObject;
        }
        public void Dispose()
        {
            if (Prefab != null)
                GameObject.Destroy(Prefab);
        }
    }
}
