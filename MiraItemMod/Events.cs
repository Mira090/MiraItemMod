using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Mirror;
using SephiriaMod.Items;
using SephiriaMod.UI;
using SephiriaMod.Utilities;
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

namespace SephiriaMod
{
    public static class Events
    {
        public static event Action<WeaponSimple_Crossbow> OnSubAttackCrossbow;
        public static event Action<WeaponSimple_Katana> OnSubAttackKatana;
        public static event Action<WeaponSimple_GreatSword> OnSubAttackGreatSword;
        public static event Action<Charm_PallasCard, int> OnPallasSpawnChance;
        public static event Action<Charm_PallasAce, int> OnAceSpawnChance;
        public static event Action<string, uint, int> OnValueRecieved;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreBasicAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreSpecialAttack;
        public static event Action<WeaponControllerSimple, UnitAvatar> OnPreDashAttack;
        public static event Action<CharacterBuff> OnAppliedBuff;

        public static EventReference HealSound { get; } = RuntimeManager.PathToEventReference("event:/Scene/healPotion_Small01");
        public static EventReference PerkSound { get; } = RuntimeManager.PathToEventReference("event:/System/talentPerk");
        static Events()
        {
            OnValueRecieved += (string command, uint netId, int value) =>
            {
                //Core.Logger($"Mod Chat({command}): {netId} To {value}");
            };
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

        #region パラスのジョーカー
        [HarmonyPatch(typeof(Charm_PallasCard), "OnBeginAttackAnimation", new Type[] { typeof(int) })]
        public static class Charm_PallasCardOnBeginAttackAnimationPatch
        {
            static void Prefix(int idx, Charm_PallasCard __instance)
            {
                if (!__instance.WeaponController || !__instance.WeaponController.currentWeapon || !__instance.throwIntervalTimer.Check())
                {
                    return;
                }
                OnPallasSpawnChance?.Invoke(__instance, idx);
            }
        }
        public static void InvokeOnAceSpawnChance(int idx, Charm_PallasAce __instance)
        {
            OnAceSpawnChance?.Invoke(__instance, idx);
        }
        #endregion

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
                panel.statElements.AddRangeToArray(manager.Stats.ToArray());
                return manager;
            }
        }
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
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing), new Type[] { typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixBasic(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }

            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwingFullyManual), new Type[] { typeof(NewWeaponFireData), typeof(int), typeof(Vector2), typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixBasicFullyManual(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateBasicAttackSwing_ManualDirection), new Type[] { typeof(int), typeof(Vector2), typeof(string) })]
            [HarmonyPrefix]
            public static void PrefixBasicManualDirection(WeaponControllerSimple __instance)
            {
                OnPreBasicAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateDashAttackSwing), new Type[] { typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponControllerSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwing), new Type[] { typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CreateSpecialAttackSwingFullyManual), new Type[] { typeof(NewWeaponFireData), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponControllerSimple __instance)
            {
                OnPreSpecialAttack?.Invoke(__instance, __instance.unitAvatar);
            }
        }
        [HarmonyPatch]
        public class WeaponSimplePatch
        {
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateDashAttackProjectile), new Type[] { typeof(int), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(float), typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixDash(WeaponSimple __instance)
            {
                OnPreDashAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectile), new Type[] { typeof(int), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixSpecial(WeaponSimple __instance)
            {
                //Core.Logger("PrefixSpecial");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateSpecialAttackProjectileFullyManual), new Type[] { typeof(NewWeaponFireData), typeof(Vector3), typeof(Vector3), typeof(List<CombatBehaviour>), typeof(float), typeof(int), typeof(int), typeof(int) })]
            [HarmonyPrefix]
            public static void PrefixSpecialFullyManual(WeaponSimple __instance)
            {
                //Core.Logger("PrefixSpecialFullyManual");
                OnPreSpecialAttack?.Invoke(__instance.owner, __instance.owner.unitAvatar);
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
                        __result.AddItem(ItemCategories.Grimoire);
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
        /*

        [HarmonyPatch(typeof(NewItemOwnInstance), nameof(NewItemOwnInstance.DoClickAction))]
        public static class Patch
        {
            static void Postfix(NewItemOwnInstance __instance)
            {
                if (__instance.Entity == null || __instance.Entity.type != EItemType.Identifiable)
                    return;
                IdentifiableItem_Rarity
            }
        }*/
    }
}
