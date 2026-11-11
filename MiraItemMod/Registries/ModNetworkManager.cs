using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

namespace MiraItemMod.Registries
{
    public static class ModNetworkManager
    {
        /// <summary>
        /// ネットワークプレハブの親オブジェクト
        /// </summary>
        /// <remarks>非アクティブのゲームオブジェクトの子オブジェクトにNetworkIdentityを追加すると、Awake()が呼ばれない。</remarks>
        public static GameObject PrefabHolder
        {
            get
            {
                if (_prefabHolder == null)
                {
                    _prefabHolder = new GameObject("MiraItemMod_PrefabHolder");
                    _prefabHolder.SetActive(false);
                    UnityEngine.Object.DontDestroyOnLoad(_prefabHolder);
                }
                return _prefabHolder;
            }
        }
        private static GameObject _prefabHolder;

        public static GameObject SetAssetId(this GameObject gameObject, uint assetId)
        {
            if (gameObject == null)
            {
                Core.LoggerError($"GameObject is null!");
                return gameObject;
            }
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            //Core.Logger(gameObject.name + ": " + assetId);
            if (gameObject.TryGetComponent<NetworkIdentity>(out var identity))
            {
                UnityEngine.Object.DestroyImmediate(identity);
            }
            /*
            if (gameObject.TryGetComponent<ModAssetId>(out var mod))
            {
                Core.LoggerError($"GameObject {gameObject} has already ModAssetId");
                mod.AssetId = assetId;
                return gameObject;
            }*/
            //gameObject.AddComponent<ModAssetId>().AssetId = assetId;
            gameObject.transform.SetParent(PrefabHolder.transform);
            gameObject.AddComponent<NetworkIdentity>();
            return gameObject;
        }



        /*
        [HarmonyPatch(typeof(UnityEngine.Object))]
        public static class ObjectInstantiatePatch
        {
            [HarmonyPatch("Internal_CloneSingle")]
            [HarmonyPostfix]
            static void Postfix0(ref UnityEngine.Object __result, UnityEngine.Object data)
            {
                Patch(ref __result, data);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion) })]
            [HarmonyPostfix]
            static void Postfix1(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                //Core.Logger(original.name + ": Patch");
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion), typeof(Transform) })]
            [HarmonyPostfix]
            static void Postfix2(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object) })]
            [HarmonyPostfix]
            static void Postfix3(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Transform), typeof(bool) })]
            [HarmonyPostfix]
            static void Postfix4(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Scene) })]
            [HarmonyPostfix]
            static void Postfix5(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            static void Patch(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                //Core.Logger(__result + ": " + original);
                if (original == null || __result == null)
                    return;
                //Core.Logger(__result.GetType() + ": " + (__result is GameObject g && g.TryGetComponent<ModAssetId>(out var _)));
                if (__result is GameObject gameObject && gameObject.TryGetComponent<ModAssetId>(out var mod))
                {
                    //Core.Logger(gameObject.name + ": " + mod.AssetId);
                    mod.ToIdentity();
                }
                else if (__result is MonoBehaviour behaviour && behaviour.gameObject.TryGetComponent<ModAssetId>(out var mod2))
                {
                    //Core.Logger(behaviour.name + ": " + mod2.AssetId);
                    mod2.ToIdentity();
                }
            }
        }
        [HarmonyPatch(typeof(NetworkClient), nameof(NetworkClient.GetPrefab))]
        public static class NetworkClientGetPrefabPatch
        {
            static void Postfix(uint assetId, ref GameObject prefab, ref bool __result)
            {
                if (!ModAssetId.CustomNetworkPrefabs.ContainsKey(assetId))
                    return;
                prefab = ModAssetId.CustomNetworkPrefabs[assetId];
                __result = prefab != null;
            }
        }*/
    }
}
