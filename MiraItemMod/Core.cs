using FMOD;
using HarmonyLib;
using Mirror;
using SephiriaMod.Items;
using SephiriaMod.Registries;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Type = System.Type;


namespace SephiriaMod
{
    public class Core : HorayModBase
    {
        public static void LoggerFew(string message)
        {
            Debug.Log("[MiraItemMod] " + message);
        }
        public static void LoggerMedium(string message)
        {
            Debug.Log("[MiraItemMod] " + message);
        }
        public static void LoggerMany(string message)
        {
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
        public static string ItemId
        {
            get
            {
                var sb = new StringBuilder();
                foreach(var line in ItemIdDic.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    sb.AppendLine(line.Replace('^', '	'));
                }
                return sb.ToString();
            }
        }
        public static string ItemIdOnly
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var line in ItemIdOnlyDic.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    sb.AppendLine(line.Replace('^', '	'));
                }
                return sb.ToString();
            }
        }
        public static string StatusId
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var status in StatusIdDic.OrderBy(x => x.Key).Select(x => x.Value))
                {
                    var name = KeywordDatabase.Convert(status.StatusName, useColor: false, useSprite: false, true);
                    var desc = KeywordDatabase.Convert(status.StatusDescription, useColor: false, useSprite: false, true);
                    if (name == $"Status_{status.StatusName}_Name")
                        name = string.Empty;
                    if (desc == $"Status_{status.StatusDescription}_Description")
                        desc = string.Empty;
                    //name = status.StatusName;
                    //desc = status.StatusDescription;
                    var line = $"{status.id}^{name.ToNoTag()}^{desc.ToNoTag()}";
                    sb.AppendLine(line.Replace('^', '	'));
                }
                return sb.ToString();
            }
        }
        public static Dictionary<int, string> ItemIdDic = new Dictionary<int, string>();
        public static Dictionary<int, string> ItemIdOnlyDic = new Dictionary<int, string>();
        public static Dictionary<string, StatusEntity> StatusIdDic = new Dictionary<string, StatusEntity>();

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
        public Harmony ModPatches;
        public bool IsInitialized = false;
        protected override void OnModLoaded()
        {
            ModPatches = new Harmony("com.Mira.MiraItemMod");
            ModPatches.PatchAll();
            base.OnModLoaded();
            if (!IsInitialized)
            {
                IsInitialized = true;
                CustomSpriteAsset.InitSprites();

                Data.Init();
            }

            CustomSpriteAsset.InitSpriteAsset();
        }
        protected override void OnModUnloaded()
        {
            if(ModPatches != null)
            {
                ModPatches.UnpatchSelf();
            }
            base.OnModUnloaded();
        }
        [HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), new Type[] { typeof(string), typeof(Type) })]
        class ResourcesLoadAllPatch
        {
            private static void ModifyItemEntity(ItemEntity item)
            {
                if (item.id == 3022)//メテオシャワー
                {
                    var prefab = item.resourcePrefab;
                    if (prefab.TryGetComponent<Charm_Magic>(out var charm))
                    {
                        if(charm.skill.magicPrefab.TryGetComponent<ActiveSkill_MeteorShower>(out var skill))
                        {
                            //skill.meteorFireTimer = new Timer(0.25f);
                            //skill.numberOfMeteorsByLevel = new int[3] { 24, 30, 36 };
                            skill.meteorFireTimer = new Timer(0.05f);
                            skill.numberOfMeteorsByLevel = new int[9] { 20, 25, 30, 35, 40, 45, 50, 60, 100 };//3 Level => DPS 1750（number100, Fire 51）
                            skill.damagePercentByLevel = new float[6] { 80f, 90f, 100f, 110f, 120f, 130f};
                            charm.skill.cooldownTime = 31f;//26 => ??
                            charm.skill.mpCostsByLevel = new int[9] { 23, 33, 37, 46, 51, 57, 66, 82, 102 };//35, 38, 41, 44
                            charm.maxLevel = 8;
                        }
                    }
                }
                if (item.id == 1039)//AutoMagic
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if (item.id == 1123 || item.id == 1124)//スタールビー・スターアクアマリン
                {
                    item.categories = new List<string> { ItemCategories.Lake, ItemCategories.Vitality };
                }
                if (item.id == 1196 || item.id == 1158 || item.id == 1005)//生命の手、強化ポーションキャップ、ハート形のニンジン
                {
                    item.categories = new List<string> { ItemCategories.Vitality };
                }
                if (item.id == 1017)//盾のイヤリング
                {
                    item.categories = new List<string> { ItemCategories.Vitality };
                }
                //if (item.isDual)
                    //Core.Logger(item.aName.ToString() + "(" + item.name + ")");
                if(item.id == 1255 || item.id == 1256 || item.id == 1257)//惑星属性絆
                {
                    //item.name == "1254_PlanetComet"
                    //item.activeType = EItemActiveType.Default;
                }
                //宝石の鎧、戦闘魔法使いの手袋、空の剣の握り、ライリーの懐中時計
                if (item.id == 1166 || item.id == 1169 || item.id == 1075 || item.id == 1165)
                {
                    var prefab = item.resourcePrefab;
                    if (prefab != null && prefab.TryGetComponent<Charm_Basic>(out var charm))
                    {
                        charm.isUniqueEffect = false;
                    }
                }
                if(item.id == 1117)//音叉
                {
                    /*
                    if(item.resourcePrefab.TryGetComponent<Charm_TuningForks>(out var charm))
                    {
                        charm.effectsString = [new LocalizedString("Charm_TuningForksMkII_Effect"), new LocalizedString("Charm_TuningForksMkII_Effect2")];
                        var component = item.resourcePrefab.AddComponent<Charm_TuningForksMkII>();
                        component.effectsString = charm.effectsString;
                        component.damageColor = charm.damageColor;
                        component.valueHUD_ID = charm.valueHUD_ID;
                        component.maxLevel = charm.maxLevel;
                        component.isUniqueEffect = charm.isUniqueEffect;
                        component.relatedWeapon = charm.relatedWeapon;
                        UnityEngine.Object.Destroy(charm);
                    }*/
                    //item.isDual = false;
                }
                if (item.id == 1076)//魔法の定法
                {
                    /*
                    if (item.resourcePrefab.TryGetComponent<Charm_StatusInstance>(out var charm))
                    {
                        charm.stats[2] = Data.CreateStatusGroup("FIRE_DAMAGE", 1, 2, 4, 7, 10);
                    }*/
                }
                if (item.id == 1235)//突き指南書
                {
                    item.categories = new List<string> { ItemCategories.Sturdy, ItemCategories.SkySong };
                }
                if (item.id == 1252)//常緑のマント
                {
                    item.categories = new List<string> { ItemCategories.Shadow, ItemCategories.SkySong };
                }
                if (item.id == 1149)//金色のマント
                {
                    item.categories = new List<string> { ItemCategories.WindSong, ItemCategories.SkySong };
                }
                if (item.id == 1082 || item.id == 1011 || item.id == 1093)//圧迫マント 風草のスカーフ いばらの茂み
                {
                    item.categories = new List<string> { ItemCategories.SkySong };
                }
                if (item.id == 1172)//パラスのカード
                {
                    item.categories = new List<string> { ItemCategories.Fortune };
                }
                if(item.id == 1262)//神秘の振り子
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if(item.id == 1265 || item.id == 1266 || item.id == 1274 || item.id == 1275 || item.id == 1276)//MPShield、MagicMP、フォールトファインダーニードル、さすらいの人の首飾り、獣の心臓
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if (item.id == 1188)//血石のイヤリング
                {
                    item.categories = new List<string> { ItemCategories.Drunk, ItemCategories.Vitality };
                    item.SetEntityRarity(EItemRarity.Rare);
                    item.isDual = true;

                    if(item.resourcePrefab.TryGetComponent<Charm_StatusInstance>(out var status) && status.stats.Length >= 2 && status.stats[1].statusID == "DEFENSE")
                    {
                        status.stats[1].valuesByLevel = new int[] { -10, -15, -20, -30 };
                    }
                }
                if (item.id == 1260)//雷の足跡
                {
                    //item.activeType = EItemActiveType.Default;
                }
                if (item.id == 1120)//血石の指輪
                {
                    item.categories = new List<string> { ItemCategories.Vitality };
                }
                var bond = "(" + new LocalizedString("ItemRarity_Dual").ToString() + ")";
                if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_Basic>(out var c))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}^{c.GetType().Name}";
                }
                else if(item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<StoneTablet>(out var t))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}^{t.GetType().Name}";
                }
                else if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<PotionEffect>(out var p))
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}^{p.GetType().Name}";
                }
                else
                {
                    ItemIdDic[item.id] = $"{item.id}^{item.name}^{item.aName.ToString()}^{item.activeType.ToJapanese()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}^{(item.resourcePrefab == null ? "プレハブ無し" : "プレハブ有り")}";
                }
                ItemIds(item);
            }
            private static void ItemIds(ItemEntity item)
            {
                if (item.activeType == EItemActiveType.Disabled)
                    return;
                var bond = "(" + new LocalizedString("ItemRarity_Dual").ToString() + ")";
                if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<Charm_Basic>(out var c))
                {
                    ItemIdOnlyDic[item.id] = $"{item.id}^{item.aName.ToString()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}";
                }
                else if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<StoneTablet>(out var t))
                {
                    ItemIdOnlyDic[item.id] = $"{item.id}^{item.aName.ToString()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}";
                }
                else if (item.resourcePrefab != null && item.resourcePrefab.TryGetComponent<PotionEffect>(out var p))
                {
                    ItemIdOnlyDic[item.id] = $"{item.id}^{item.aName.ToString()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}";
                }
                else
                {
                    ItemIdOnlyDic[item.id] = $"{item.id}^{item.aName.ToString()}^{item.type.ToJapanese()}^{item.rarity.ToJapanese() + (item.isDual ? bond : "")}";
                }
            }
            private static void ModifyItemCategoryEntity(ItemCategoryEntity category)
            {

            }
            private static void ModifyStatusEntity(StatusEntity status)
            {
                if(status.id == "HP_POTION_BONUS")
                {
                    status.divideForDisplay = 1;
                }
                StatusIdDic[status.id] = status;
            }
            private static void ModifyMiracle(Miracle miracle)
            {
                if(miracle.id == "Scholar")
                {
                    miracle.categories = new string[] { ItemCategories.Lake };
                }
                if(miracle.id == "IntelligenceAgent")
                {
                    miracle.categories = new string[] { ItemCategories.SkySong };
                }
                if(miracle.id == "Guard")
                {
                    miracle.categories = new string[] { ItemCategories.Vitality };
                }
                if (miracle.id == "Berserker")
                {
                    miracle.categories = new string[] { ItemCategories.Drunk };
                }
                if (miracle.id == "Elementalist")
                {
                    miracle.categories = new string[] { ItemCategories.Elemental };
                }
            }
            private static void ModifyKeywordEntity(KeywordEntity keyword)
            {
                if(keyword.keyword == "Toughness")
                {
                    keyword.displayDetails = true;
                }
            }
            static void Postfix(string path, Type systemTypeInstance, ref UnityEngine.Object[] __result)
            {
                //Core.Logger("Postfix: (" + systemTypeInstance.ToString() + ") " + path);
                if (systemTypeInstance == typeof(ItemEntity) && path == "Item")
                {
                    var list = __result.ToList();

                    Data.Register(list);

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

                    foreach (var item in list)
                        if (item is StatusEntity entity)
                            ModifyStatusEntity(entity);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(KeywordEntity) && path == "Keyword")
                {
                    var list = __result.ToList();

                    /*
                    var magicExecution = ScriptableObject.CreateInstance<KeywordEntity>();
                    magicExecution.name = "MagicExecution";
                    magicExecution.keyword = "MagicExecution";
                    magicExecution.visualText = new LocalizedString("Status_MagicExecution_Name");
                    magicExecution.description = new LocalizedString("Status_MagicExecution_Description");
                    magicExecution.detailedValue = new LocalizedString();
                    magicExecution.displayDetails = true;
                    foreach (var item in list)
                        if (item is KeywordEntity entity)
                        {
                            if(entity.keyword == "MagicDamageBonus")
                            {
                                magicExecution.keywordImage = entity.keywordImage;
                                magicExecution.textColor = entity.textColor;
                            }
                            else if(entity.keyword == "Elemental_Chaos")
                            {
                                magicExecution.connectedDetailEntities = [entity];
                            }
                        }

                    Core.Logger("New Keyword: " + magicExecution.visualText.ToString());
                    list.Add(magicExecution);*/

                    /*
                    var stargazeLevel = ScriptableObject.CreateInstance<KeywordEntity>();
                    stargazeLevel.name = "StargazeLevel";
                    stargazeLevel.keyword = "StargazeLevel";
                    stargazeLevel.visualText = new LocalizedString("Status_StargazeLevel_Name");
                    stargazeLevel.description = new LocalizedString("Status_StargazeLevel_Description");
                    stargazeLevel.detailedValue = new LocalizedString();
                    stargazeLevel.displayDetails = true;
                    stargazeLevel.textColor = Color.white;

                    Core.Logger("New Keyword: " + stargazeLevel.visualText.ToString());
                    list.Add(stargazeLevel);

                    var invLevel = ScriptableObject.CreateInstance<KeywordEntity>();
                    invLevel.name = "InvLevel";
                    invLevel.keyword = "InvLevel";
                    invLevel.visualText = new LocalizedString("Status_InvLevel_Name");
                    invLevel.description = new LocalizedString("Status_InvLevel_Description");
                    invLevel.detailedValue = new LocalizedString();
                    invLevel.displayDetails = true;
                    invLevel.textColor = Color.white;
                    Core.Logger("New Keyword: " + invLevel.visualText.ToString());
                    list.Add(invLevel);*/

                    /*
                    var binaryPlanet = ScriptableObject.CreateInstance<KeywordEntity>();
                    binaryPlanet.name = "BinaryPlanet";
                    binaryPlanet.keyword = "BinaryPlanet";
                    binaryPlanet.visualText = new LocalizedString("Status_BinaryPlanet_Name");
                    binaryPlanet.description = new LocalizedString("Status_BinaryPlanet_Description");
                    binaryPlanet.detailedValue = new LocalizedString();
                    binaryPlanet.displayDetails = true;
                    binaryPlanet.textColor = new Color(0.7f, 0.4f, 0.1f);
                    binaryPlanet.keywordImage = CustomSpriteAsset.BinaryPlanet;
                    Core.Logger("New Keyword: " + binaryPlanet.visualText.ToString());
                    list.Add(binaryPlanet);

                    var assasination = ScriptableObject.CreateInstance<KeywordEntity>();
                    assasination.name = "Assasination";
                    assasination.keyword = "Assasination";
                    assasination.visualText = new LocalizedString("Status_Assasination_Name");
                    assasination.description = new LocalizedString("Status_Assasination_Description");
                    assasination.detailedValue = new LocalizedString();
                    assasination.displayDetails = true;
                    assasination.textColor = new Color(0.9f, 0.1f, 0.1f);
                    assasination.keywordImage = CustomSpriteAsset.Assasination;
                    Core.Logger("New Keyword: " + assasination.visualText.ToString());
                    list.Add(assasination);*/


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

                    Data.RegisterWeapons(list);

                    __result = list.ToArray();
                }
                if(systemTypeInstance == typeof(PassiveEntity) && path == "Passive")
                {
                    var list = __result.ToList();

                    Data.RegisterPassives(list);

                    __result = list.ToArray();
                }
            }
        }
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.GetItemDropWeight), new Type[] { typeof(ItemEntity) })]
        public static class GridInventoryGetItemDropWeightPatch
        {
            static void Postfix(ItemEntity entity, ref int __result, ref GridInventory __instance)
            {
                int bonus = 0;
                if (__instance.itemDropBonusBySemantic.TryGetValue("DUAL", out bonus) && entity.isDual)
                {
                    //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                    __result += bonus;
                }
                if (__instance.itemDropBonusBySemantic.TryGetValue("TABLET", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                    __result += bonus;
                }
                if (__instance.itemDropBonusBySemantic.TryGetValue("DISCONNECT", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    if(entity.id == 2049)//断絶
                    {
                        //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                        __result += bonus;
                    }
                }
                if (__instance.itemDropBonusBySemantic.TryGetValue("NO_DISCONNECT", out bonus) && entity.type == EItemType.StoneTablet)
                {
                    if (entity.id != 2049)//断絶
                    {
                        //Debug.Log(string.Format("마법서 드롭 확률 보너스 가중치: {0}", bonus));
                        __result += bonus;
                    }
                }
                if(__instance.UnitAvatar.GetCustomStatUnsafe("AddGrimoire".ToUpperInvariant()) > 0)
                {
                    if (entity.resourcePrefab != null && entity.resourcePrefab.TryGetComponent<Charm_Magic>(out var magic))
                    {
                        var num = __result;
                        var value = 0;
                        if(__instance.itemDropBonusBySemantic.TryGetValue("GRIMOIRE", out value))
                        {
                            num -= value;
                        }
                        int currentCategoryItemDropWeight2 = __instance.GetCurrentCategoryItemDropWeight(ItemCategories.Grimoire, 0, bondBonus: false, allBondCategoryAcquired: false, addDefaultWeight: true);
                        num = Mathf.Max(num, currentCategoryItemDropWeight2);
                        __result = num + value;
                    }
                }
            }
        }
        /// <summary>
        /// アイテムのInstantiateパッチ
        /// </summary>
        [HarmonyPatch(typeof(GridInventory), nameof(GridInventory.SetItem))]
        public static class InstantiateStoneTabletPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (Core.LogFew)
                    Core.Logger($"InstantiateStoneTabletPatch Transpiler");
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
                if (Core.LogFew)
                    Core.Logger($"InstantiateItemCategoryPatch Transpiler");
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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for Combo: {original.name}");

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
                if (Core.LogFew)
                    Core.Logger($"InstantiateMiraclePatch Transpiler");
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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for Miracle: {original.name}");

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
        [HarmonyPatch(typeof(WeaponControllerSimple), "LocalEquipWeapon")]
        public static class InstantiateWeaponPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (Core.LogFew)
                    Core.Logger($"InstantiateWeaponPatch Transpiler");
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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for Weapon: {original.name}");

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
        [HarmonyPatch(typeof(PlayerAvatar), "AddPassiveStatOnServer")]
        public static class InstantiatePassivePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (Core.LogFew)
                    Core.Logger($"InstantiatePassivePatch Transpiler");
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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for lv5: {original.name}");

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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for lv5: {original.name}");

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
                        if (Core.LogMedium)
                            Core.Logger($"Bypassing Instantiate for lv5: {original.name}");

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
                if (Core.LogFew)
                    Core.Logger($"InstantiateNetworkClientPatch Transpiler");
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
                        if (Core.LogFew)
                            Core.Logger($"Transpiler SpawnPrefab Instantiate");
                        code.operand = replacement;
                    }
                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo mi2 && mi2.Name == nameof(NetworkClient.GetPrefab))
                    {
                        // Instantiate(GameObject) 呼び出しを CustomInstantiate に差し替える
                        if (Core.LogFew)
                            Core.Logger($"Transpiler SpawnPrefab GetPrefab");
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
                }

                    // 通常の GetPrefab
                return NetworkClient.GetPrefab(assetId, out prefab);
            }
        }
        [HarmonyPatch(typeof(GameDataLoader), "Awake")]
        public static class GameDataLoaderAwakePatch
        {
            static void Postfix(GameDataLoader __instance)
            {
                if (__instance == null || __instance != ModUtil.GetGameDataLoader())
                    return;
                if (Core.LogFew)
                    Core.Logger("Mod GameData Loading...");
                foreach(var miracle in Data.Miracles)
                {
                    miracle.LoadManuallyGivenItems();
                }
                //CustomCostumeDatabase.LoadAllStartingItems(CostumeDatabase.GetAll());
            }
        }
        [HarmonyPatch(typeof(GameDataLoader), "OnDestroy")]
        public static class GameDataLoaderOnDestroyPatch
        {
            static void Prefix(GameDataLoader __instance)
            {
                if (__instance == null || __instance != ModUtil.GetGameDataLoader())
                    return;
                if (Core.LogFew)
                    Core.Logger("Mod GameData Destroying...");
            }
        }
        //[HarmonyPatch(typeof(PlayerAvatar), "UpdateCostumeOutfit", [typeof(string)])]
        [Obsolete("CustomCustomeEntityのデバッグ用Patch")]
        public static class PlayerAvatarUpdateCostumeOutfit
        {
            static bool Prefix(string skinID, PlayerAvatar __instance)
            {
                Core.Logger("UpdateCostumeOutfit Pre: " + skinID);

                if (!__instance.TopdownActor.body)
                {
                    return true;
                }
                if (__instance.GetCurrentCostumeObject())
                {
                    __instance.GetCurrentCostumeObject().Unequip(__instance.isServer);
                    Animator2D_Basic component = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                    if (component)
                    {
                        component.SetAnimationSpeed("MOVE", null);
                        component.SetAnimationSpeed("MOVE_BACK", null);
                        component.SetAnimationSpeed("GREATSWORDSWING_LOWER", null);
                        component.SetAnimationSpeed("GREATSWORDSWING_UPPER", null);
                    }
                }


                CustomCostumeEntity costumeSkinEntity = CustomCostumeDatabase.FindCostumeByID(skinID);

                Core.Logger("UpdateCostumeOutfit: " + skinID);
                if (costumeSkinEntity == null)
                    return true;
                Core.Logger("UpdateCostumeOutfit New: " + costumeSkinEntity.costumeName);
                PlayerAvatarCostume component2 = costumeSkinEntity.costumePrefab.GetComponent<PlayerAvatarCostume>();
                __instance.SetCurrentCostumeObject(UnityEngine.Object.Instantiate<PlayerAvatarCostume>(component2, __instance.TopdownActor.body));
                __instance.GetCurrentCostumeObject().transform.localPosition = new Vector3(0, 0.4f, 0);
                __instance.GetCurrentCostumeObject().transform.localEulerAngles = Vector3.zero;
                if (__instance.GetCurrentCostumeObject().waterReflectionObject)
                {
                    __instance.GetCurrentCostumeObject().waterReflectionObject.SetParent(__instance.TopdownActor.bodyWrapper.transform.parent);
                    __instance.GetCurrentCostumeObject().waterReflectionObject.GetComponent<SyncLocalPosition>().syncTarget = __instance.TopdownActor.body.transform;
                }
                __instance.GetCurrentCostumeObject().Equip(__instance, __instance.isServer);
                __instance.moveFxPrefab = __instance.GetCurrentCostumeObject().moveFxPrefab;
                __instance.GetCurrentCostumeObject().hitbox.combatBehaviour = __instance;
                __instance.GetWeaponController().SetShoulderPositionScale(__instance.GetCurrentCostumeObject().bodyScale);
                __instance.stencilSolidColor = __instance.GetCurrentCostumeObject().stencilSolid;
                __instance.TopdownActor.bodyRenderer = __instance.GetCurrentCostumeObject().GetComponent<SpriteRenderer>();
                __instance.TopdownActor.animator = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                if (__instance.TopdownActor.animator)
                {
                    __instance.TopdownActor.animator.SetTransition(__instance.SetAnimationTransition(true, true));//(costumeSkinEntity.canLookBack, costumeSkinEntity.containsAttackAnimation)
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE_BACK", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_LOWER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_UPPER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                }
                __instance.TopdownRigidbody.MovementCollider.radius = __instance.GetCurrentCostumeObject().movementColliderRadius;
                return false;
            }
            static void P(string skinID, PlayerAvatar __instance)
            {
                CustomCostumeEntity costumeSkinEntity = CustomCostumeDatabase.FindCostumeByID(skinID);

                Core.Logger("UpdateCostumeOutfit: " + skinID);
                if (costumeSkinEntity == null)
                    return;
                Core.Logger("UpdateCostumeOutfit New: " + costumeSkinEntity.costumeName);
                PlayerAvatarCostume component2 = costumeSkinEntity.costumePrefab.GetComponent<PlayerAvatarCostume>();
                __instance.SetCurrentCostumeObject(UnityEngine.Object.Instantiate<PlayerAvatarCostume>(component2, __instance.TopdownActor.body));
                __instance.GetCurrentCostumeObject().transform.localPosition = Vector3.zero;
                __instance.GetCurrentCostumeObject().transform.localEulerAngles = Vector3.zero;
                if (__instance.GetCurrentCostumeObject().waterReflectionObject)
                {
                    __instance.GetCurrentCostumeObject().waterReflectionObject.SetParent(__instance.TopdownActor.bodyWrapper.transform.parent);
                    __instance.GetCurrentCostumeObject().waterReflectionObject.GetComponent<SyncLocalPosition>().syncTarget = __instance.TopdownActor.body.transform;
                }
                __instance.GetCurrentCostumeObject().Equip(__instance, __instance.isServer);
                __instance.moveFxPrefab = __instance.GetCurrentCostumeObject().moveFxPrefab;
                __instance.GetCurrentCostumeObject().hitbox.combatBehaviour = __instance;
                __instance.GetWeaponController().SetShoulderPositionScale(__instance.GetCurrentCostumeObject().bodyScale);
                __instance.stencilSolidColor = __instance.GetCurrentCostumeObject().stencilSolid;
                __instance.TopdownActor.bodyRenderer = __instance.GetCurrentCostumeObject().GetComponent<SpriteRenderer>();
                __instance.TopdownActor.animator = __instance.GetCurrentCostumeObject().GetComponent<Animator2D_Basic>();
                if (__instance.TopdownActor.animator)
                {
                    __instance.TopdownActor.animator.SetTransition(__instance.SetAnimationTransition(false, false));//(costumeSkinEntity.canLookBack, costumeSkinEntity.containsAttackAnimation)
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("MOVE_BACK", () => (__instance.IsRunning ? __instance.runSpeedMultiplier : 1f) * __instance.moveSpeed * __instance.moveSpeedMultiplier);
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_LOWER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                    __instance.TopdownActor.animator.SetAnimationSpeed("GREATSWORDSWING_UPPER", () => (100f + (float)__instance.GetCustomStatUnsafe("ATTACKSPEED")) * 0.01f * __instance.GetGreatsworSwingSpeed());
                }
                __instance.TopdownRigidbody.MovementCollider.radius = __instance.GetCurrentCostumeObject().movementColliderRadius;
            }
        }

        [HarmonyPatch(typeof(ObjectPoolingFactory<SpriteFx>), nameof(ObjectPoolingFactory<SpriteFx>.Initialize))]
        public static class ObjectPoolingFactoryInitializePatch
        {
            static void Postfix(ObjectPoolingFactory<SpriteFx> __instance, Transform parent, IEnumerable<GameObject> poolablePrefabs, string defaultKey)
            {
                if (__instance == null)
                    return;
                if (!__instance.GetType().IsGenericType || __instance.GetType().GenericTypeArguments.FirstOrDefault() != typeof(SpriteFx))
                    return;
                    //Core.Logger($"ObjectPoolingFactory<SpriteFx>.Initialize Postfix: {__instance.GetType().GenericTypeArguments.FirstOrDefault()}");

                //var path = "IceDagger\\";
                foreach (var original in poolablePrefabs)
                {
                    /*
                    if (original.name == "DaggerDashFx")
                    {
                        if (DashFx == null)
                        {
                            DashFx = UnityEngine.Object.Instantiate(original);
                            DashFx.name = "DaggerDashFx_Ice";
                            if (DashFx.TryGetComponent<SpriteFx>(out var fx))
                            {
                                var set = ScriptableObject.CreateInstance<AnimationSet>();
                                set.name = "DaggerDashFx_Ice";
                                set.sprites = [];
                                foreach (var state in fx.animator2D.currentSet.sprites)
                                {
                                    var newState = new AnimationSet.StateInfo();
                                    newState.fps = state.fps;
                                    newState.state = state.state;
                                    newState.repeat = state.repeat;
                                    newState.frameEvents = state.frameEvents;
                                    newState.soundEvents = state.soundEvents;
                                    newState.transformAttributes = state.transformAttributes;

                                    newState.timeline = [
                                        new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = SpriteLoader.LoadSprite(ModUtil.WeaponPath + path + "Weapon_Dagger_DashAttack_0") },
                                    new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 1, sprite = SpriteLoader.LoadSprite(ModUtil.WeaponPath + path + "Weapon_Dagger_DashAttack_1") }
                                        ];
                                    set.sprites.Add(newState);
                                }

                                fx.animator2D.currentSet = set;
                                fx.animator2D.ChangeSet(set);
                            }
                        }
                    }*/

                    foreach(var moditem in Data.SpriteFxs)
                    {
                        if(moditem.ResourcePrefab == null && original.name == moditem.OriginalName && original.TryGetComponent<SpriteFx>(out var fx))
                        {
                            moditem.InitPrefab(fx);
                        }
                    }
                }
                foreach (var moditem in Data.SpriteFxs)
                {
                    if (moditem.ResourcePrefab == null)
                        continue;
                    __instance.InvokeMakePool(parent, moditem.ResourcePrefab);
                }

                foreach(var moditem in Data.Weapons)
                {
                    moditem.OnSpriteFxRegistered();
                }
            }
        }
    }
}