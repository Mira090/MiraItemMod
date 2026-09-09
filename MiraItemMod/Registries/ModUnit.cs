using HarmonyLib;
using MiraItemMod.StatusInstances;
using MiraItemMod.Units;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModUnit
    {
        public static ModUnit Create(string name, Func<GameObject> originalSupplier)
        {
            var unit = new ModUnit
            {
                Name = name,
                OriginalSupplier = originalSupplier
            };
            return unit;
        }

        public string Name { get; internal set; }
        public AnimationSet AnimationSet { get; internal set; }
        public UnitAnimationMetadata AnimationMetadata { get; internal set; }
        public GameObject Prefab { get; internal set; }
        public uint AssetId { get; internal set; }
        public Func<GameObject> OriginalSupplier { get; internal set; }

        public static GameObject GetOriginalBySummonCharm(int id)
        {
            var entity = ItemDatabase.FindItemById(id);
            if (entity == null)
                return null;
            if(entity.resourcePrefab == null)
                return null;
            if(entity.resourcePrefab.TryGetComponent<Charm_SummonUnit>(out var summon))
            {
                return summon.unitPrefab;
            }
            else if(entity.resourcePrefab.TryGetComponent<Charm_MiniBallista>(out var ballista))
            {
                return ballista.unitPrefab;
            }
            return null;
        }
        public virtual void InitPrefab()
        {
            var original = OriginalSupplier?.Invoke();
            if(original == null)
            {
                Core.LoggerError($"Original prefab for Unit {Name} is null");
                return;
            }

            Prefab = UnityEngine.Object.Instantiate(original);
            Prefab.name = "Unit_" + Name;
            Prefab.SetAssetId(AssetId);
            if(Prefab.TryGetComponent<TopdownActorRenderingMetadata>(out var actor) && AnimationSet != null)
            {
                actor.animator.currentSet = AnimationSet;
            }
            if(Prefab.TryGetComponent<UnitAvatar>(out var avatar))
            {
                var face = AssetLoader.LoadSprite(ModUtil.UnitPath + Name + "\\FaceChip");
                avatar.faceChipSprite = face;
                avatar.miniFaceChipSprite = face;
                avatar.enabled = false;
            }
            if(Prefab.TryGetComponent<UnitAI_NewBasic>(out var ai))
            {
                ai.enabled = false;

                if (Prefab.transform.Find("Hitbox") is Transform hitbox)
                {
                    hitbox.gameObject.SetActive(false);
                }
            }
        }
        public virtual void Init(uint assetId)
        {
            AssetId = assetId;
            AnimationMetadata = UnitAnimationLoader.LoadMetadata(this);
            if(AnimationMetadata != null)
            {
                AnimationSet = AnimationMetadata.CreateAnimationSet(Name);
            }
        }


        [HarmonyPatch(typeof(TopdownRigidbody), "OnEnable")]
        public static class TopdownRigidbodyOnEnablePatch
        {
            static bool Prefix(TopdownRigidbody __instance)
            {
                if (ReflectionExtensions.GetCachedRigidbodies() == null)
                {
                    Core.LoggerFew("Blocked TopdownRigidbody.OnEnable due to null cachedRigidbodies");
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(TopdownRigidbody), "OnDisable")]
        public static class TopdownRigidbodyOnDisablePatch
        {
            static bool Prefix(TopdownRigidbody __instance)
            {
                if (ReflectionExtensions.GetCachedRigidbodies() == null)
                {
                    Core.LoggerFew("Blocked TopdownRigidbody.OnDisable due to null cachedRigidbodies");
                    return false;
                }
                return true;
            }
        }
    }
}
