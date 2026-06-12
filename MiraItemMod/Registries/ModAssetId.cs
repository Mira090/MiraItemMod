using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModAssetId : MonoBehaviour
    {
        public static readonly Dictionary<uint, GameObject> CustomNetworkPrefabs = new Dictionary<uint, GameObject>();
        public uint AssetId
        {
            get => _assetId;
            internal set
            {
                if (CustomNetworkPrefabs.ContainsKey(value))
                    CustomNetworkPrefabs.Remove(value);
                _assetId = value;
                CustomNetworkPrefabs[_assetId] = gameObject;
            }
        }
        [SerializeField]
        private uint _assetId;
        public void ToIdentity()
        {
            var identity = gameObject.AddComponent<NetworkIdentity>();
            identity.SetAssetId(AssetId);

            if (gameObject.TryGetComponent<Charm_Basic>(out var charm))
            {
                charm.enabled = true;
            }
            if (gameObject.TryGetComponent<StoneTablet>(out var tablet))
            {
                tablet.enabled = true;
            }
            if (gameObject.TryGetComponent<ComboEffectBase>(out var combo))
            {
                combo.enabled = true;
            }
            if (gameObject.TryGetComponent<Miracle>(out var miracle))
            {
                miracle.enabled = true;
            }
            if (gameObject.TryGetComponent<PassiveObject>(out var perk))
            {
                perk.enabled = true;
            }
            if (gameObject.TryGetComponent<Sephirite>(out var sephirite))
            {
                sephirite.enabled = true;
            }

            UnityEngine.Object.Destroy(this);
        }
    }
}
