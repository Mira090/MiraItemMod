using FMOD;
using HarmonyLib;
using MiraItemMod.Items;
using MiraItemMod.Items.Pallas;
using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using MiraItemMod.Weapons;
using Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Type = System.Type;


namespace MiraItemMod
{
    public class Core : HorayModBase
    {
        public static Core Instance { get; private set; }
        public static GameObject ModSingletonObject { get; private set; }
        public static void LoggerFew(string message)
        {
            if (Core.LogFew)
                Debug.Log("[MiraItemMod] " + message);
        }
        public static void LoggerMedium(string message)
        {
            if (Core.LogMedium)
                Debug.Log("[MiraItemMod] " + message);
        }
        public static void LoggerMany(string message)
        {
            if (Core.LogMany)
                Debug.Log("[MiraItemMod] " + message);
        }
        public static void Logger(string message)
        {
            Debug.Log("[MiraItemMod] " + message);
        }
        public static void LoggerWarning(string message)
        {
            Debug.LogWarning("[MiraItemMod] " + message);
        }
        public static void LoggerWarning(System.Exception message)
        {
            Debug.LogWarning("[MiraItemMod] " + message);
        }
        public static void LoggerError(string message)
        {
            Debug.LogError("[MiraItemMod] " + message);
        }
        public static void LoggerError(System.Exception message)
        {
            Debug.LogError("[MiraItemMod] " + message);
        }

        public List<List<string>> ModOptions => new List<List<string>>() { new List<string>() { "No Log", "Few", "Medium", "Many" } };
        public List<string> ModOptionsDescription => new List<string>() { "Log Mode" };
        public List<int> ModOptionsDefault => new List<int>() { 1 };

        public static int LogMode = 1;
        public static bool LogFew => LogMode >= 1;
        public static bool LogMedium => LogMode >= 2;
        public static bool LogMany => LogMode >= 3;

        public void OnModOptionChanged(int index, int value)
        {
            if (index == 0)
                LogMode = value;
            Core.Logger("Changed: " + ModOptions[index][value]);
        }
        public void OnModOptionLoaded(int index, int value)
        {
            if (index == 0)
                LogMode = value;
            Core.Logger("Loaded: " + ModOptions[index][value]);
        }
        public static Harmony ModPatches { get; private set; }
        public static bool IsInitialized { get; private set; } = false;
        protected override void OnModLoaded()
        {
            base.OnModLoaded();
            if (!IsInitialized)
            {
                IsInitialized = true;
                Instance = this;

                ModPatches = new Harmony("com.Mira.MiraItemMod");
                ModPatches.PatchAll();

                CustomSpriteAsset.InitSprites();
                Data.Init();

                CustomSpriteAsset.InitSpriteAsset();

                HorayModAPI.OnLoadItemDatabase += OnLoadItemDatabase;
                HorayModAPI.OnLoadMiracleDatabase += OnLoadMiracleDatabase;
                HorayModAPI.OnLoadStatusDatabase += OnLoadStatusDatabase;
                HorayModAPI.OnLoadKeywordDatabase += OnLoadKeywordDatabase;
                HorayModAPI.OnAllDatabasesReady += OnAllDatabasesReady;
                HorayModAPI.OnLocalizationReady += OnLocalizationReady;

                HorayModAPI.OnLoadWeaponDatabase += OnLoadWeaponDatabase;

                ModCompat.LoadCompats();
            }

            if (ModSingletonObject != null)
            {
                UnityEngine.Object.Destroy(ModSingletonObject);
            }
            if (ScreenFader.Instance.IsTestMode)
            {
                ModSingletonObject = new GameObject();
                ModSingletonObject.AddComponent<ModSingletonBehavior>();
            }
        }

        protected override void OnModUnloaded()
        {
            IsInitialized = false;

            ModCompat.UnloadCompats();

            if (ModSingletonObject != null)
            {
                UnityEngine.Object.Destroy(ModSingletonObject);
            }

            HorayModAPI.OnLoadItemDatabase -= OnLoadItemDatabase;
            HorayModAPI.OnLoadMiracleDatabase -= OnLoadMiracleDatabase;
            HorayModAPI.OnLoadStatusDatabase -= OnLoadStatusDatabase;
            HorayModAPI.OnLoadKeywordDatabase -= OnLoadKeywordDatabase;
            HorayModAPI.OnAllDatabasesReady -= OnAllDatabasesReady;
            HorayModAPI.OnLocalizationReady -= OnLocalizationReady;

            HorayModAPI.OnLoadWeaponDatabase -= OnLoadWeaponDatabase;

            //Data.Dispose();
            WeaponAddonKatana_Plasma.Dispose();
            Events.UIStatsPanelPatch.Patched = false;

            if (ModPatches != null)
            {
                ModPatches.UnpatchSelf();
            }
            base.OnModUnloaded();
        }

        private void OnLoadWeaponDatabase()
        {
            Data.RegisterWeapons();
        }

        private void OnLoadItemDatabase()
        {
            Data.RegisterItems();
            ItemDatabase.Modify(3022, item =>//メテオシャワー
            {
                var prefab = item.resourcePrefab;
                if (prefab.TryGetComponent<Charm_Magic>(out var charm))
                {
                    if (charm.skill.magicPrefab.TryGetComponent<ActiveSkill_MeteorShower>(out var skill))
                    {
                        //skill.meteorFireTimer = new Timer(0.25f);
                        //skill.numberOfMeteorsByLevel = new int[3] { 24, 30, 36 };
                        skill.meteorFireTimer = new Timer(0.05f);
                        skill.numberOfMeteorsByLevel = new int[9] { 20, 25, 30, 35, 40, 45, 50, 60, 100 };//3 Level => DPS 1750（number100, Fire 51）
                        skill.damagePercentByLevel = new float[6] { 80f, 90f, 100f, 110f, 120f, 130f };
                        charm.skill.cooldownTime = 31f;//26 => ??
                        charm.skill.mpCostsByLevel = new int[9] { 23, 33, 37, 46, 51, 57, 66, 82, 102 };//35, 38, 41, 44
                        charm.maxLevel = 8;
                    }
                }
            });
            ItemDatabase.Modify(1123, SetItemCategories(ItemCategories.Lake, ItemCategories.Vitality));//スタールビー
            ItemDatabase.Modify(1124, SetItemCategories(ItemCategories.Lake, ItemCategories.Vitality));//スターアクアマリン
            ItemDatabase.Modify(1196, SetItemCategories(ItemCategories.Vitality));//生命の手
            ItemDatabase.Modify(1158, SetItemCategories(ItemCategories.Vitality));//強化ポーションキャップ
            ItemDatabase.Modify(1005, SetItemCategories(ItemCategories.Vitality));//ハート形のニンジン
            ItemDatabase.Modify(1017, SetItemCategories(ItemCategories.Vitality));//盾のイヤリング
            ItemDatabase.Modify(1165, SetIsNotUniqueEffect());//ライリーの懐中時計
            ItemDatabase.Modify(1166, SetIsNotUniqueEffect());//宝石の鎧
            ItemDatabase.Modify(1169, SetIsNotUniqueEffect());//戦闘魔法使いの手袋
            ItemDatabase.Modify(1075, SetIsNotUniqueEffect());//空の剣の握り
            ItemDatabase.Modify(1235, SetItemCategories(ItemCategories.Sturdy, ItemCategories.SkySong));//突き指南書
            ItemDatabase.Modify(1149, SetItemCategories(ItemCategories.WindSong, ItemCategories.SkySong));//金色のマント
            ItemDatabase.Modify(1082, SetItemCategories(ItemCategories.SkySong));//圧迫マント
            ItemDatabase.Modify(1011, SetItemCategories(ItemCategories.SkySong));//風草のスカーフ
            ItemDatabase.Modify(1093, SetItemCategories(ItemCategories.SkySong));//いばらの茂み
            ItemDatabase.Modify(1172, SetItemCategories(ItemCategories.Fortune));//パラスのカード
            ItemDatabase.Modify(1188, item =>//血石のイヤリング
            {
                item.categories = new List<string> { ItemCategories.Drunk, ItemCategories.Vitality };
                item.SetEntityRarity(EItemRarity.Rare);
                item.isDual = true;

                if (item.resourcePrefab.TryGetComponent<Charm_StatusInstance>(out var status) && status.stats.Length >= 2 && status.stats[1].statusID == "DEFENSE")
                {
                    status.stats[1].valuesByLevel = new int[] { -10, -15, -20, -30 };
                }
            });
            ItemDatabase.Modify(1120, SetItemCategories(ItemCategories.Vitality));//血石の指輪
            ItemDatabase.Modify(1174, item =>
            {
                item.categories = new List<string> { ItemCategories.Vitality };
                item.activeType = EItemActiveType.Default;
                if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_ElruNaptimePillow>(out var charm))
                {
                    var mod = item.resourcePrefab.AddComponent<Charm_ElruNaptimePillowMkII>();
                    mod.healByLevel = charm.healByLevel;
                    mod.healFxPrefab = charm.healFxPrefab;
                    mod.healSound = charm.healSound;
                    mod.effectsString = charm.effectsString;
                    mod.maxLevel = charm.maxLevel;
                    mod.isUniqueEffect = charm.isUniqueEffect;
                    UnityEngine.Object.Destroy(charm);
                }
            });
            ItemDatabase.Modify(1148, item =>
            {
                item.categories = new List<string> { ItemCategories.Vitality };
                item.activeType = EItemActiveType.Default;
                if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_FairyJar>(out var charm))
                {
                    charm.orbCreateChanceByLevel = new float[] { 2, 4, 7, 10, 15, 20 };
                    charm.maxLevel = 5;
                }
            });
            ItemDatabase.Modify(1172, item =>//パラスのカード
            {
                if (item.resourcePrefab == null)
                    return;
                if(item.resourcePrefab.TryGetComponent<Charm_PallasCard>(out var charm))
                    charm.gameObject.AddComponent<CustomPallasController>().Set(charm);
            });
            ItemDatabase.Modify(Data.PallasAce.Id, item =>//パラスのエース
            {
                if (item.resourcePrefab == null)
                    return;
                if (item.resourcePrefab.TryGetComponent<Charm_PallasAce>(out var charm))
                    charm.gameObject.AddComponent<CustomPallasController>().Set(charm);
            });
        }
        private void OnLoadMiracleDatabase()
        {
            MiracleDatabase.Modify("Scholar", SetMiracleCategories(ItemCategories.Lake));
            MiracleDatabase.Modify("IntelligenceAgent", SetMiracleCategories(ItemCategories.SkySong));
            MiracleDatabase.Modify("Guard", SetMiracleCategories(ItemCategories.Vitality));
            MiracleDatabase.Modify("Berserker", SetMiracleCategories(ItemCategories.Drunk));
            MiracleDatabase.Modify("Elementalist", SetMiracleCategories(ItemCategories.Elemental));
        }
        private void OnLoadStatusDatabase()
        {
            StatusDatabase.Modify("HP_POTION_BONUS", status =>
            {
                status.divideForDisplay = 1;
            });
        }
        private void OnLoadKeywordDatabase()
        {
            KeywordDatabase.Modify("Toughness", keyword =>
            {
                keyword.displayDetails = true;
            });
        }
        private void OnAllDatabasesReady()
        {
            Data.LoadMiracleManuallyGivenItems();
            Data.LoadTreeShopItems();
        }
        private void OnLocalizationReady(HorayModLocalizationContext context)
        {
            AssetLoader.LoadLocalization(context);

            foreach (var moditem in Data.Weapons)
            {
                moditem.OnSpriteFxRegistered();
            }
        }

        private static Action<ItemEntity> SetItemCategories(params string[] categories)
        {
            return item => item.categories = categories.ToList();
        }
        private static Action<Miracle> SetMiracleCategories(params string[] categories)
        {
            return item => item.categories = categories;
        }
        private static Action<ItemEntity> SetIsNotUniqueEffect()
        {
            return item =>
            {
                var prefab = item.resourcePrefab;
                if (prefab != null && prefab.TryGetComponent<Charm_Basic>(out var charm))
                {
                    charm.isUniqueEffect = false;
                }
            };
        }

        #region アイテム追加関連パッチ
        
        [HarmonyPatch(typeof(TreeShopItemStorage), nameof(TreeShopItemStorage.Load))]
        public static class TreeShopItemStorageLoadPatch
        {
            static void Postfix(TreeShopItemStorage __instance)
            {
                //Core.LoggerFew("unlockedCharms");
                var player = __instance.GetComponent<PlayerSpawner>();
                var list = player.unlockedCharms.Distinct().ToList();
                player.unlockedCharms.Clear();
                player.unlockedCharms.AddRange(list);
                //foreach (var item in player.unlockedCharms.OrderBy(x => x))
                {
                    //Core.LoggerFew("Id: " + item);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerSpawner), nameof(PlayerSpawner.OnStartServer))]
        public static class PlayerSpawnerOnStartServerPatch
        {
            static void Postfix(PlayerSpawner __instance)
            {
                foreach (ModItem item in Data.All)
                {
                    ItemEntity itemEntity = item.ItemEntity;
                    if(itemEntity == null)
                    {
                        Core.LoggerError($"ItemEntity is null for item: {item.Name} (ID: {item.Id})");
                        continue;
                    }
                    if (itemEntity.activeType == EItemActiveType.Default)
                    {
                        if (itemEntity.type == EItemType.Charm && !__instance.unlockedCharms.Contains(itemEntity.id))
                        {
                            __instance.unlockedCharms.Add(itemEntity.id);
                        }
                        else if (itemEntity.type == EItemType.StoneTablet && !__instance.unlockedStoneTablets.Contains(itemEntity.id))
                        {
                            __instance.unlockedStoneTablets.Add(itemEntity.id);
                        }
                        else if (itemEntity.type == EItemType.Potion && !__instance.unlockedPotions.Contains(itemEntity.id))
                        {
                            __instance.unlockedPotions.Add(itemEntity.id);
                        }
                    }
                    else if (itemEntity.activeType == EItemActiveType.TestOnly && (bool)ScreenFader.Instance && ScreenFader.Instance.IsTestMode)
                    {
                        if (itemEntity.type == EItemType.Charm && !__instance.unlockedCharms.Contains(itemEntity.id))
                        {
                            __instance.unlockedCharms.Add(itemEntity.id);
                        }
                        else if (itemEntity.type == EItemType.StoneTablet && !__instance.unlockedStoneTablets.Contains(itemEntity.id))
                        {
                            __instance.unlockedStoneTablets.Add(itemEntity.id);
                        }
                        else if (itemEntity.type == EItemType.Potion && !__instance.unlockedPotions.Contains(itemEntity.id))
                        {
                            __instance.unlockedPotions.Add(itemEntity.id);
                        }
                    }
                }
            }
        }
        #endregion

        [HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), new Type[] { typeof(string), typeof(Type) })]
        public static class ResourcesLoadAllPatch
        {
            private static void ModifyItemEntity(ItemEntity item)
            {

            }
            private static void ModifyItemCategoryEntity(ItemCategoryEntity category)
            {
                if(category.id == ItemCategories.Savvy)
                {
                    try
                    {
                        if (category.comboEffectPrefab != null && category.comboEffectPrefab.TryGetComponent<ComboEffectBase>(out var combo))
                        {
                            combo.addStatByCombo = new ComboEffectBase.ComboStat[]
                            {
                                combo.addStatByCombo[0],
                                combo.addStatByCombo[1],
                                new ComboEffectBase.ComboStat()
                                { 
                                    comboCount = 6,
                                    status = new string[]{ "ADDITIONAL_MONEY/500", "LEAF_DROP/35"},
                                },
                                new ComboEffectBase.ComboStat()
                                {
                                    comboCount = 8,
                                    status = new string[]{"ADDITIONAL_MONEY/1000", "LEAF_DROP/50"},
                                },
                                new ComboEffectBase.ComboStat()
                                {
                                    comboCount = 10,
                                    status = new string[]{ "NEGOTIATION/10", "ADDITIONAL_SHOP/2"},
                                }
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.LoggerError($"Error in ModifyItemCategoryEntity for category {category.id}: {ex}");
                    }
                }
            }
            private static void ModifyStatusEntity(StatusEntity status)
            {

            }
            private static void ModifyMiracle(Miracle miracle)
            {

            }
            private static void ModifyKeywordEntity(KeywordEntity keyword)
            {

            }
            private static void ModifyTreeShopItemEntity(TreeShopItemEntity entity)
            {

            }
            static void Postfix(string path, Type systemTypeInstance, ref UnityEngine.Object[] __result)
            {
                //Core.Logger("Postfix: (" + systemTypeInstance.ToString() + ") " + path);
                if (systemTypeInstance == typeof(ItemEntity) && path == "Item")
                {
                    var list = __result.ToList();

                    //Data.Register(list);

                    foreach (var item in list)
                    {
                        if (item is ItemEntity entity)
                            ModifyItemEntity(entity);
                    }

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(ItemCategoryEntity) && path == "ItemCategory")
                {
                    var list = __result.ToList();


                    foreach (var item in list)
                    {
                        if (item is ItemCategoryEntity entity)
                        {
                            if (Data.DefaultEnableSound.IsNull)
                            {
                                Data.DefaultEnableSound = entity.enableSound;
                            }
                        }
                    }

                    Data.RegisterCombos(list);

                    foreach (var item in list)
                        if (item is ItemCategoryEntity entity)
                            ModifyItemCategoryEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(EffectHUDEntity) && path == "EffectHUD")
                {
                    var list = __result.ToList();

                    Data.RegisterEffectHUDs(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(DamageIdEntity) && path == "DamageId")
                {
                    //一回
                    var list = __result.ToList();

                    Data.RegisterDamageIds(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(StatusEntity) && path == "Status")
                {
                    //複数回
                    var list = __result.ToList();

                    Data.RegisterStatuses(list);

                    //Core.Logger(list.ToAllString());

                    foreach (var item in list)
                        if (item is StatusEntity entity)
                            ModifyStatusEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(KeywordEntity) && path == "Keyword")
                {
                    var list = __result.ToList();

                    Data.RegisterKeywords(list);

                    foreach (var item in list)
                        if (item is KeywordEntity entity)
                            ModifyKeywordEntity(entity);

                    __result = list.ToArray();
                }
                /*
                if(systemTypeInstance == typeof(CostumeEntity) && path == "Costume")
                {
                    var list = __result.ToList();

                    Data.RegisterCostume(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(CostumeSkinEntity) && path == "CostumeSkin")
                {
                    var list = __result.ToList();

                    Data.RegisterCostumeSkin(list);

                    __result = list.ToArray();
                }*/
                if (systemTypeInstance == typeof(GameObject) && path == "Miracle")
                {
                    var list = __result.ToList();

                    Data.RegisterMiracles(list);

                    foreach (var item in list)
                        if (item is GameObject entity && entity.TryGetComponent<Miracle>(out var miracle))
                            ModifyMiracle(miracle);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(WeaponEntity) && path == "Weapon")
                {
                    var list = __result.ToList();

                    //Data.RegisterWeapons(list);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(PassiveEntity) && path == "Passive")
                {
                    var list = __result.ToList();

                    Data.RegisterPassives(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(CharacterBuff) && path == "Buff")
                {
                    var list = __result.ToList();

                    Data.RegisterBuffs(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(CharacterDebuff) && path == "Debuff")
                {
                    var list = __result.ToList();

                    Data.RegisterDebuffs(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(TreeShopItemEntity) && path == "TreeShopItem")
                {
                    var list = __result.ToList();

                    //Core.Logger("TreeShopItems: " + list.Count);
                    //foreach(var item in list)
                    {
                        //Core.Logger(item.ToAllString());
                    }

                    Data.RegisterTreeShops(list);

                    //list.AddRange(addTreeShop);
                    __result = list.ToArray();
                }

            }
        }

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
        }

        /*
        /// <summary>
        /// アイテムのInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SetItem))]
        public static class InstantiateStoneTabletPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiateStoneTabletPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateStoneTabletPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Core.Logger($"CustomInstantiate: {original.name}");
                foreach (var modItem in Data.All)
                {
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if(ob.TryGetComponent<Charm_Basic>(out var charm))
                        {
                            charm.enabled = true;
                        }
                        if (ob.TryGetComponent<StoneTablet>(out var tablet))
                        {
                            tablet.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// コンボ効果（カテゴリー）のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SearchSetEffectInInventory))]
        public static class InstantiateItemCategoryPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiateItemCategoryPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateItemCategoryPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Core.Logger($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Combos)
                {
                    //Core.Logger($"A: {modItem.ResourcePrefab.name}");
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for Combo: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<ComboEffectBase>(out var combo))
                        {
                            combo.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// 奇跡のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(MiracleController), "LocalAddMiracle")]
        public static class InstantiateMiraclePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiateMiraclePatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateMiraclePatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static Miracle CustomInstantiate(Miracle original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Core.Logger($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Miracles)
                {
                    //Core.Logger($"A: {modItem.ResourcePrefab.name}");
                    if (original.gameObject.name == modItem.Prefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for Miracle: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// 武器のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(WeaponControllerSimple), "LocalEquipWeapon")]
        public static class InstantiateWeaponPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiateWeaponPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateWeaponPatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Core.Logger($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Weapons)
                {
                    //Core.Logger($"A: {modItem.ResourcePrefab.name}");
                    if (original.name == modItem.MainWeaponPrefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for Weapon: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        /// <summary>
        /// 才能のInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(PlayerAvatar), "AddPassiveStatOnServer")]
        public static class InstantiatePassivePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiatePassivePatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject) }
                );

                var replacement = AccessTools.Method(typeof(InstantiatePassivePatch), nameof(CustomInstantiate));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        code.operand = replacement;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original)
            {
                if (original == null)
                    return UnityEngine.Object.Instantiate(original);

                //Core.Logger($"CustomInstantiate2: {original.name}");
                foreach (var modItem in Data.Passives)
                {
                    //Core.Logger($"A: {modItem.ResourcePrefab.name}");
                    if (original.name == modItem.Lv5Perk.PerkPrefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for lv5: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv5Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                    else if (original.name == modItem.Lv10Perk.PerkPrefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for lv5: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv10Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                    else if (original.name == modItem.Lv20Perk.PerkPrefab.name)
                    {
                        Core.LoggerMedium($"Bypassing Instantiate for lv5: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original);
                        var identity = ob.gameObject.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv20Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original);
            }
        }
        [HarmonyPatch(typeof(NetworkClient), "SpawnPrefab")]
        public static class InstantiateNetworkClientPatch
        {
            public static event Func<uint, GameObject> OnGetPrefab;
            public static event Func<GameObject, Vector3, Quaternion, GameObject> OnInstantiate;
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Core.LoggerFew($"InstantiateNetworkClientPatch Transpiler");
                var target = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.Instantiate),
                    new[] { typeof(GameObject), typeof(Vector3), typeof(Quaternion) }
                );

                var replacement = AccessTools.Method(typeof(InstantiateNetworkClientPatch), nameof(CustomInstantiate));
                var replacement2 = AccessTools.Method(typeof(InstantiateNetworkClientPatch), nameof(CustomGetPrefab));

                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi && mi.Name == nameof(UnityEngine.Object.Instantiate))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        Core.LoggerFew($"Transpiler SpawnPrefab Instantiate");
                        code.operand = replacement;
                    }
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi2 && mi2.Name == nameof(NetworkClient.GetPrefab))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        Core.LoggerFew($"Transpiler SpawnPrefab GetPrefab");
                        code.operand = replacement2;
                    }
                    yield return code;
                }
            }

            // 差し替え用メソッド
            public static GameObject CustomInstantiate(GameObject original, Vector3 position, Quaternion rotation)
            {
                if(original == null)
                    return UnityEngine.Object.Instantiate(original, position, rotation);

                var o = OnInstantiate?.Invoke(original, position, rotation);
                if (o != null)
                {
                    //Core.Logger($"CustomInstantiate: {o.name}");
                    return o;
                }
                //Core.Logger($"CustomInstantiate: {original.name}");
                foreach(var modItem in Data.All)
                {
                    if(original.name == modItem.ResourcePrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<Charm_Basic>(out var charm))
                        {
                            charm.enabled = true;
                        }
                        if (ob.TryGetComponent<StoneTablet>(out var tablet))
                        {
                            tablet.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Combos)
                {
                    if (original.name == modItem.ResourcePrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<ComboEffectBase>(out var combo))
                        {
                            combo.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Miracles)
                {
                    if (original.name == modItem.Prefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<Miracle>(out var miracle))
                        {
                            miracle.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Weapons)
                {
                    if (modItem.WeaponEntity == null)
                        continue;
                    if (original.name == modItem.MainWeaponPrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        //if (ob.TryGetComponent<Miracle>(out var miracle))
                        {
                            //miracle.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Buffs)
                {
                    if (original.name == modItem.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        return ob;
                    }
                }
                foreach (var modItem in Data.Passives)
                {
                    if (original.name == modItem.Lv5Perk.PerkPrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv5Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                    else if (original.name == modItem.Lv10Perk.PerkPrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv10Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                    else if (original.name == modItem.Lv20Perk.PerkPrefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.Lv20Perk.AssetId);
                        if (ob.TryGetComponent<PassiveObject>(out var perk))
                        {
                            perk.enabled = true;
                        }
                        return ob;
                    }
                }
                foreach (var modItem in Data.Sephirites)
                {
                    if (original.name == modItem.Prefab.name)
                    {
                        //Core.Logger($"Bypassing Instantiate for: {original.name}");

                        var ob = UnityEngine.Object.Instantiate(original, position, rotation);
                        var identity = ob.AddComponent<NetworkIdentity>();
                        var assetId = identity.GetType().GetProperty("assetId");
                        assetId.SetValue(identity, modItem.AssetId);
                        if (ob.TryGetComponent<Sephirite>(out var sephirite))
                        {
                            sephirite.enabled = true;
                        }
                        return ob;
                    }
                }

                // 通常の Instantiate
                return UnityEngine.Object.Instantiate(original, position, rotation);
            }

            // 差し替え用メソッド
            public static bool CustomGetPrefab(uint assetId, out GameObject prefab)
            {
                //Core.Logger($"CustomGetPrefab: {assetId}");
                var on = OnGetPrefab?.Invoke(assetId);
                if(on != null)
                {
                    prefab = on;
                    return true;
                }

                if (assetId != 0 && !NetworkClient.prefabs.TryGetValue(assetId, out _))
                {
                    foreach(var modItem in Data.All)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.ResourcePrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Combos)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.ResourcePrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Miracles)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.Prefab;
                        return true;
                    }
                    foreach (var modItem in Data.Weapons)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.MainWeaponPrefab;
                        return true;
                    }
                    foreach (var modItem in Data.Buffs)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.gameObject;
                        return true;
                    }
                    foreach (var modItem in Data.Passives)
                    {
                        if (modItem.Lv5Perk.AssetId == assetId)
                        {
                            prefab = modItem.Lv5Perk.PerkPrefab;
                            return true;
                        }
                        if (modItem.Lv10Perk.AssetId == assetId)
                        {
                            prefab = modItem.Lv10Perk.PerkPrefab;
                            return true;
                        }
                        if (modItem.Lv20Perk.AssetId == assetId)
                        {
                            prefab = modItem.Lv20Perk.PerkPrefab;
                            return true;
                        }
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                    }
                    foreach (var modItem in Data.Sephirites)
                    {
                        if (modItem.AssetId != assetId)
                            continue;
                        //Core.Logger($"CustomGetPrefab: {modItem.Name}");
                        prefab = modItem.Prefab;
                        return true;
                    }
                }

                    // 通常の GetPrefab
                return NetworkClient.GetPrefab(assetId, out prefab);
            }
        }*/

        /// <summary>
        /// SpriteFxの登録パッチ
        /// </summary>

        [HarmonyPatch(typeof(ObjectPoolingFactory<SpriteFx>), nameof(ObjectPoolingFactory<SpriteFx>.Initialize))]
        public static class ObjectPoolingFactoryInitializePatch
        {
            static void Postfix(ObjectPoolingFactory<SpriteFx> __instance, Transform parent, IEnumerable<GameObject> poolablePrefabs, string defaultKey)
            {
                if (__instance == null)
                    return;
                if (!__instance.GetType().IsGenericType || __instance.GetType().GenericTypeArguments.FirstOrDefault() != typeof(SpriteFx))
                    return;

                foreach (var original in poolablePrefabs)
                {
                    foreach(var moditem in Data.SpriteFxs)
                    {
                        if(moditem.ResourcePrefab == null && original.name == moditem.OriginalName && original.TryGetComponent<SpriteFx>(out var fx))
                        {
                            Core.LoggerMany($"Init {moditem.Name}. (OriginalName): {moditem.OriginalName}");
                            moditem.InitPrefab(fx);
                        }
                    }
                }
                foreach (var moditem in Data.SpriteFxs)
                {
                    if (moditem.ResourcePrefab == null)
                    {
                        Core.LoggerError($"Failed to initialize SpriteFx prefab for {moditem.Name}. ResourcePrefab is null (OriginalName): {moditem.OriginalName}");
                        continue;
                    }
                    MakePool(__instance, parent, moditem.ResourcePrefab);
                }
                /*
                foreach(var moditem in Data.Weapons)
                {
                    moditem.OnSpriteFxRegistered();
                }*/
            }
            static void MakePool<T>(ObjectPoolingFactory<T> __instance, Transform parent, GameObject prefab) where T : ObjectPoolable
            {
                if (__instance.pooledGameObjects.ContainsKey(prefab))
                {
                    Core.LoggerWarning($"이미 같은 오브젝트가 풀에 등록되어 있습니다. {prefab.name}");
                    //return;
                }

                __instance.GetPrefabsByName()[prefab.name] = prefab;
                T component = prefab.GetComponent<T>();
                __instance.pooledGameObjects[prefab] = new List<T>();
                __instance.poolCountByObject[prefab] = 0;
                for (int i = 0; i < component.poolSize; i++)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(prefab, Vector2.zero, Quaternion.identity, parent);
                    if (!gameObject.activeSelf)
                    {
                        Debug.LogError("풀 오브젝트 프리팹은 activeSelf 켜주세요.");
                    }

                    gameObject.SetActive(value: false);
                    T component2 = gameObject.GetComponent<T>();
                    __instance.pooledGameObjects[prefab].Add(component2);
                }
            }
        }
    }
}