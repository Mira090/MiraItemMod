using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Mirror;
using MiraItemMod.Items;
using MiraItemMod.Items.Pallas;
using MiraItemMod.UI;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AvatarStatsHooker;
using static GridInventory;

namespace MiraItemMod
{
    public static class Events
    {
        public static event Action<WeaponSimple_Crossbow> OnSubAttackCrossbow;
        public static event Action<WeaponSimple_Katana> OnSubAttackKatana;
        public static event Action<WeaponSimple_GreatSword> OnSubAttackGreatSword;
        public static event Action<string, uint, int> OnValueRecieved;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreBasicAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreSpecialAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreDashAttack;
        public static event Action<CharacterBuff> OnAppliedBuff;
        public static event Action<int> OnMiniBossKillCountChanged;

        public static EventReference HealSound { get; } = RuntimeManager.PathToEventReference("event:/Scene/healPotion_Small01");
        public static EventReference PerkSound { get; } = RuntimeManager.PathToEventReference("event:/System/talentPerk");
        static Events()
        {
            OnValueRecieved += (string command, uint netId, int value) =>
            {
                //Core.Logger($"Mod Chat({command}): {netId} To {value}");
            };
            //GameCamera.Instance.radialDistortion.Play(player.transform.position, RadialDistortion.Type.Weak);
            ///GameCamera.Instance.glitch.Play(0);
        }

        #region Chat
        public static void CommandValue(UnitAvatar player, NewItemOwnInstance item, int value)
        {
            if ((bool)DungeonManager.Instance)
            {
                if(item.Charm)
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/value {item.Charm.netId} {value}");
                if(item.StoneTablet)
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/value {item.StoneTablet.netId} {value}");
            }
            else
            {
                Core.LoggerWarning("CommandValue() DungeonManager.Instance is null!!");
            }
        }
        public static void ChatSound(UnitAvatar player, string name)
        {
            if ((bool)DungeonManager.Instance)
            {
                DungeonManager.Instance.Chat(player as PlayerAvatar, "Mod", $"/sound {name}");
            }
            else
            {
                Core.LoggerWarning("CommandValue() DungeonManager.Instance is null!!");
            }
        }
        public static void PlaySound(UnitAvatar player, string message)//  /sound 
        {
            var id = message.Remove(0, 7);

            try
            {
                EventInstance eventInstance = RuntimeManager.CreateInstance(id);
                eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
            }
            catch (EventNotFoundException e)
            {

            }
            var param = id.Split(' ');
            if(param.Length == 4)
            {
                var list = new List<int>();
                foreach(var p in param)
                {
                    if(Int32.TryParse(p, out var value))
                    {
                        list.Add(value);
                    }
                }
                if(list.Count == 4)
                {
                    var Guid = new FMOD.GUID();
                    Guid.Data1 = list[0];
                    Guid.Data2 = list[1];
                    Guid.Data3 = list[2];
                    Guid.Data4 = list[3];

                    var refe = new EventReference();
                    refe.Guid = Guid;
                    DungeonManager.Instance.Chat(player as PlayerAvatar, "Log", $"path {Guid.GUIDToPath()}");
                    try
                    {
                        EventInstance eventInstance = RuntimeManager.CreateInstance(refe);
                        eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                        eventInstance.start();
                        eventInstance.release();
                    }
                    catch(EventNotFoundException e)
                    {
                        DungeonManager.Instance.Chat(player as PlayerAvatar, "Log", $"the sound event is not Found!");
                    }
                }
            }

            foreach (var sound in typeof(Events).GetProperties().Where(p => p.PropertyType == typeof(EventReference)))
            {
                if (id != sound.Name)
                    continue;
                EventInstance eventInstance = RuntimeManager.CreateInstance((EventReference)sound.GetValue(typeof(Events)));
                eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
                eventInstance.start();
                eventInstance.release();
                return;
            }
        }
        #endregion



        #region Mod一覧表示
        /*
        [HarmonyPatch(typeof(UI_TitleLobby), ("Start"))]
        public static class UI_TitleLobbyPatch
        {
            public static GameObject ModListObject = null;
            public static RectTransform ModListTransform = null;
            public static TextMeshProUGUI ModListText = null;
            static void Postfix(UI_TitleLobby __instance)
            {
                ModListObject = new GameObject("ModListObject");
                ModListObject.transform.SetParent(__instance.transform);
                ModListText = ModListObject.AddComponent<TextMeshProUGUI>();
                //ModListObject.AddComponent<UI_LocalizationFontChanger>();
                ModListTransform = ModListObject.transform as RectTransform;
                ModListTransform.localPosition = Vector3.zero;
                ModListTransform.localScale = Vector3.one * 0.75f;
                ModListTransform.anchorMin = new Vector2(0f, 1f);
                ModListTransform.anchorMax = new Vector2(0f, 1f);
                ModListTransform.pivot = new Vector2(0f, 1f);
                ModListTransform.anchoredPosition = new Vector2(0f, 0f);


                StringBuilder sb = new("Mods\n");
                foreach (var melon in MelonMod.RegisteredMelons)
                {
                    sb.AppendLine("<size=150%>" + melon.Info.Name + "</size> <alpha=#AA>v" + melon.Info.Version + "\nby " + melon.Info.Author + "<alpha=#FF>");
                }
                ModListText.text = sb.ToString();
                ModListText.fontSize = 8;
                ModListText.textWrappingMode = TextWrappingModes.NoWrap;
                ModListText.margin = Vector4.one * 12f;
                ModListText.raycastTarget = false;
            }
        }*/
        #endregion

        #region 神秘の壺ブラックリスト
        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.GetMysticPotItems), new Type[] { typeof(EItemRarity) })]
        public static class UnitAvatarGetMysticPotItemsPatch
        {
            static void Postfix(EItemRarity targetRarity, ref ItemEntity[] __result, UnitAvatar __instance)
            {
                if(targetRarity == EItemRarity.Legend)
                {
                    var temp = __result.ToList();
                    temp.Remove(Data.WarCrime.ItemEntity);
                    __result = temp.ToArray();
                }
                else if(targetRarity == EItemRarity.Common)
                {
                    var temp = __result.ToList();
                    temp.Remove(Data.Malice.ItemEntity);
                    __result = temp.ToArray();
                }
            }
        }
        #endregion

        #region バフ付与イベント
        [HarmonyPatch(typeof(CharacterBuff), nameof(CharacterBuff.AddStack))]
        public static class CharacterBuffAddStackPatch
        {
            static void Postfix(CharacterBuff __instance)
            {
                OnAppliedBuff?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(CharacterBuff), nameof(CharacterBuff.Initialize))]
        public static class CharacterBuffInitializePatch
        {
            static void Postfix(CharacterBuff __instance)
            {
                OnAppliedBuff?.Invoke(__instance);
            }
        }
        #endregion

        #region 追加ステータス表示
        /// <summary>
        /// ステータス表示
        /// </summary>
        [HarmonyPatch(typeof(UI_StatsPanel), nameof(UI_StatsPanel.Connect))]
        public static class UIStatsPanelPatch
        {
            public static bool Patched { get; internal set; }
            static void Prefix(UI_StatsPanel __instance)
            {
                if (Patched)
                    return;
                Patched = true;
                try
                {
                    var tab = __instance.tab;
                    var common = tab.tabContents[0];
                    var special = tab.tabContents[1];

                    var jewelry = InstantiateInfo(__instance, special);
                    jewelry?.SetStats(new LocalizedString("ItemType_Misc"), Data.MaxMiracleCount.Id, Data.JewelryCount.Id);
                }
                catch (Exception e)
                {
                    Core.LoggerWarning("Status display patch failed.");
                    Core.LoggerWarning(e);
                }
            }
            private static UI_StatusTooltipOpenerManager InstantiateInfo(UI_StatsPanel panel, UI_TabContent tab)
            {
                var content = GetContent(tab);
                var example = GetInfoExample(content);
                return InstantiateInfo(panel, content, example);
            }
            private static RectTransform GetContent(UI_TabContent tab)
            {
                if (!tab.TryGetComponent<ScrollRect>(out var scroll))
                    return null;
                return scroll.content;
            }
            private static RectTransform GetInfoExample(RectTransform content)
            {
                if (content.childCount == 0)
                    return null;
                return content.GetChild(content.childCount - 1) as RectTransform;
            }
            private static UI_StatusTooltipOpenerManager InstantiateInfo(UI_StatsPanel panel, RectTransform content, RectTransform example)
            {
                var ob = UnityEngine.Object.Instantiate(example, content);
                var manager = ob.gameObject.AddComponent<UI_StatusTooltipOpenerManager>();
                manager.Init();
                panel.statElements = panel.statElements.AddRangeToArray(manager.Stats.ToArray());
                return manager;
            }
        }
        /// <summary>
        /// ステータス表示
        /// </summary>
        [HarmonyPatch(typeof(AvatarStatsHooker), nameof(AvatarStatsHooker.HookStat))]
        public static class AvatarStatsHookerPatch
        {
            static void Postfix(AvatarStatsHooker __instance, ref string __result, string id, ref EStatValueSign sign)
            {
                if (!__instance.TryGetComponent<UnitAvatar>(out var avatar))
                    return;

                if(id == Data.JewelryCount.Id)
                {
                    sign = GetStandardSign(avatar.GetCustomStatUnsafe(Data.JewelryCount.Name.ToSephiriaUpperId()));
                    __result = avatar.GetCustomStatUnsafe(Data.JewelryCount.Name.ToSephiriaUpperId()).ToString();
                }
                else if (id == Data.MaxMiracleCount.Id)
                {
                    if(avatar.gameObject.TryGetComponent<MiracleController>(out var miracle))
                    {
                        sign = GetStandardSign(miracle.maxMiracleCount - 1);
                        __result = (miracle.maxMiracleCount).ToString();
                    }
                }
                //Core.Logger(id + ": " + __result);
            }
            private static EStatValueSign GetStandardSign(float value, float defaultValue = 0f)
            {
                if (Mathf.Abs(value - defaultValue) <= float.Epsilon)
                {
                    return EStatValueSign.Neutral;
                }

                if (value < defaultValue)
                {
                    return EStatValueSign.Negative;
                }

                return EStatValueSign.Positive;
            }
        }
        #endregion

        #region ModChat
        [HarmonyPatch(typeof(DungeonManager), "UserCode_RpcChat__PlayerAvatar__String__String", new Type[] { typeof(PlayerAvatar), typeof(string), typeof(string) })]
        public static class DungeonManagerChatPatch
        {
            static bool Prefix(PlayerAvatar avatar, string name, string message, ref DungeonManager __instance)
            {
                if (name == "Mod" && message.StartsWith("/"))
                {
                    if (Core.LogMany)
                        Core.Logger($"Mod Chat({avatar.Name}): {message}");
                    if (message.StartsWith("/sound"))
                    {
                        PlaySound(avatar, message);
                    }
                    if (avatar.isOwned)
                    {
                        if (message == "/stargaze" && UIManager.Instance != null)
                        {
                            UIManager.Instance.GetElement<UI_SystemMessage>().Open(Charm_StargazeTablet.Notice.ToString(), 2.7f);

                            EventInstance eventInstance = RuntimeManager.CreateInstance(PerkSound);
                            eventInstance.set3DAttributes(avatar.transform.position.To3DAttributes());
                            eventInstance.start();
                            eventInstance.release();
                        }
                        if(message == "/sacrifice" && UIManager.Instance != null)
                        {
                            UIManager.Instance.GetElement<UI_SystemMessage>().Open(Charm_CompanionSacrifice.Notice.ToString(), 2.7f);

                            EventInstance eventInstance = RuntimeManager.CreateInstance(PerkSound);
                            eventInstance.set3DAttributes(avatar.transform.position.To3DAttributes());
                            eventInstance.start();
                            eventInstance.release();
                        }
                        if(message == "/dash")
                        {
                            if (avatar.CurrentDashModule.currentDashCount > 0)
                                avatar.CurrentDashModule.currentDashCount--;
                            avatar.Dash(avatar.NetworkaimObject.transform.position);
                        }
                        if (message == "/dash_heal")
                        {
                            if (avatar.CurrentDashModule.currentDashCount > 0)
                                avatar.CurrentDashModule.currentDashCount--;
                        }
                        if (message.StartsWith("/value"))
                        {
                            var data = message.Split(' ');
                            if(data.Length == 3 && uint.TryParse(data[1], out uint netId) && int.TryParse(data[2], out int value) )
                            {
                                OnValueRecieved?.Invoke(data[0], netId, value);
                            }
                            else
                            {
                                Core.LoggerWarning("Error: " + message);
                            }
                        }
                    }
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region ダメージ表示
        [HarmonyPatch(typeof(UnitAvatar), "RpcShowDamageParticle", new Type[] { typeof(Vector2), typeof(string), typeof(Color), typeof(int), typeof(bool), typeof(UnitAvatar), typeof(UnitAvatar) })]
        [Obsolete]
        public static class UnitAvatarRpcShowDamageParticlePatch
        {
            static void Prefix(Vector2 position, ref string msg, ref Color color, int fontSize, bool isPrivate, UnitAvatar self, UnitAvatar attacker, UnitAvatar __instance)
            {
                //Core.Logger("RpcShowDamageParticle");
                if(color.a == 0 && color.r == 1 && color.g == 0 && color.b == 0)
                {
                    msg = "<sprite=\"Keyword\" name=Assasination>" + msg;
                    color = new Color(1, 0, 0);
                }
                else if(color.a == 0)
                {
                    //Core.Logger("RpcShowDamageParticle blue!");
                    msg = msg.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = new Color(0.2784f, 0.5529f, 0.9804f);
                }
            }
        }
        [HarmonyPatch(typeof(UI_DamageParticle), nameof(UI_DamageParticle.SetDamage))]
        public static class UI_DamageParticlePatch
        {
            static void Prefix(ref string damage, ref Color color, UI_DamageParticle __instance)
            {
                var text = __instance.GetText();
                text.enableVertexGradient = false;
                if (color.a == 0 && color.r == 1 && color.g == 0 && color.b == 0)
                {
                    damage = "<sprite=\"Keyword\" name=Assasination>" + damage;
                    color = new Color(1, 0, 0);
                }
                else if (color == ModUtil.FourGradation)
                {
                    text.colorGradient = new VertexGradient()
                    {
                        bottomLeft = Color.white,
                        bottomRight = Color.cyan,
                        topLeft = new Color(0.2f, 0.5f, 1f),
                        topRight = new Color(1, 1, 0),
                    };
                    text.enableVertexGradient = true;
                    color = Color.white;
                }
                else if (color == ModUtil.FourGradationMagicExecution)
                {
                    text.colorGradient = new VertexGradient()
                    {
                        bottomLeft = Color.white,
                        bottomRight = Color.cyan,
                        topLeft = new Color(0.2f, 0.5f, 1f),
                        topRight = new Color(1, 1, 0),
                    };
                    text.enableVertexGradient = true;
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = Color.white;
                }
                else if(color == ModUtil.Excavation)
                {
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=ExcavationJewelry>");
                    color = new Color32(255, 216, 53, 255);
                }
                else if (color == ModUtil.ExcavationFaild)
                {
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=Excavation>");
                    color = new Color32(178, 175, 120, 255);
                }
                else if (color.a == 0)
                {
                    damage = damage.Replace("<sprite=\"Keyword\" name=CriticalChance>", "<sprite=\"Keyword\" name=MagicDamageBonus>");
                    color = new Color(0.2784f, 0.5529f, 0.9804f);
                }
            }
        }
        #endregion

        #region 武器イベント
        [HarmonyPatch(typeof(WeaponSimple_Crossbow), nameof(WeaponSimple_Crossbow.SubAttackButtonDown))]
        public static class WeaponSimple_CrossbowPatch
        {
            static void Postfix(WeaponSimple_Crossbow __instance)
            {
                OnSubAttackCrossbow?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Katana), nameof(WeaponSimple_Katana.SubAttackButtonDown))]
        public static class WeaponSimple_KatanaPatch
        {
            static void Postfix(WeaponSimple_Katana __instance)
            {
                OnSubAttackKatana?.Invoke(__instance);
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_GreatSword), nameof(WeaponSimple_GreatSword.SubAttackButtonUp))]
        public static class WeaponSimple_GreatSwordPatch
        {
            static void Postfix(WeaponSimple_GreatSword __instance)
            {
                OnSubAttackGreatSword?.Invoke(__instance);
            }
        }
        [HarmonyPatch]
        public class WeaponControllerSimplePatch
        {
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing))]
            [HarmonyPrefix]
            public static void PrefixBasic(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }

            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwingFullyManual))]
            [HarmonyPrefix]
            public static void PrefixBasicFullyManual(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing_ManualDirection))]
            [HarmonyPrefix]
            public static void PrefixBasicManualDirection(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateDashAttackSwing))]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponControllerSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwing))]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwingFullyManual))]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
        }
        [HarmonyPatch]
        public class WeaponSimplePatch
        {
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateDashAttackProjectile))]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectile))]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponSimple __instance)
            {
                //Core.Logger("PrefixSpecial");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectileFullyManual))]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponSimple __instance)
            {
                //Core.Logger("PrefixSpecialFullyManual");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
        }
        #endregion

        #region アイテムドロップ確率
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
                    if (entity.id == 2049)//断絶
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
                if (__instance.UnitAvatar.GetCustomStatUnsafe("AddGrimoire".ToUpperInvariant()) > 0)
                {
                    if (entity.resourcePrefab != null && entity.resourcePrefab.TryGetComponent<Charm_Magic>(out var magic))
                    {
                        var num = __result;
                        var value = 0;
                        if (__instance.itemDropBonusBySemantic.TryGetValue("GRIMOIRE", out value))
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
        #endregion

        #region 魔導書カテゴリー
        [HarmonyPatch(typeof(Charm_Basic), nameof(Charm_Basic.GetItemCategory), new Type[] { })]
        public static class CharmGetItemCategoryPatch
        {
            static void Postfix(Charm_Basic __instance, ref IEnumerable<string> __result)
            {
                if (__instance is Charm_Magic magic)
                {
                    if (magic.NetworkAvatar != null && magic.NetworkAvatar.GetCustomStatUnsafe("AddGrimoire".ToUpperInvariant()) > 0 && !__result.Contains(ItemCategories.Grimoire))
                    {
                        __result = __result.AddItem(ItemCategories.Grimoire);
                    }
                }
            }
        }
        #endregion

        #region ショップ拡張
        public static readonly string AdditionalShop = "AdditionalShop".ToUpperInvariant();
        public static readonly string AdditionalShopLegendary = "AdditionalShopLegendary".ToUpperInvariant();
        public static readonly string AdditionalShopInventory = "AdditionalShopInventory".ToUpperInvariant();
        public static readonly string AdditionalMoney = "AdditionalMoney".ToUpperInvariant();

        public static readonly string ReplenishmentCharm = "ReplenishmentCharm".ToUpperInvariant();
        public static readonly string ReplenishmentTablet = "ReplenishmentTablet".ToUpperInvariant();

        public static int DefaultAdditionalShop = 0;
        public static int DefaultAdditionalShopLegendary = 0;
        public static int DefaultAdditionalMoney = 0;

        public static int DefaultReplenishmentCharm = 0;
        public static int DefaultReplenishmentTablet = 0;

        [HarmonyPatch(typeof(UnitAI_NewBasic), nameof(UnitAI_NewBasic.SetSocialID))]
        public static class SetSocialIDPatch
        {
            static void Postfix(UnitAI_NewBasic __instance, string socialID, string nameSource, EPersonality personality, EFactionAlignment alignment, string roleName, EProceduralMerchantType merchant, int startingMoney, ItemMetadata[] startingItems)
            {
                if (merchant == EProceduralMerchantType.None)
                    return;
                System.Random random = new System.Random(__instance.Avatar.RandomID);

                if (socialID.StartsWith("TrialMerchant_"))
                {
                    var splited = socialID.Split("_");
                    if(splited.Length == 3 && int.TryParse(splited[1], out var phase))
                    {
                        OnTrialMerchant(__instance, random, phase);
                    }
                    else
                    {
                        OnTrialMerchant(__instance, random);
                    }
                }

                int more = DefaultAdditionalShop;
                int legendary = DefaultAdditionalShopLegendary;
                int money = DefaultAdditionalMoney;
                List<int> inventory = new List<int>();
                foreach (var connection in NetworkServer.connections.Values)
                {
                    if (connection == null || connection.identity == null || connection.identity.gameObject == null)
                        return;
                    if (!connection.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                        return;
                    more += player.GetCustomStatUnsafe(AdditionalShop);
                    money += player.GetCustomStatUnsafe(AdditionalMoney);
                    legendary += player.GetCustomStatUnsafe(AdditionalShopLegendary);
                    inventory.Add(player.GetCustomStatUnsafe(AdditionalShopInventory));
                }
                if (more < 0)
                    more = 0;
                if (legendary < 0)
                    legendary = 0;

                if ((bool)DungeonManager.Instance && DungeonManager.Instance.dungeonEnvironment.TryGetValue("RedMerchant", out var value2) && value2 > 0 && __instance.Avatar.faction == "Merchant")
                {
                    return;
                }

                if (money > 0)
                {
                    __instance.Avatar.AddMoney(money);
                    //Core.Logger($"{__instance.name}: Add {money} leafs");
                }
                if(legendary > 0 && merchant != EProceduralMerchantType.VendorButNoItem)
                {
                    List<ItemMetadata> legendaries = new List<ItemMetadata>();

                    if (merchant == EProceduralMerchantType.Vendor || merchant == EProceduralMerchantType.MerchantUnionVendor || merchant == EProceduralMerchantType.SmallVendor)
                    {
                        Events.AddTradingCharms(random, legendaries, legendary, EItemRarity.Legend);
                    }
                    else if(merchant == EProceduralMerchantType.PotionVendor)
                    {
                        int potion = random.Next(0, 2) switch
                        {
                            0 => 35,
                            _ => 36,
                        };
                        legendaries.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(potion), 1));
                    }

                    if ((bool)__instance.NetworkMySafe)
                    {
                        __instance.NetworkMySafe.GenerateItemInInventory(legendaries.ToArray());
                    }
                    else if ((bool)__instance.Avatar.Inventory)
                    {
                        __instance.Avatar.Inventory.AddItems(legendaries.ToArray());
                    }

                    //Core.Logger($"{__instance.name}: Add {legendaries.Count} legendary items");
                }


                List<ItemMetadata> invlist = new List<ItemMetadata>();
                foreach (var inv in inventory)
                {
                    if (inv <= 0)
                        continue;
                    if(random.Next(0, 100) < inv)
                    {
                        invlist.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(Data.AddInventory.Id), 1));
                    }
                }

                if(invlist.Count > 0)
                {
                    if ((bool)__instance.NetworkMySafe)
                    {
                        __instance.NetworkMySafe.GenerateItemInInventory(invlist.ToArray());
                    }
                    else if ((bool)__instance.Avatar.Inventory)
                    {
                        __instance.Avatar.Inventory.AddItems(invlist.ToArray());
                    }
                }

                if (more <= 0)
                    return;

                //Core.Logger($"{__instance.name}: Add {more} items");

                List<ItemMetadata> list = new List<ItemMetadata>();
                switch (merchant)
                {
                    case EProceduralMerchantType.Vendor:
                        {
                            int charms = random.Next(0, more + 1);
                            int stoneTablets = more - charms;
                            UnitAI_NewBasic.AddTradingItemsToList(random, list, charms, stoneTablets, 0);
                            break;
                        }
                    case EProceduralMerchantType.MerchantUnionVendor:
                        {
                            int charms2 = random.Next(0, more + 1);
                            int stoneTablets2 = more - charms2;
                            UnitAI_NewBasic.AddTradingItemsToList(random, list, charms2, stoneTablets2, 0);
                            break;
                        }
                    case EProceduralMerchantType.SmallVendor:
                        {
                            int charms3 = more;
                            UnitAI_NewBasic.AddTradingItemsToList(random, list, charms3, 0, 0);
                            break;
                        }
                    case EProceduralMerchantType.PotionVendor:
                        {
                            for (int q = 0; q < more; q++)
                            {
                                switch (UnityEngine.Random.Range(0, 8))
                                {
                                    case 1:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(36), 1));
                                        break;
                                    case 2:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(30), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(31), 1));
                                        break;
                                    case 3:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(33), 1));
                                        break;
                                    case 4:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(28), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(29), 1));
                                        break;
                                    case 5:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(28), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(30), 1));
                                        break;
                                    case 6:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(34), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(27), 1));
                                        break;
                                    case 7:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(34), 1));
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(38), 1));
                                        break;
                                    default:
                                        list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(35), 1));
                                        break;
                                }
                            }
                            break;
                        }
                    case EProceduralMerchantType.VendorButNoItem:
                        //list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(1), 1));
                        break;
                }

                if ((bool)__instance.NetworkMySafe)
                {
                    __instance.NetworkMySafe.GenerateItemInInventory(list.ToArray());
                }
                else if ((bool)__instance.Avatar.Inventory)
                {
                    __instance.Avatar.Inventory.AddItems(list.ToArray());
                }
            }
            static void OnTrialMerchant(UnitAI_NewBasic __instance, System.Random random, int phase = -1)
            {
                var list = new List<ItemMetadata>();

                //list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(Data.AddInventory.Id), 1));
                list.Add(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), ItemDatabase.FindItemById(Data.AddMaxMiracle.Id), 1));

                if (list.Count > 0)
                {
                    if ((bool)__instance.NetworkMySafe)
                    {
                        __instance.NetworkMySafe.GenerateItemInInventory(list.ToArray());
                    }
                    else if ((bool)__instance.Avatar.Inventory)
                    {
                        __instance.Avatar.Inventory.AddItems(list.ToArray());
                    }
                }
            }
        }
        [HarmonyPatch(typeof(UnitAI_NewBasic), nameof(UnitAI_NewBasic.AddReplenishmentItemsClientside))]
        public static class AddReplenishmentItemsClientsidePatch
        {
            static void Prefix(UnitAI_NewBasic __instance, ref int charms, ref int stoneTablets)
            {
                if (NetworkClient.localPlayer == null || NetworkClient.localPlayer.gameObject == null)
                    return;
                if (!NetworkClient.localPlayer.TryGetComponent<PlayerAvatar>(out var player))
                    return;
                charms += DefaultReplenishmentCharm;
                stoneTablets += DefaultReplenishmentTablet;
                charms += player.GetCustomStatUnsafe(ReplenishmentCharm);
                stoneTablets += player.GetCustomStatUnsafe(ReplenishmentTablet);
                if (charms < 0)
                    charms = 0;
                if (stoneTablets < 0)
                    stoneTablets = 0;
            }
        }
        [HarmonyPatch(typeof(UI_ShopPanel), "UpdateReplenishmentIcon")]
        public static class UpdateReplenishmentIconPatch
        {
            static void Postfix(UI_ShopPanel __instance)
            {
                __instance.replenishmentZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 14 + __instance.replenishmentIcons.Count * 40);
            }
        }
        [HarmonyPatch(typeof(UI_ScrollToSelection), "ScrollRectToLevelSelection")]
        public static class ScrollRectToLevelSelectionPatch
        {
            static bool Prefix(UI_ScrollToSelection __instance)
            {
                var target = typeof(UI_ScrollToSelection).GetProperty("CurrentTargetRectTransform", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as RectTransform;
                if (target != null && target.gameObject.name == "ReplenishmentZone")
                    return false;
                return true;
            }
        }
        public static void AddTradingCharms(System.Random itemSeed, List<ItemMetadata> list, int charms, EItemRarity rarity)
        {
            if (charms <= 0)
                return;

            List<int> items = new List<int>();
            foreach (PlayerSpawner multiplayer in PlayerSpawner.MultiplayerList)
            {
                if (!multiplayer)
                {
                    continue;
                }

                PlayerAvatar playerAvatar = multiplayer.PlayerAvatar;
                multiplayer.GetComponent<WeaponControllerSimple>();
                foreach (int unlockedCharm in multiplayer.unlockedCharms)
                {
                    ItemEntity itemEntity = ItemDatabase.FindItemById(unlockedCharm);
                    if ((bool)itemEntity && !itemEntity.isDual)
                    {
                        Charm_Basic charm_Basic = (itemEntity.resourcePrefab ? itemEntity.resourcePrefab.GetComponent<Charm_Basic>() : null);
                        if ((bool)charm_Basic && !charm_Basic.isWeaponRelatedCharm && !itemEntity.cannotBeReward && !playerAvatar.Inventory.HasItem(itemEntity, out var _, out var _, out var _))
                        {
                            if (itemEntity.rarity == rarity)
                                items.Add(unlockedCharm);
                        }
                    }
                }
            }

            for (int i = 0; i < charms; i++)
            {
                int elementAt = items[itemSeed.Next(0, items.Count)];
                ItemEntity itemEntity2 = ItemDatabase.FindItemById(elementAt);

                if (itemEntity2 != null)
                {
                    list.Add(new ItemMetadata(itemSeed.Next(), itemEntity2, 1));
                }
            }
        }
        #endregion

        #region ポーションバッグ枠
        [HarmonyPatch]
        public static class UpdateInventorySizePatch
        {
            [HarmonyPatch(typeof(UI_OtherCharacterPanel), "UpdateInventorySize", new Type[] { typeof(int), typeof(int), typeof(int) })]
            [HarmonyPrefix]
            static void OtherPrefix(int inventoryWidth, int inventoryHeight, ref int potionStorage)
            {
                if(potionStorage > 6)
                    potionStorage = 6;
            }
            [HarmonyPatch(typeof(UI_CharacterStatusPanel), "SetPotionStorageCount", new Type[] { typeof(int) })]
            [HarmonyPrefix]
            static void StatusPrefix(ref int obj)
            {
                if (obj > 6)
                    obj = 6;
            }
        }
        #endregion

        #region 奇跡の最大数
        [HarmonyPatch(typeof(UI_MiracleElement), nameof(UI_MiracleElement.HandleClick))]
        public static class MiracleElementPatch
        {
            static bool Prefix(UI_MiracleElement __instance)
            {
                var actor = __instance.GetActor();
                var entity = __instance.GetEntity();
                var miracleSelector = __instance.GetMiracleSelector();
                int num = actor.UnitAvatar.Inventory.GetEmptySlotCount(EItemType.Charm);
                int num2 = actor.UnitAvatar.Inventory.GetEnptyPotionStorageCount();
                Miracle prefab = MiracleDatabase.FindMiracle(entity.id);
                ItemMetadata[] items = prefab.GetItems(generateInstanceID: false, actor, entity.instanceID);
                if (items != null)
                {
                    ItemMetadata[] array = items;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ItemEntity itemEntity = ItemDatabase.FindItemById(array[i].entityID);
                        if (itemEntity.type == EItemType.Potion)
                        {
                            if (num2 > 0)
                            {
                                num2--;
                            }
                            else
                            {
                                num--;
                            }
                        }
                        else if (itemEntity.type == EItemType.Charm)
                        {
                            Charm_Basic charmInstance;
                            if (actor.UnitAvatar.Inventory.uniquePairCount > 0)
                            {
                                if (!actor.UnitAvatar.Inventory.HasItem(itemEntity, out var _, out var _, out var _))
                                {
                                    num--;
                                }
                            }
                            else if (!actor.UnitAvatar.Inventory.TryGetUniqueEffect(itemEntity, out charmInstance))
                            {
                                num--;
                            }
                        }
                        else
                        {
                            num--;
                        }
                    }
                }

                if (num >= 0 && num2 >= 0)
                {
                    if (actor.CanAddMiracle())
                    {
                        UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(Loc.Convert(__instance.miracleSelectText.ToString(), "MIRACLE", prefab.aName.ToString()), delegate
                        {
                            actor.AddMiracle(entity.id);
                            actor.CloseMiraclePanel();
                            if (items != null)
                            {
                                actor.UnitAvatar.Inventory.AddItemsWithGenerateInstanceID(entity.instanceID, items);
                            }

                            miracleSelector.HandleMiracleAcquired(actor);
                        }, null);
                        return false;
                    }

                    LocalizedString aName = actor.miracles[0].aName;
                    UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(Loc.Convert(__instance.miracleChangeText.ToString(), "ALREADY", aName.ToString()), delegate
                    {
                        actor.RemoveMiracle(0);
                        actor.AddMiracle(entity.id);
                        actor.CloseMiraclePanel();
                        if (items != null)
                        {
                            actor.UnitAvatar.Inventory.AddItemsWithGenerateInstanceID(entity.instanceID, items);
                        }

                        miracleSelector.HandleMiracleAcquired(actor);
                    }, null);
                    return false;
                }

                UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(__instance.itemFullText.ToString(), delegate
                {
                    if (actor.CanAddMiracle())
                    {
                        UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(Loc.Convert(__instance.miracleSelectText.ToString(), "MIRACLE", prefab.aName.ToString()), delegate
                        {
                            actor.AddMiracle(entity.id);
                            actor.CloseMiraclePanel();
                            if (items != null)
                            {
                                actor.UnitAvatar.Inventory.AddItemsWithGenerateInstanceID(entity.instanceID, items);
                            }

                            miracleSelector.HandleMiracleAcquired(actor);
                        }, null);
                    }
                    else
                    {
                        LocalizedString aName2 = actor.miracles[0].aName;
                        UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(Loc.Convert(__instance.miracleChangeText.ToString(), "ALREADY", aName2.ToString()), delegate
                        {
                            actor.RemoveMiracle(0);
                            actor.AddMiracle(entity.id);
                            actor.CloseMiraclePanel();
                            if (items != null)
                            {
                                actor.UnitAvatar.Inventory.AddItemsWithGenerateInstanceID(entity.instanceID, items);
                            }

                            miracleSelector.HandleMiracleAcquired(actor);
                        }, null);
                    }
                }, null);
                return false;
            }
        }
        #endregion

        #region 総合訓練所のリセット
        [HarmonyPatch(typeof(TrainingSchoolLeaveDoor), nameof(TrainingSchoolLeaveDoor.MoveTo))]
        public static class TrainingSchoolLeaveDoorMoveToPatch
        {
            static void Postfix(TrainingSchoolLeaveDoor __instance, GameObject actor)
            {
                if (actor.TryGetComponent<PlayerAvatar>(out var player))
                {
                    player.ClearOrphanedStatusInstance();
                    if(actor.TryGetComponent<TreeShopItemStorage>(out var tree))
                    {
                        player.SetMoney(tree.GettStartingMoney());
                    }
                    else
                    {
                        player.SetMoney(0);
                    }
                    player.NetworkreservedMp = 0;
                    player.Inventory?.ClearDungeonTempLevels();
                    RemoveOtherItems(player.Inventory);
                }
                if (actor.TryGetComponent<MiracleController>(out var miracle))
                {
                    miracle.ClearMiracle();
                }
                if(actor.TryGetComponent<LevelController>(out var level))
                {
                    level.levelUpQueue.Clear();
                }
            }
            private static void RemoveOtherItems(GridInventory inventory)
            {
                if (inventory == null)
                    return;
                using (new Permission(inventory))
                {
                    foreach (ItemPosition item in new List<ItemPosition>(inventory.inventoryMatrix.Keys))
                    {
                        NewItemOwnInstance newItemOwnInstance = inventory.inventoryMatrix[item];
                        if (newItemOwnInstance.Entity != null && (newItemOwnInstance.Entity.rarity == EItemRarity.Eternal || newItemOwnInstance.Entity.IsJewelry()))
                        {
                            inventory.ForceRemoveItem(item.x, item.y);
                        }
                    }
                }
            }
        }
        #endregion

        #region DevCommandPanel

        public static Dictionary<string, string> commandDescriptionsJapanese = new Dictionary<string, string>
    {
        { "MIRACLE", "奇跡の選択肢を生成" },
        { "ROOMINFO", "部屋情報の表示切り替え" },
        { "MULTI", "マルチプレイヤーパネルを開く" },
        { "ADDPOTIONSTORAGE", "ポーションバッグ枠を追加" },
        { "PASSIVE", "才能パネルを開く" },
        { "WEAPONENHANCEMENT", "武器強化パネルを開く" },
        { "TGM", "無敵モードの切り替え" },
        { "ADDKEY", "鍵の追加 (パラメータ: 鍵ID)" },
        { "SUICIDE", "自殺コマンド" },
        { "ADDSAPPHIRE", "サファイア追加 (パラメータ: 数量)" },
        { "COSTUME", "コスチュームパネルを開く" },
        { "EQUIPCOSTUME", "コスチュームを装備 (パラメータ: コスチュームID)" },
        { "UNLOCKCOSTUME", "コスチュームを強制アンロック (パラメータ: コスチュームID)" },
        { "UNLOCKCOSTUMEALL", "すべてのコスチュームを強制アンロック" },
        { "UNLOCKCOSTUMESKINALL", "すべてのコスチュームのスキンを強制アンロック" },
        { "LOCKCOSTUME", "コスチュームをロック (パラメータ: コスチュームID)" },
        { "CONNECT", "他のホストに接続 (パラメータ: ホストID)" },
        { "GOTOHOSTTOWN", "ホストの町に移動" },
        { "GOTOMYTOWN", "自分の町に移動" },
        { "WORLDMAP", "ワールドマップパネルを開く" },
        { "DISTORTION", "画面歪み効果を再生" },
        { "GLITCH", "グリッチ効果を再生" },
        { "SUMMON", "ユニットを召喚 (パラメータ: ユニットID, [任意] 数量)" },
        { "SUMMON_POS", "指定位置にユニットを召喚 (パラメータ: ユニットID, X, Y)" },
        { "SUMMONNPC", "NPCを召喚 (パラメータ: NPC名)" },
        { "TREESHOP", "運命の刻印を開く" },
        { "TUTORIALCLEAR", "チュートリアルクリア処理" },
        { "SETLANGUAGE", "言語設定変更 (パラメータ: 言語コード)" },
        { "SETRESOLUTION", "解像度設定 (パラメータ: 幅, 高さ, フルスクリーン)" },
        { "SETSWITCH", "スイッチ設定 (パラメータ: スイッチ名, 状態)" },
        { "SETDESTINYSWITCH", "デスティニスイッチ設定 (パラメータ: スイッチ名, 状態)" },
        { "STUN", "スタン状態を適用" },
        { "TELEPORT", "テレポートパネルを開く" },
        { "ABILITY", "アビリティパネルを開く" },
        { "ITEMBOX", "アイテムボックスパネルを開く" },
        { "DESTINYBOOK", "運命の刻印の本を開く" },
        { "MESSAGEBOX", "メッセージボックステスト" },
        { "RESTART", "ゲーム再起動" },
        { "SKIPPROLOGUE", "プロローグスキップ" },
        { "ADDEXP", "経験値追加 (パラメータ: 数量)" },
        { "ADDITEM", "アイテム追加 (パラメータ: アイテムID, 数量)" },
        { "ADDWEAPON", "武器追加 (パラメータ: 武器ID)" },
        { "UNEQUIPWEAPON", "武器解除" },
        { "ADDMONEY", "お金追加 (パラメータ: 数量)" },
        { "IDENTIFICATION", "アイテム鑑定パネルを開く" },
        { "ENCHANT", "エンチャントパネルを開く" },
        { "SAVE", "ゲーム保存" },
        { "ADDDICE", "サイコロ追加 (パラメータ: 数量)" },
        { "ADDMAXDICE", "最大サイコロ増加 (パラメータ: 数量)" },
        { "HIDEUI", "UI非表示" },
        { "SHOWUI", "UI表示" },
        { "ADDINVENTORYSTORAGE", "インベントリ枠を追加 (パラメータ: 数量)" },
        { "REVIVE", "プレイヤーを復活 (パラメータ: プレイヤーID)" },
        { "DAMAGE", "プレイヤーへダメージ (パラメータ: プレイヤーID)" },
        { "SETCUSTOMSTAT", "カスタムステータスを設定 (パラメータ: ステータス名, 数値)" },
        { "READY", "マルチプレイヤー準備状態に変更" },
        { "CLEARSTEAMCLOUD", "Steamクラウド初期化" },
        { "HARDMODE", "ハードモードパネルを開く" },
        { "HARDMODEREWARDUNLOCK", "ハードモード報酬の才能ポイントをすべて解放" },
        { "SNARE_START", "捕獲開始" },
        { "SNARE_STOP", "捕獲終了" },
        { "ADDPASSIVEPOINT", "才能ポイント追加 (パラメータ: 数量)" },
        { "POCKETDIMENSIONSHOP", "ディメンションショップ" },
        { "UNLOCKPOCKETDIMENSIONSHOPITEM", "ディメンションショップアイテムをすべて解放" },
        { "UNLOCKBOSSITEM", "ボスアイテムをすべて解放" },
        { "SAVEBACKUP", "現在のセーブファイルバックアップ作成" },
        { "PROPSPAWN", "プロップ召喚 (パラメータ: プロップID)" },
        { "DIMENSIONPOCKET", "願いの泉を開く" },
        { "LORE_ALLVIEW", "すべてのLoreEntityを閲覧済み状態に変更" },
        { "LORE_ALLNEW", "すべてのLoreEntityを未閲覧状態に変更" },
        { "MULTIINVENTORY", "マルチプレイヤーインベントリを開く (パラメータ: プレイヤーID)" },
        { "CHAPTER", "チャプター選択" },
        { "TUTORIALSHOW", "チュートリアルカテゴリのポップアップ" },
        { "TAGBAN", "フルーツ串パネルを開く (ダンジョン外でのみ適用)" },
        { "EARLYACCESSENDPANEL", "アーリーアクセス終了パネルを開く" },
        { "GETTIEMFROMJOURNAL", "ジャーナルで右クリックしてアイテム/武器を即座に取得するモードを切り替え" },
        { "UNLOCKALLWEAPON", "すべての武器を強制アンロック" },
        { "UNLOCKTREESHOPITEMALL", "すべての運命の刻印を解放 (NewCharm_Bossを除く)" },
        { "LOCKTREESHOPITEM", "運命の刻印をロック (パラメータ: 運命の刻印ID)" },
        { "UNLOCKALL", "すべてのアンロック可能な項目を一括解放" },
        { "RESETALLACHIEVEMENT", "すべての実績をリセット" },
        { "CHANGEFONT", "フォント変更 (パラメータ: 言語, フォント名)" },
        { "GENERATEALLBOSS", "ステージ生成時にすべてのボスを生成" },
        { "TOGGLESEPHIRITESKIPGUIDE", "アイテム報酬UIのスキップガイドを切り替え" },
        { "SWAPSUBBAGANDINVENTORY", "サブバッグとインベントリを入れ替え (パラメータ: X, Y, XSubBag)" },
        { "ENEMYHPMULTIPLIER", "敵のHP倍率 (150 = 150%)" }
    };
        public static Dictionary<string, string> commandDescriptions = new Dictionary<string, string>
    {
        { "MIRACLE", "미라클 셀렉터 생성" },
        { "ROOMINFO", "방 정보 표시 토글" },
        { "MULTI", "멀티플레이어 패널 열기" },
        { "ADDPOTIONSTORAGE", "포션 저장소 추가" },
        { "PASSIVE", "패시브 패널 열기" },
        { "WEAPONENHANCEMENT", "무기 강화 패널 열기" },
        { "TGM", "무적 모드 토글" },
        { "ADDKEY", "열쇠 추가 (매개변수: 키 ID)" },
        { "SUICIDE", "자살 명령어" },
        { "ADDSAPPHIRE", "사파이어 추가 (매개변수: 수량)" },
        { "COSTUME", "코스튬 패널 열기" },
        { "EQUIPCOSTUME", "코스튬 장착 (매개변수: 코스튬 ID)" },
        { "UNLOCKCOSTUME", "코스튬 강제 언락 (매개변수: 코스튬 ID)" },
        { "UNLOCKCOSTUMEALL", "모든 코스튬 강제 언락" },
        { "UNLOCKCOSTUMESKINALL", "모든 코스튬 외형 강제 해금" },
        { "LOCKCOSTUME", "코스튬 잠금 처리 (매개변수: 코스튬 ID)" },
        { "CONNECT", "다른 호스트에 연결 (매개변수: 호스트 ID)" },
        { "GOTOHOSTTOWN", "호스트 마을로 이동" },
        { "GOTOMYTOWN", "내 마을로 이동" },
        { "WORLDMAP", "월드맵 패널 열기" },
        { "DISTORTION", "화면 왜곡 효과 재생" },
        { "GLITCH", "글리치 효과 재생" },
        { "SUMMON", "유닛 소환 (매개변수: 유닛 ID, [선택적] 수량)" },
        { "SUMMON_POS", "지정 위치에 유닛 소환 (매개변수: 유닛 ID, X, Y)" },
        { "SUMMONNPC", "NPC 소환 (매개변수: NPC 이름)" },
        { "TREESHOP", "트리 상점 패널 열기" },
        { "TUTORIALCLEAR", "튜토리얼 클리어 처리" },
        { "SETLANGUAGE", "언어 설정 변경 (매개변수: 언어 코드)" },
        { "SETRESOLUTION", "해상도 설정 (매개변수: 가로, 세로, 전체화면)" },
        { "SETSWITCH", "스위치 설정 (매개변수: 스위치명, 상태)" },
        { "SETDESTINYSWITCH", "데스티니 스위치 설정 (매개변수: 스위치명, 상태)" },
        { "STUN", "스턴 상태 적용" },
        { "TELEPORT", "텔레포트 패널 열기" },
        { "ABILITY", "어빌리티 패널 열기" },
        { "ITEMBOX", "아이템 박스 패널 열기" },
        { "DESTINYBOOK", "데스티니 북 패널 열기" },
        { "MESSAGEBOX", "메시지 박스 테스트" },
        { "RESTART", "게임 재시작" },
        { "SKIPPROLOGUE", "프롤로그 스킵" },
        { "ADDEXP", "경험치 추가 (매개변수: 수량)" },
        { "ADDITEM", "아이템 추가 (매개변수: 아이템 ID, 수량)" },
        { "ADDWEAPON", "무기 추가 (매개변수: 무기 ID)" },
        { "UNEQUIPWEAPON", "무기 해제" },
        { "ADDMONEY", "돈 추가 (매개변수: 수량)" },
        { "IDENTIFICATION", "아이템 감정 패널 열기" },
        { "ENCHANT", "인챈트 패널 열기" },
        { "SAVE", "게임 저장" },
        { "ADDDICE", "주사위 추가 (매개변수: 수량)" },
        { "ADDMAXDICE", "최대 주사위 증가 (매개변수: 수량)" },
        { "HIDEUI", "UI 숨기기" },
        { "SHOWUI", "UI 표시" },
        { "ADDINVENTORYSTORAGE", "인벤토리 저장공간 추가 (매개변수: 수량)" },
        { "REVIVE", "플레이어 부활 (매개변수: 플레이어 ID)" },
        { "DAMAGE", "플레이어에게 데미지 (매개변수: 플레이어 ID)" },
        { "SETCUSTOMSTAT", "커스텀 스탯 설정 (매개변수: 스탯명, 수치)" },
        { "READY", "멀티플레이어 준비 상태로 변경" },
        { "CLEARSTEAMCLOUD", "스팀 클라우드 초기화" },
        { "HARDMODE", "하드모드 패널 열기" },
        { "HARDMODEREWARDUNLOCK", "하드모드 보상 패시브 포인트 모두 해금" },
        { "SNARE_START", "잡힘 시작" },
        { "SNARE_STOP", "잡힘 종료" },
        { "ADDPASSIVEPOINT", "패시브 포인트 추가 (매개변수: 수량)" },
        { "POCKETDIMENSIONSHOP", "아공간 상점" },
        { "UNLOCKPOCKETDIMENSIONSHOPITEM", "아공간 상점 아이템 모두 해금" },
        { "UNLOCKBOSSITEM", "보스 아이템 모두 해금" },
        { "SAVEBACKUP", "현재 세이브 파일 백업 생성" },
        { "PROPSPAWN", "프롭 소환 (매개변수: 프롭 ID)" },
        { "DIMENSIONPOCKET", "차원 주머니 열기" },
        { "LORE_ALLVIEW", "모든 로어엔티티를 조회됨 상태로 변경" },
        { "LORE_ALLNEW", "모든 로어엔티티를 미조회 상태로 변경" },
        { "MULTIINVENTORY", "멀티플레이어 인벤토리 열기 (매개변수: 플레이어 ID)" },
        { "CHAPTER", "챕터 선택" },
        { "TUTORIALSHOW", "튜토리얼 카테고리 팝업" },
        { "TAGBAN", "과일 꼬치 패널 열기(던전 밖에서만 적용됨)" },
        { "EARLYACCESSENDPANEL", "얼리 액세스 종료 패널 열기" },
        { "GETTIEMFROMJOURNAL", "저널에서 오른쪽 클릭으로 아이템/무기 즉시 획득 모드 토글" },
        { "UNLOCKALLWEAPON", "모든 무기 강제 언락" },
        { "UNLOCKTREESHOPITEMALL", "모든 트리샵 아이템 해금 (NewCharm_Boss 제외)" },
        { "LOCKTREESHOPITEM", "트리샵 아이템 잠금 처리 (매개변수: 트리샵 아이템 ID)" },
        { "UNLOCKALL", "모든 언락 가능한 항목 일괄 해금" },
        { "RESETALLACHIEVEMENT", "모든 업적 초기화" },
        { "CHANGEFONT", "폰트 변경 (매개변수: 언어, 폰트 이름)" },
        { "GENERATEALLBOSS", "스테이지 생성 시 모든 보스 생성" },
        { "TOGGLESEPHIRITESKIPGUIDE", "Toggle Item Reward UI's Skip Guide" },
        { "SWAPSUBBAGANDINVENTORY", "Swap sub bag and inventory (params X, Y, XSubBag)" },
        { "ENEMYHPMULTIPLIER", "Enemy HP Multiplier (150 = 150%)" }
    };

        [HarmonyPatch(typeof(UI_DevCommandlinePanel), nameof(UI_DevCommandlinePanel.OnOpened))]
        public static class DevCommandPanelPatch
        {
            static void Postfix(UI_DevCommandlinePanel __instance)
            {
                if(LocalizationManager.Instance.CurrentLanguage == "ja-JP")
                {
                    __instance.SetCommandDescriptions(commandDescriptionsJapanese);
                }
                else
                {
                    __instance.SetCommandDescriptions(commandDescriptions);
                }
            }
        }
        #endregion

        #region HandleDungeonEnvironmentChanged
        [HarmonyPatch(typeof(DungeonManager), "HandleDungeonEnvironmentChanged")]
        public static class DungeonManagerHandleDungeonEnvironmentChangedPatch
        {
            static void Postfix(DungeonManager __instance, SyncIDictionary<string, int>.Operation op, string key, int item)
            {
                if (key == "MiniBossKillCount")
                {
                    OnMiniBossKillCountChanged?.Invoke(item);
                }
            }
        }
        #endregion
    }
}
